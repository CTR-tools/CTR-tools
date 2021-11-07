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
        public static SpriteFont Font;

        public static void Write(string message)
        {
            foreach (var line in message.Split("\r\n"))
            {
                Console.WriteLine(line);

                Lines.Add(line);
                if (Lines.Count > MaxLines)
                    Lines.RemoveAt(0);
            }
        }

        public static void Draw(GraphicsDevice gd, SpriteBatch g)
        {
            if (Font == null) return;
            if (gd == null) return;

            float scale = gd.Viewport.Height / 1080f;

            Vector2 loc = new Vector2(0.05f, 0.05f);

            float inc = Font.MeasureString(Lines[0]).Y * 1.25f * scale;

            foreach (string msg in Lines)
            {
                g.DrawString(
                Font,
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
