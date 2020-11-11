using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ContentLoader.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace ContentLoader
{
    public class ContentLoader
    {
        // Public Textures
        public static Dictionary<string, Texture2D> textureSheets;
        public static Dictionary<string, Dictionary<string, Rectangle>> textures;
        public static Dictionary<string, Dictionary<string, SoundEffect>> sounds;
        public static Dictionary<string, SpriteFont> fonts;

        private static ContentManager Content;

        public static void Init(ContentManager content)
        {
            // Set the content
            Content = content;
            textureSheets = new Dictionary<string, Texture2D>();
            textures = new Dictionary<string, Dictionary<string, Rectangle>>();
            sounds = new Dictionary<string, Dictionary<string, SoundEffect>>();
            fonts = new Dictionary<string, SpriteFont>();
        }

        public static void Load()
        {
            // Load fonts
            fonts.Add("JetBrainsMono", Content.Load<SpriteFont>("JetBrainsMono"));
            // Load the Content.json
            ContentFile contentFile = GetContentFile();
            // Load each sheet
            foreach (ContentIdentifier list in contentFile.Content)
            {
                textureSheets.Add(list.List, Content.Load<Texture2D>(list.List));
                EnumerateContentFromSheet(list, textureSheets[list.List]);
            }
#if DEBUG
            // On DEBUG print out all the loading shit
            foreach (var texture in textures)
            {
                Debug.WriteLine(texture.Key + " Loaded");
            }
            foreach (var sound in sounds)
            {
                Debug.WriteLine(sound.Key + " Loaded");
            }
            foreach (var font in fonts)
            {
                Debug.WriteLine(font.Key + " Loaded");
            }
#endif
        }

        private static void EnumerateContentFromSheet(ContentIdentifier contentIdentifier, Texture2D textureSheet)
        {
            // Get the Type of sheet
            Type type = Type.GetType(contentIdentifier.Type);
            // Get the sheet values
            dynamic sheetList = GetSheet(contentIdentifier);
            // Check if the list is a rectangle sheet
            if (sheetList is ITextureList textureList)
            {
                Dictionary<string, Rectangle> rectangleDictionary = CreateRectangleDictionary(sheetList, textureSheet);
                // Add the Texture list to the Dictionary
                textures.Add(contentIdentifier.List, rectangleDictionary);
            }
            // Check if the list is a sound sheet
            if (sheetList is SoundEffectList soundEffectList)
            {
                Dictionary<string, SoundEffect> soundEffectDictionary = CreateSoundDictionary(sheetList);
                // Add the SoundEffect list to the Dictionary
                sounds.Add(contentIdentifier.List, soundEffectDictionary);
            }
            
        }

        private static Dictionary<string, Rectangle> CreateRectangleDictionary(dynamic sheetList, Texture2D textureSheet)
        {
            // Check if the sheetList is indeed a textureList
            ITextureList textureList = sheetList as ITextureList;
            // Throw if that last line didn't work out as intended
            if (textureList == null) throw new Exception(sheetList + " is not a ITextureList");
            // Calculate the 16x16 rectangles on the sheet
            List<Rectangle> rectangles = CalculateRectangles(textureSheet, textureList.RectangleSize.ToPoint());
            // Define the rectangle texture to textureName dictionary
            Dictionary<string, Rectangle> rectangleDictionary = new Dictionary<string, Rectangle>();
            // Assign names to each texture
            for (int i = 0; i < sheetList.List.Count; i++)
            {
                rectangleDictionary.Add(sheetList.List[i].name, rectangles[i]);
            }
            return rectangleDictionary;
        }

        private static List<Rectangle> CalculateRectangles(Texture2D textureSheet, Point rectangleSize)
        {
            // Get the row and column count
            int rowCount = textureSheet.Width / rectangleSize.X;
            int columnCount = textureSheet.Height / rectangleSize.Y;
            // Define the row List
            List<Rectangle> rectangles = new List<Rectangle>();
            // Iterate through and get each texture
            for (int y = 0; y < columnCount; y++)
            {
                for (int x = 0; x < rowCount; x++)
                {
                    // Set and Add the rectangle
                    Rectangle rect = new Rectangle(x * 16, y * 16, 16, 16);
                    rectangles.Add(rect);
                }
            }
            return rectangles;
        }

        private static Dictionary<string, SoundEffect> CreateSoundDictionary(SoundEffectList sheetList)
        {
            // Define the return dictionary
            var soundDictionary = new Dictionary<string, SoundEffect>();
            // Load each sound asset along with their name
            for (int i = 0; i < sheetList.List.Count; i++)
            {
                // Load the sound effect in
                var soundEffect = Content.Load<SoundEffect>(sheetList.List[i].file);
                soundDictionary.Add(sheetList.List[i].name, soundEffect);
            }
            // Return sound dictionary
            return soundDictionary;
        }

        private static dynamic GetSheet(ContentIdentifier contentIdentifier)
        {
            // Import the sheet values
            using var streamReader = new StreamReader(Content.RootDirectory + "/" + contentIdentifier.List + ".json");
            JsonSerializer serializer = new JsonSerializer();
            var output = DeserializeList(streamReader, serializer, contentIdentifier);
            return output;
        }

        private static dynamic DeserializeList(StreamReader streamReader, JsonSerializer serializer, ContentIdentifier contentIdentifier)
        {
            dynamic output = false;
            Type type = Type.GetType(contentIdentifier.Type);
            output = Convert.ChangeType(serializer.Deserialize(streamReader, type), type);
            return output;
        }

        private static ContentFile GetContentFile()
        {
            // Deserialize the content.json
            using var streamReader = new StreamReader(Content.RootDirectory + "/Content.json");
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                ContentFile output = (ContentFile) serializer.Deserialize(streamReader, typeof(ContentFile));
                if (output.Content == null)
                {
                    throw new IOException("Content/Content.json was empty or not found.");
                }
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
