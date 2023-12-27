var emojis = await Stylish.Unicode.EmojiGenerator.Generate ( "..\\..\\..\\..\\Stylish.Unicode.Emoji" ).ConfigureAwait ( false );

Console.WriteLine ( $"Generated { emojis.Length } emojis." );