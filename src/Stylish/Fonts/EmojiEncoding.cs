using System.Buffers.Binary;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Stylish.Fonts;

public static class EmojiEncoding
{
    private const char UnicodePlane0   = '\u0000';
    private const char UnicodePlane1   = '\u0001';
    private const char UnicodePlane14  = '\u000E';
    private const char HighBit         = '\u8000';
    private const char EmojiPresentationSelector    = '\uFE0F';
    private const char TextPresentationSelector    = '\uFE0E';
    private const char ZeroWidthJoiner = '\u200D';

    private const char Person = '\uF9D1';
    private const char Man    = '\uF468';
    private const char Woman  = '\uF469';

    private const char LightSkinPerson = '\u0001';
    private const char LightSkinMan    = '\u0006';
    private const char LightSkinWoman  = '\u000B';

    private const char DarkSkinPerson = '\u0005';
    private const char DarkSkinMan    = '\u000A';
    private const char DarkSkinWoman  = '\u000F';

    private const char LightSkinTone  = '\uF3FB';
    private const char DarkSkinTone   = '\uF3FF';

    private const char RegionalFlag       = '\uF3F4';
    private const char RegionalIndicatorA = '\uF1E6';
    private const char RegionalIndicatorZ = '\uF1FF';
    private const char CancelTag          = '\u007F';

    private const char CombiningEnclosingKeycap = '\u20E3';

    private const byte Base64MaxValue = (byte) '/';

    public static string SetVariant ( string emoji, Variant variant )
    {
        ArgumentNullException.ThrowIfNull ( emoji );

        emoji = emoji.TrimEnd ( (char) Variant.Monochrome, (char) Variant.Color );

        if ( variant is Variant.Monochrome or Variant.Color )
            return emoji + (char) variant;

        return emoji;
    }

    public static unsafe long? Encode ( string emoji )
    {
        ArgumentException.ThrowIfNullOrEmpty ( emoji );

        const int RegionalFlagCodePoint = UnicodePlane1  << 16 | RegionalFlag;
        const int CancelTagCodePoint    = UnicodePlane14 << 16 | CancelTag;

        var encoded = (Span < char >) stackalloc char [ 5 ];
        var length  = 0;

        for ( var index = 0; index < emoji.Length; index += char.IsSurrogatePair(emoji, index) ? 2 : 1 )
        {
            if ( length >= 5 )
                return null;

            var codepoint = char.ConvertToUtf32(emoji, index);

            if ( length is 0 && codepoint is RegionalFlagCodePoint && emoji.Length > 4 )
            {
                if ( char.IsSurrogatePair(emoji, emoji.Length - 2) && char.ConvertToUtf32(emoji, emoji.Length - 2) is CancelTagCodePoint )
                    return EncodeBase64 ( emoji );
            }

            Pack ( encoded, ref length, codepoint );
        }

        if ( encoded [ length - 1 ] is EmojiPresentationSelector or TextPresentationSelector )
            length--;

        return length switch
        {
            1 => encoded [ 0 ],
            2 => (long) encoded [ 0 ] << 16 | encoded [ 1 ],
            3 => (long) encoded [ 0 ] << 32 | (long) encoded [ 1 ] << 16 | encoded [ 2 ],
            4 => (long) encoded [ 0 ] << 48 | (long) encoded [ 1 ] << 32 | (long) encoded [ 2 ] << 16 | encoded [ 3 ],
            _ => null,
        };
    }

    public static string Decode ( long emoji, Variant variant = Variant.None )
    {
        const long Base64BitMask       = (long) char.MaxValue << 48;
        const long RegionalFlagBitMask = (long) RegionalFlag  << 48;

        if ( ( emoji & Base64BitMask ) is RegionalFlagBitMask )
            return DecodeBase64 ( emoji );

        var unpacked = (Span < char >) stackalloc char [ 20 ];
        var length   = 0;

        Unpack ( unpacked, ref length, (char) ( emoji >> 48 ) );
        Unpack ( unpacked, ref length, (char) ( emoji >> 32 ) );
        Unpack ( unpacked, ref length, (char) ( emoji >> 16 ) );
        Unpack ( unpacked, ref length, (char)   emoji );

        var decoded = (Span < char >) stackalloc char [ 20 ];
        var length2 = 0;

        for ( var index = 0; index < length; index++ )
        {
            var high = unpacked [ index ];

            if ( high is UnicodePlane1 )
                EncodeToUtf16 ( decoded, ref length2, high << 16 | unpacked [ ++index ] );
            else
                EncodeToUtf16 ( decoded, ref length2, high );
        }

        if ( variant is Variant.Color or Variant.Monochrome )
            EncodeToUtf16 ( decoded, ref length2, (int) variant );

        return new string ( decoded [ ..length2 ] );
    }

