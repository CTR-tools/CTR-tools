using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    public class ContentVault
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static List<string> alphalist = new List<string>();

        public static Dictionary<string, TriList> Tris = new Dictionary<string, TriList>();
        public static Dictionary<string, QuadList> Models = new Dictionary<string, QuadList>();

        public static bool AddTexture(string name, Texture2D texture)
        {
            if (Textures.ContainsKey(name))
            {
                Helpers.Panic("ContentVault", CTRFramework.PanicType.Warning, $"Attempted to add a duplicate texture: '{name}'.");
                return false;
            }

            Textures.Add(name, texture);
            return true;
        }

        public static void Clear()
        {
            alphalist.Clear();
            Textures.Clear();

            Tris.Clear();
            Models.Clear();
        }
    }
}