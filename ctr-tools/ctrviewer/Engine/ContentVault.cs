using CTRFramework;
using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    public class ContentVault
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> ReplacementTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, TriListCollection> Models = new Dictionary<string, TriListCollection>();
        public static Dictionary<string, Effect> Shaders = new Dictionary<string, Effect>();

        public static List<string> alphalist = new List<string>();

        public static bool AddShader(string name, Effect shader)
        {
            if (Shaders.ContainsKey(name))
            {
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate shader: '{name}'.");
                return false;
            }

            Shaders.Add(name, shader);
            return true;
        }

        public static bool AddSound(string name, SoundEffect sound)
        {
            if (Sounds.ContainsKey(name))
            {
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate sound: '{name}'.");
                return false;
            }

            Sounds.Add(name, sound);
            return true;
        }

        public static bool AddTexture(string name, Texture2D texture)
        {
            if (Textures.ContainsKey(name))
            {
                //Textures[name] = texture;
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate texture: '{name}'.");
                return false;
            }

            Textures.Add(name, texture);
            return true;
        }
        public static bool AddReplacementTexture(string name, Texture2D texture)
        {
            if (ReplacementTextures.ContainsKey(name))
            {
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate replacement texture: '{name}'.");
                return false;
            }

            ReplacementTextures.Add(name, texture);
            return true;
        }

        public static bool AddModel(string name, TriListCollection model)
        {
            if (Models.ContainsKey(name))
            {
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate model: '{name}'.");
                return false;
            }

            Models.Add(name, model);
            return true;
        }

        public static Effect GetShader(string name)
        {
            if (!Shaders.ContainsKey(name))
                return null;

            return Shaders[name];
        }

        public static TriListCollection GetModel(string name)
        {
            if (!Models.ContainsKey(name))
                return null;

            return Models[name];
        }

        public static Texture2D GetTexture(string name, bool useReplacements)
        {
            if (useReplacements)
            {
                if (ReplacementTextures.ContainsKey(name))
                    return ReplacementTextures[name];
            }

            if (!Textures.ContainsKey(name))
                return null;

            return Textures[name];
        }

        public static void Clear()
        {
            alphalist.Clear();
            Textures.Clear();

            Models.Clear();
        }
    }
}