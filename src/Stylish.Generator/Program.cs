var emojis = await Stylish.Generator.Emoji.EmojiGenerator.Generate ( "..\\..\\..\\..\\Stylish\\Fonts" ).ConfigureAwait ( false );

Console.WriteLine ( $"Generated { emojis.Length } emojis." );