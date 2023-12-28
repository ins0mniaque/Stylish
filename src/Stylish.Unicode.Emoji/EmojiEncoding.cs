using System.Buffers.Binary;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Stylish.Unicode;

public static class EmojiEncoding
{
    private const char Plane0                    = '\u0000';
    private const char Plane1                    = '\u0001';
    private const char Plane14                   = '\u000E';
    private const char HighBit                   = '\u8000';
    private const char EmojiPresentationSelector = '\uFE0F';
    private const char TextPresentationSelector  = '\uFE0E';
    private const char ZeroWidthJoiner           = '\u200D';

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

    private const char CombiningEnclosingKeycap = '\u20E3';

    private const char RegionalFlag       = '\uF3F4';
    private const char RegionalIndicatorA = '\uF1E6';
    private const char RegionalIndicatorZ = '\uF1FF';
    private const char CancelTag          = '\u007F';

    private const int Plane1CodePoint       = Plane1  << 16;
    private const int Plane14CodePoint      = Plane14 << 16;
    private const int RegionalFlagCodePoint = Plane1  << 16 | RegionalFlag;
    private const int CancelTagCodePoint    = Plane14 << 16 | CancelTag;

    private const int PlaneAndHighByteBitMask = char.MaxValue << 16 | byte.MaxValue << 8;
    private const int PlaneAndHighBitMask     = char.MaxValue << 16 | HighBit;
    private const int Plane1HighBit1          = Plane1 << 16 | HighBit;
    private const int Plane0HighBit1          = Plane0 << 16 | HighBit;
    private const int Plane0HighBit0          = Plane0 << 16;

    private const long Base64BitMask       = (long) char.MaxValue << 48;
    private const long RegionalFlagBitMask = (long) RegionalFlag  << 48;

    public static string SetVariant ( string emoji, Variant variant )
    {
        ArgumentNullException.ThrowIfNull ( emoji );

        var encoded = (Span < char >) stackalloc char [ emoji.Length * 2 ];
        var length  = 0;

        for ( var index = 0; index < emoji.Length; index++ )
        {
            var code = emoji [ index ];

            if ( index > 0 && code is ZeroWidthJoiner or CombiningEnclosingKeycap && variant is Variant.Monochrome or Variant.Color )
                encoded [ length++ ] = (char) variant;

            if ( code is not EmojiPresentationSelector and not TextPresentationSelector )
                encoded [ length++ ] = code;
        }

        if ( variant is Variant.Monochrome or Variant.Color )
            encoded [ length++ ] = (char) variant;

        return new string ( encoded [ ..length ] );
    }

    public static unsafe long? Encode ( string emoji )
    {
        ArgumentNullException.ThrowIfNull ( emoji );

        const int CodeSize = sizeof ( long ) / sizeof ( char );

        var encoded = (Span < char >) stackalloc char [ CodeSize ];
        var length  = 0;

        for ( var index = 0; index < emoji.Length; index += char.IsSurrogatePair ( emoji, index ) ? 2 : 1 )
        {
            if ( length > CodeSize )
                return null;

            var codepoint = char.ConvertToUtf32 ( emoji, index );

            if ( length is 0 && codepoint is RegionalFlagCodePoint && emoji.Length > 4 )
            {
                if ( char.IsSurrogatePair ( emoji, emoji.Length - 2 ) && char.ConvertToUtf32 ( emoji, emoji.Length - 2 ) is CancelTagCodePoint )
                    return Alphanumeric.Encode ( emoji );
            }

            Pack ( encoded, ref length, codepoint );
        }

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
        if ( ( emoji & Base64BitMask ) is RegionalFlagBitMask )
            return Alphanumeric.Decode ( emoji );

        const int CodeSize    = sizeof ( long ) / sizeof ( char );
        const int UnpackSize  = 6;
        const int VariantSize = sizeof ( char );
        const int DecodeSize  = CodeSize * UnpackSize + VariantSize;

        var decoded = (Span < char >) stackalloc char [ DecodeSize ];
        var length  = 0;

        Unpack ( decoded, ref length, (char) ( emoji >> 48 ), variant );
        Unpack ( decoded, ref length, (char) ( emoji >> 32 ), variant );
        Unpack ( decoded, ref length, (char) ( emoji >> 16 ), variant );
        Unpack ( decoded, ref length, (char)   emoji,         variant );
        Unpack ( decoded, ref length,                         variant );

        const int Utf16Size = DecodeSize;

        var utf16       = (Span < char >) stackalloc char [ Utf16Size ];
        var utf16Length = 0;

        for ( var index = 0; index < length; index++ )
        {
            var high = decoded [ index ];

            if ( high is Plane1 )
                EncodeToUtf16 ( utf16, ref utf16Length, high << 16 | decoded [ ++index ] );
            else
                EncodeToUtf16 ( utf16, ref utf16Length, high );
        }

        return new string ( utf16 [ ..utf16Length ] );
    }

