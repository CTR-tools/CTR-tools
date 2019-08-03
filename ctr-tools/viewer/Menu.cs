using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace viewer
{
    public class MenuItem
    {
        public string Title;
        public string Action;
        public string Param;
        public bool Enabled;
        public float Width;

        public MenuItem(string t, string a, string p, bool e)
        {
            Title = t;
            Action = a;
            Param = p;
            Enabled = e;
        }

        public void CalcWidth(SpriteFont font)
        {
            Width = font.MeasureString(Title).X;
        }
    }


    class Menu
    {
        public Vector2 Position = new Vector2(0.5f, 0.25f);
        public bool Exec = false;

        List<MenuItem> items = new List<MenuItem>();

        public MenuItem SelectedItem
        {
            get { return items[selection]; }
        }

        public int Selection
        {
            get { return selection; }
            set {
                selection = value;
                if (value >= items.Count) selection = 0;
                if (value < 0) selection = items.Count - 1;
            }
        }

        private int selection;

        public Menu(SpriteFont font)
        {
            selection = 0;

            LoadMenuItems();

            foreach (MenuItem m in items)
                m.CalcWidth(font);
        }

        public void LoadMenuItems()
        {
            items.Add(new MenuItem("paused".ToUpper(), "", "", false));
            items.Add(new MenuItem("toggle fullscreen".ToUpper(), "toggle", "window", true));
            items.Add(new MenuItem("toggle mouse".ToUpper(), "toggle", "mouse", true));
            items.Add(new MenuItem("---", "", "", false));
            items.Add(new MenuItem("toggle invisible".ToUpper(), "toggle", "invis", true));
            items.Add(new MenuItem("toggle wireframe".ToUpper(), "toggle", "wire", true));
            //items.Add(new MenuItem("toggle antialias".ToUpper(), "toggle", "antialias", true));
 
            items.Add(new MenuItem("previous flag".ToUpper(), "flag", "prev", true));
            items.Add(new MenuItem("next flag".ToUpper(), "flag", "next", true));
            items.Add(new MenuItem("---", "", "", false));

            items.Add(new MenuItem("reload level".ToUpper(), "load", "", true));
            //items.Add(new MenuItem("options".ToUpper(), "options", "", false));
            items.Add(new MenuItem("exit".ToUpper(), "exit", "", true));
        }

        public void Next()
        {
            Selection++;
        }

        public void Previous()
        {
            Selection--;
        }


        public void Update(GamePadState oldstate, GamePadState newstate)
        {
            if (newstate.DPad.Up == ButtonState.Pressed && newstate.DPad.Up != oldstate.DPad.Up) Previous();
            if (newstate.DPad.Down == ButtonState.Pressed && newstate.DPad.Down != oldstate.DPad.Down) Next();
            if (newstate.Buttons.A == ButtonState.Pressed && newstate.Buttons.A != oldstate.Buttons.A) Exec = true;
        }

        Vector2 shadow_offset = new Vector2(2, 4);

        public void Render(GraphicsDevice gd, SpriteBatch g, SpriteFont fnt, Texture2D background)
        {

            float scale = gd.Viewport.Height / 1080f;

            g.Begin(depthStencilState: DepthStencilState.Default);

            g.Draw(background, color: Color.White * 0.25f, destinationRectangle: gd.Viewport.Bounds, layerDepth: 0.99f);

            int i = 0;

            Vector2 loc = new Vector2(gd.Viewport.Width, gd.Viewport.Height) * Position;

            foreach (MenuItem m in items)
            {
                g.DrawString(fnt, m.Title.ToUpper(), loc + shadow_offset - new Vector2(m.Width / 2 * scale, 0), Color.Black, 
                   0, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);

                g.DrawString(fnt, m.Title.ToUpper(), loc - new Vector2(m.Width / 2 * scale, 0),
                    (i == selection ? (m.Enabled ? Color.Red : Color.DarkRed) : (m.Enabled ? Color.White : Color.Gray)),
                    0, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);

                loc += new Vector2(0, 40 * scale);

                i++;
            }

            loc = Position;

            g.End();
        }
    }
}
