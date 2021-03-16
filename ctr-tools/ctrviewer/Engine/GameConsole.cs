using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    class GameConsole
    {
        public static int MaxLines = 40;
        public static Color color = Color.Lime;
        public static List<string> Lines = new List<string>();

        public static void Write(string message)
        {
            Console.WriteLine(message);

            Lines.Add(message);
            if (Lines.Count > MaxLines)
                Lines.RemoveAt(0);
        }

        public static SpriteFont font;

        public static void Draw(GraphicsDevice gd, SpriteBatch g)
        {
            if (font == null)
                return;

            if (gd == null)
                return;

            float scale = gd.Viewport.Height / 1080f;

            Vector2 loc = new Vector2(0.05f, 0.05f);

            float inc = font.MeasureString(Lines[0]).Y * 1.25f * scale;

            foreach (string msg in Lines)
            {
                g.DrawString(
                font,
                msg,
                loc,
                color,
                0,
                new Vector2(0, 0),
                scale,
                SpriteEffects.None,
                0.5f);

                loc.Y += inc;
            }
        }

        public static void Clear()
        {
            Lines.Clear();
        }
    }
}