    private static void Pack ( Span < char > encoded, ref int length, int codepoint )
    {
        if ( ( codepoint & PlaneAndHighBitMask ) is Plane1HighBit1 or Plane0HighBit1 or Plane0HighBit0 )
        {
            var code = (char) codepoint;

            if ( length > 0 && IsBetween ( code, LightSkinTone, DarkSkinTone ) )
            {
                ref var previous = ref encoded [ length - 1 ];

                if      ( previous is Person ) previous = (char) ( LightSkinPerson + ( code - LightSkinTone ) );
                else if ( previous is Man    ) previous = (char) ( LightSkinMan    + ( code - LightSkinTone ) );
                else if ( previous is Woman  ) previous = (char) ( LightSkinWoman  + ( code - LightSkinTone ) );
                else                           encoded [ length++ ] = code;
            }
            else if ( code is not ZeroWidthJoiner and not EmojiPresentationSelector and not TextPresentationSelector )
                encoded [ length++ ] = code;
        }
        else
        {
            encoded [ length++ ] = (char) ( codepoint >> 16 );
            encoded [ length++ ] = (char) codepoint;
        }
    }

    private static void Unpack ( Span < char > decoded, ref int length, char code, Variant variant )
    {
        if ( length > 0 )
        {
            var isZeroWidthJoined = IsZeroWidthJoined ( code );

            if ( variant is Variant.Color or Variant.Monochrome && ( isZeroWidthJoined || code is CombiningEnclosingKeycap ) )
                decoded [ length++ ] = (char) variant;

            if ( isZeroWidthJoined )
                decoded [ length++ ] = ZeroWidthJoiner;
        }

        if ( code is not EmojiPresentationSelector or TextPresentationSelector && ( code & HighBit ) is not 0 )
        {
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = code;
        }
        else if ( IsBetween ( code, LightSkinPerson, DarkSkinPerson ) )
        {
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = Person;
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = (char) ( LightSkinTone + ( code - LightSkinPerson ) );
        }
        else if ( IsBetween ( code, LightSkinMan, DarkSkinMan ) )
        {
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = Man;
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = (char) ( LightSkinTone + ( code - LightSkinMan ) );
        }
        else if ( IsBetween ( code, LightSkinWoman, DarkSkinWoman ) )
        {
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = Woman;
            decoded [ length++ ] = Plane1;
            decoded [ length++ ] = (char) ( LightSkinTone + ( code - LightSkinWoman ) );
        }
        else if ( code is not char.MinValue )
            decoded [ length++ ] = code;
    }

    private static void Unpack ( Span < char > decoded, ref int length, Variant variant )
    {
        if ( length is 0 || IsZeroWidthJoined ( decoded [ length ] ) && variant is Variant.Color or Variant.Monochrome )
            decoded [ length++ ] = (char) variant;
    }

