using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.UI;

namespace OldGoldMine.Engine
{
    public static class Resources
    {
        /* 3D models for the game */
        private static readonly Dictionary<string, Model> models 
            = new Dictionary<string, Model>();

        /// <summary>
        /// Add a 3D model to the resource collection.
        /// </summary>
        /// <param name="name">The name that will be used to access this resource in the future.</param>
        /// <param name="model">The Model that has to be added to the collection.</param>
        public static void AddModel(string name, Model model)
        {
            models.TryAdd(name, model);
        }

        /// <summary>
        /// Remove a 3D model from the resource collection.
        /// </summary>
        /// <param name="name">The name of the resource that has to be removed.</param>
        public static void RemoveModel(string name)
        {
            models.Remove(name);
        }

        /// <summary>
        /// Check if a specific 3D model is already loaded and available in the collection.
        /// </summary>
        /// <param name="name">Name of the resource that has to be checked.</param>
        /// <returns>Boolean value indicating if the requested 3D model is present in the collection.</returns>
        public static bool ContainsModel(string name)
        {
            return models.ContainsKey(name);
        }

        /// <summary>
        /// Retrieve a 3D model from the resource collection, by name.
        /// </summary>
        /// <param name="name">The name of the resource that has to be retrieved.</param>
        /// <returns>The 3D model corresponding to the provided name (or null).</returns>
        public static Model GetModel(string name)
        {
            if (models.TryGetValue(name, out Model model))
            {
                return model;
            }
            return null;
        }


        /* UI elements sprite packs */
        private static readonly Dictionary<string, Button.SpritePack> spritePacks 
            = new Dictionary<string, Button.SpritePack>();

        /// <summary>
        /// Add a SpritePack to the resource collection.
        /// </summary>
        /// <param name="name">The name that will be used to access this resource in the future.</param>
        /// <param name="spritePack">The SpritePack that has to be added to the collection.</param>
        public static void AddSpritePack(string name, Button.SpritePack spritePack)
        {
            spritePacks.TryAdd(name, spritePack);
        }

        /// <summary>
        /// Remove a SpritePack from the resource collection.
        /// </summary>
        /// <param name="name">The name of the resource that has to be removed.</param>
        public static void RemoveSpritePack(string name)
        {
            spritePacks.Remove(name);
        }

        /// <summary>
        /// Check if a specific SpritePack is already loaded and available in the collection.
        /// </summary>
        /// <param name="name">Name of the resource that has to be checked.</param>
        /// <returns>Boolean value indicating if the requested SpritePack is present in the collection.</returns>
        public static bool ContainsSpritePack(string name)
        {
            return spritePacks.ContainsKey(name);
        }

        /// <summary>
        /// Retrieve a SpritePack from the resource collection, by name.
        /// </summary>
        /// <param name="name">The name of the resource that has to be retrieved.</param>
        /// <returns>The SpritePack corresponding to the provided name (or an empty one).</returns>
        public static Button.SpritePack GetSpritePack(string name)
        {
            if (spritePacks.TryGetValue(name, out Button.SpritePack spritePack))
            {
                return spritePack;
            }
            return new Button.SpritePack();
        }


        /* Images and textures */
        private static readonly Dictionary<string, Texture2D> textures
            = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Add a texture to the resource collection.
        /// </summary>
        /// <param name="name">The name that will be used to access this resource in the future.</param>
        /// <param name="texture">The texture that has to be added to the collection.</param>
        public static void AddTexture(string name, Texture2D texture)
        {
            textures.TryAdd(name, texture);
        }

        /// <summary>
        /// Remove a texture from the resource collection.
        /// </summary>
        /// <param name="name">The name of the resource that has to be removed.</param>
        public static void RemoveTexture(string name)
        {
            textures.Remove(name);
        }

        /// <summary>
        /// Check if a specific textures is already loaded and available in the collection.
        /// </summary>
        /// <param name="name">Name of the resource that has to be checked.</param>
        /// <returns>Boolean value indicating if the requested texture is present in the collection.</returns>
        public static bool ContainsTexture(string name)
        {
            return textures.ContainsKey(name);
        }

        /// <summary>
        /// Retrieve a texture from the resource collection, by name.
        /// </summary>
        /// <param name="name">The name of the resource that has to be retrieved.</param>
        /// <returns>The texture corresponding to the provided name (or null).</returns>
        public static Texture2D GetTexture(string name)
        {
            if (textures.TryGetValue(name, out Texture2D texture))
            {
                return texture;
            }
            return null;
        }


        /* Fonts */
        private static readonly Dictionary<string, SpriteFont> fonts
            = new Dictionary<string, SpriteFont>();

        /// <summary>
        /// Add a font to the resource collection.
        /// </summary>
        /// <param name="name">The name that will be used to access this resource in the future.</param>
        /// <param name="font">The font that has to be added to the collection.</param>
        public static void AddFont(string name, SpriteFont font)
        {
            fonts.TryAdd(name, font);
        }

        /// <summary>
        /// Remove a font from the resource collection.
        /// </summary>
        /// <param name="name">The name of the resource that has to be removed.</param>
        public static void RemoveFont(string name)
        {
            fonts.Remove(name);
        }

        /// <summary>
        /// Check if a specific fonts is already loaded and available in the collection.
        /// </summary>
        /// <param name="name">Name of the resource that has to be checked.</param>
        /// <returns>Boolean value indicating if the requested font is present in the collection.</returns>
        public static bool ContainsFont(string name)
        {
            return fonts.ContainsKey(name);
        }

        /// <summary>
        /// Retrieve a font from the resource collection, by name.
        /// </summary>
        /// <param name="name">The name of the resource that has to be retrieved.</param>
        /// <returns>The font corresponding to the provided name (or null).</returns>
        public static SpriteFont GetFont(string name)
        {
            if (fonts.TryGetValue(name, out SpriteFont font))
            {
                return font;
            }
            return null;
        }

    }
}
