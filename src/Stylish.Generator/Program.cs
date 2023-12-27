var emojis = await Stylish.Fonts.Generators.EmojiGenerator.Generate ( "..\\..\\..\\..\\Stylish\\Fonts" ).ConfigureAwait ( false );

Console.WriteLine ( $"Generated { emojis.Length } emojis." );