    private static unsafe long? EncodeBase64 ( string emoji )
    {
        const int PlaneAndHighByteBitMask = char.MaxValue  << 16 | byte.MaxValue << 8;
        const int Plane14CodePoint        = UnicodePlane14 << 16;
        const int CancelTagCodePoint      = UnicodePlane14 << 16 | CancelTag;

        var encoded = (Span < byte >) stackalloc byte [ 10 ];
        var length  = 2;

        for ( var index = 0; index < emoji.Length; index += char.IsSurrogatePair(emoji, index) ? 2 : 1 )
        {
            if ( length >= 10 )
                return null;

            var codepoint = char.ConvertToUtf32(emoji, index);

            if ( index > 0 )
            {
                if ( ( codepoint & PlaneAndHighByteBitMask ) is not Plane14CodePoint )
                    return null;

                if ( codepoint is not CancelTagCodePoint )
                    encoded [ length++ ] = (byte) ( codepoint - Plane14CodePoint );
            }
            else
            {
                encoded [ 0 ] = (byte) ( codepoint >> 8 );
                encoded [ 1 ] = (byte) codepoint;
            }
        }

        encoded [ length.. ].Fill ( Base64MaxValue );

        var base64 = encoded [ 2.. ].ToArray ( );

        Base64.DecodeFromUtf8InPlace ( encoded [ 2.. ], out length );

        encoded [ (length+2)..8 ].Clear ( );

        var e = MemoryMarshal.Read < long > (encoded);

        if (BitConverter.IsLittleEndian)
            return BinaryPrimitives.ReverseEndianness(e);

        return e;
    }

    private static unsafe string DecodeBase64 ( long emoji )
    {
        const int Plane1CodePoint    = UnicodePlane1  << 16;
        const int Plane14CodePoint   = UnicodePlane14 << 16;
        const int CancelTagCodePoint = UnicodePlane14 << 16 | CancelTag;

        var unpacked = (Span < byte >) stackalloc byte [ 11 ];

        if (BitConverter.IsLittleEndian)
            emoji = BinaryPrimitives.ReverseEndianness(emoji);

        MemoryMarshal.Write ( unpacked, ref emoji );

        var base64 = unpacked [ 2.. ];

        Base64.EncodeToUtf8InPlace ( base64, 6, out _ );

        base64 [ 8 ] = Base64MaxValue;
        var base64Length = base64.IndexOf ( Base64MaxValue );

        var decoded = (Span < char >) stackalloc char [ 20 ];
        var length2 = 0;

        EncodeToUtf16 ( decoded, ref length2, Plane1CodePoint | unpacked [ 0 ] << 8 | unpacked [ 1 ] );

        for ( var index = 0; index < base64Length; index++ )
            EncodeToUtf16 ( decoded, ref length2, Plane14CodePoint | base64 [ index ] );

        EncodeToUtf16 ( decoded, ref length2, CancelTagCodePoint );

        return new string ( decoded [ ..length2 ] );
    }

    private static void Pack ( Span < char > encoded, ref int length, int codepoint )
    {
        const int PlaneAndHighBitMask = char.MaxValue << 16 | HighBit;
        const int Plane1HighBit1      = UnicodePlane1 << 16 | HighBit;
        const int Plane0HighBit1      = UnicodePlane0 << 16 | HighBit;
        const int Plane0HighBit0      = UnicodePlane0 << 16;

        if ( ( codepoint & PlaneAndHighBitMask ) is Plane1HighBit1 or Plane0HighBit1 or Plane0HighBit0 )
        {
            var code = (char) codepoint;

            if ( length > 0 && char.IsBetween ( code, LightSkinTone, DarkSkinTone ) )
            {
                ref var previous = ref encoded [ length - 1 ];

                if      ( previous is Person ) previous = (char) ( LightSkinPerson + ( code - LightSkinTone ) );
                else if ( previous is Man    ) previous = (char) ( LightSkinMan    + ( code - LightSkinTone ) );
                else if ( previous is Woman  ) previous = (char) ( LightSkinWoman  + ( code - LightSkinTone ) );
                else                           encoded [ length++ ] = code;
            }
            else if ( code is not ZeroWidthJoiner )
                encoded [ length++ ] = code;
        }
        else
        {
            encoded [ length++ ] = (char) ( codepoint >> 16 );
            encoded [ length++ ] = (char) codepoint;
        }
    }

    private static void Unpack ( Span < char > decoded, ref int length, char code )
    {
        if ( length > 0 && IsZeroWidthJoined ( code ) )
            decoded [ length++ ] = ZeroWidthJoiner;

        if ( code is not EmojiPresentationSelector or TextPresentationSelector && ( code & HighBit ) is not 0 )
        {
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = code;
        }
        else if ( char.IsBetween ( code, LightSkinPerson, DarkSkinPerson ) )
        {
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = Person;
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = (char) ( LightSkinTone + ( code - LightSkinPerson ) );
        }
        else if ( char.IsBetween ( code, LightSkinMan, DarkSkinMan ) )
        {
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = Man;
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = (char) ( LightSkinTone + ( code - LightSkinMan ) );
        }
        else if ( char.IsBetween ( code, LightSkinWoman, DarkSkinWoman ) )
        {
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = Woman;
            decoded [ length++ ] = UnicodePlane1;
            decoded [ length++ ] = (char) ( LightSkinTone + ( code - LightSkinWoman ) );
        }
        else if ( code is not char.MinValue )
            decoded [ length++ ] = code;
    }

    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    private static void EncodeToUtf16 ( Span < char > destination, ref int length, int codepoint )
    {
        length += new Rune ( codepoint ).EncodeToUtf16 ( destination [ length.. ] );
    }

    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    private static bool IsZeroWidthJoined ( char code )
    {
        return code is not CombiningEnclosingKeycap &&
               ! char.IsBetween ( code, LightSkinTone,      DarkSkinTone       ) &&
               ! char.IsBetween ( code, RegionalIndicatorA, RegionalIndicatorZ );
    }
}