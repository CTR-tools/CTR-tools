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
        public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, TriList> Models = new Dictionary<string, TriList>();

        public static List<string> alphalist = new List<string>();
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
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate texture: '{name}'.");
                return false;
            }

            Textures.Add(name, texture);
            return true;
        }

        public static bool AddModel(string name, TriList model)
        {
            if (Models.ContainsKey(name))
            {
                Helpers.Panic("ContentVault", PanicType.Warning, $"Attempted to add a duplicate model: '{name}'.");
                return false;
            }

            Models.Add(name, model);
            return true;
        }

        public static TriList GetModel(string name)
        {
            if (!Models.ContainsKey(name))
                return null;

            return Models[name];
        }
        public static Texture2D GetTexture(string name)
        {
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