    private static class Alphanumeric
    {
        private const byte Base64MaxValue = (byte) '/';

        public static unsafe long? Encode ( string emoji )
        {
            const int CodeSize          = sizeof ( long );
            const int PrefixSize        = sizeof ( char );
            const int EncodedBase64Size = CodeSize - PrefixSize;
            const int DecodedBase64Size = EncodedBase64Size * 4 / 3;
            const int EncodeSize        = PrefixSize + DecodedBase64Size;

            var encoded = (Span < byte >) stackalloc byte [ EncodeSize ];
            var length  = PrefixSize;

            for ( var index = 0; index < emoji.Length; index += char.IsSurrogatePair ( emoji, index ) ? 2 : 1 )
            {
                if ( length > EncodeSize )
                    return null;

                var codepoint = char.ConvertToUtf32 ( emoji, index );

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

            var base64 = encoded [ PrefixSize.. ].ToArray ( );

            Base64.DecodeFromUtf8InPlace ( encoded [ PrefixSize.. ], out length );

            encoded [ ( PrefixSize + length )..CodeSize ].Clear ( );

            var code = MemoryMarshal.Read < long > ( encoded );

            if ( BitConverter.IsLittleEndian )
                return BinaryPrimitives.ReverseEndianness ( code );

            return code;
        }

        public static unsafe string Decode ( long emoji )
        {
            const int CodeSize          = sizeof ( long );
            const int PrefixSize        = sizeof ( char );
            const int EncodedBase64Size = CodeSize - PrefixSize;
            const int DecodedBase64Size = EncodedBase64Size * 4 / 3;
            const int Terminator        = sizeof ( byte );
            const int DecodeSize        = PrefixSize + DecodedBase64Size + Terminator;

            var decoded = (Span < byte >) stackalloc byte [ DecodeSize ];

            if ( BitConverter.IsLittleEndian )
                emoji = BinaryPrimitives.ReverseEndianness ( emoji );

            #if NET8_0
            MemoryMarshal.Write ( decoded, in emoji );
            #else
            MemoryMarshal.Write ( decoded, ref emoji );
            #endif

            var base64 = decoded [ PrefixSize.. ];

            Base64.EncodeToUtf8InPlace ( base64, EncodedBase64Size, out _ );

            base64 [ CodeSize ] = Base64MaxValue;

            const int Utf16RuneSize = sizeof ( int ) / sizeof ( char );
            const int Utf16Size     = DecodeSize * Utf16RuneSize;

            var utf16       = (Span < char >) stackalloc char [ Utf16Size ];
            var utf16Length = 0;

            EncodeToUtf16 ( utf16, ref utf16Length, Plane1CodePoint | decoded [ 0 ] << 8 | decoded [ 1 ] );

            var base64Length = base64.IndexOf ( Base64MaxValue );
            for ( var index = 0; index < base64Length; index++ )
                EncodeToUtf16 ( utf16, ref utf16Length, Plane14CodePoint | base64 [ index ] );

            EncodeToUtf16 ( utf16, ref utf16Length, CancelTagCodePoint );

            return new string ( utf16 [ ..utf16Length ] );
        }
    }

    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    private static void EncodeToUtf16 ( Span < char > destination, ref int length, int codepoint )
    {
        length += new Rune ( codepoint ).EncodeToUtf16 ( destination [ length.. ] );
    }

    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    private static bool IsBetween ( char c, char minInclusive, char maxInclusive )
    {
        return (uint) ( c - minInclusive ) <= (uint) ( maxInclusive - minInclusive );
    }

    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    private static bool IsZeroWidthJoined ( char code )
    {
        return code is not char.MinValue and not CombiningEnclosingKeycap   &&
               ! IsBetween ( code, LightSkinTone,      DarkSkinTone       ) &&
               ! IsBetween ( code, RegionalIndicatorA, RegionalIndicatorZ );
    }
}