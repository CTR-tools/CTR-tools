using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ctrviewer
{
    public enum SwitchType
    {
        None, Toggle, Range, Set
    }

    public class MenuItem
    {
        public string Title;
        public string Action;
        public string Param;
        public bool Enabled;
        public float Width;
        public SwitchType sType;

        public int rangeval;
        public int rangemax;

        public MenuItem(string t, string a, string p, bool e, SwitchType st = SwitchType.None, int rmax = 0)
        {
            Title = t;
            Action = a;
            Param = p;
            Enabled = e;
            sType = st;
            rangeval = 0;
            rangemax = rmax;
        }

        public void CalcWidth(SpriteFont font)
        {
            Width = font.MeasureString(Title).X;
        }
    }


    class Menu
    {

        public static Dictionary<string, List<MenuItem>> menus = new Dictionary<string, List<MenuItem>>();


        public Vector2 Position = new Vector2(0.5f, 0.35f);
        public bool Exec = false;

        public List<MenuItem> items = new List<MenuItem>();

        public MenuItem SelectedItem
        {
            get { return items[selection]; }
        }

        public int Selection
        {
            get { return selection; }
            set
            {
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

        public void SetMenu(SpriteFont font)
        {
            SetMenu(font, SelectedItem.Param);
        }

        public void SetMenu(SpriteFont font, string name)
        {
            if (!menus.ContainsKey(name))
                throw new Exception("missing menu! " + name);

            items = menus[name];
            selection = 0;

            foreach (MenuItem m in items)
                m.CalcWidth(font);
        }

        public void LoadMenuItems()
        {
            List<MenuItem> level = new List<MenuItem>();
            level.Add(new MenuItem("toggle wireframe".ToUpper(), "toggle", "wire", true));
            level.Add(new MenuItem("toggle filtering".ToUpper(), "toggle", "filter", true));
            level.Add(new MenuItem("toggle invisible".ToUpper(), "toggle", "invis", true));
            level.Add(new MenuItem("toggle game objects".ToUpper(), "toggle", "inst", true));
            level.Add(new MenuItem("toggle paths".ToUpper(), "toggle", "paths", true));
            level.Add(new MenuItem("toggle lod".ToUpper(), "toggle", "lod", true));
            level.Add(new MenuItem("<< quadflag: {0} >>".ToUpper(), "flag", "scroll", true, SwitchType.Range, 15));
            level.Add(new MenuItem("back".ToUpper(), "link", "main", true));
            menus.Add("level", level);

            List<MenuItem> video = new List<MenuItem>();
            video.Add(new MenuItem("toggle fullscreen".ToUpper(), "toggle", "window", true));
            video.Add(new MenuItem("toggle vsync/fps lock".ToUpper(), "toggle", "lockfps", true));
            video.Add(new MenuItem("toggle antialias".ToUpper(), "toggle", "antialias", true));
            video.Add(new MenuItem("back".ToUpper(), "link", "main", true));
            menus.Add("video", video);

            List<MenuItem> main = new List<MenuItem>();
            main.Add(new MenuItem("resume".ToUpper(), "close", "", true));
            main.Add(new MenuItem("reload level".ToUpper(), "load", "", true));
            main.Add(new MenuItem("level options".ToUpper(), "link", "level", true));
            main.Add(new MenuItem("video options".ToUpper(), "link", "video", true));
            main.Add(new MenuItem("quit".ToUpper(), "exit", "", true));

            menus.Add("main", main);

            items = main;

            //Selection = 0;
        }

        public void Next()
        {
            do
            {
                Selection++;
            }
            while (items[Selection].Action == "");
        }

        public void Previous()
        {
            do
            {
                Selection--;
            }
            while (items[Selection].Action == "");
        }


        public bool IsPressed(Keys key, KeyboardState oldkb, KeyboardState newkb)
        {
            return newkb.IsKeyDown(key) && oldkb.IsKeyDown(key) != newkb.IsKeyDown(key);
        }

        public void Update(GamePadState oldstate, GamePadState newstate, KeyboardState oldkb, KeyboardState newkb)
        {
            if ((newstate.DPad.Up == ButtonState.Pressed && newstate.DPad.Up != oldstate.DPad.Up) || IsPressed(Keys.W, oldkb, newkb) || IsPressed(Keys.Up, oldkb, newkb)) Previous();
            if ((newstate.DPad.Down == ButtonState.Pressed && newstate.DPad.Down != oldstate.DPad.Down) || IsPressed(Keys.S, oldkb, newkb) || IsPressed(Keys.Down, oldkb, newkb)) Next();

            if (newstate.DPad.Left == ButtonState.Pressed && newstate.DPad.Left != oldstate.DPad.Left || IsPressed(Keys.A, oldkb, newkb) || IsPressed(Keys.Left, oldkb, newkb))
            {
                if (SelectedItem.sType == SwitchType.Range)
                {
                    SelectedItem.rangeval--;
                    if (SelectedItem.rangeval < 0)
                        SelectedItem.rangeval = SelectedItem.rangemax;

                    Game1.currentflag = SelectedItem.rangeval;
                }
            }
            if ((newstate.DPad.Right == ButtonState.Pressed && newstate.DPad.Right != oldstate.DPad.Right) || IsPressed(Keys.D, oldkb, newkb) || IsPressed(Keys.Right, oldkb, newkb))
            {
                if (SelectedItem.sType == SwitchType.Range)
                {
                    SelectedItem.rangeval++;
                    if (SelectedItem.rangeval > SelectedItem.rangemax)
                        SelectedItem.rangeval = 0;

                    Game1.currentflag = SelectedItem.rangeval;
                }
            }
            if ((newstate.Buttons.A == ButtonState.Pressed && newstate.Buttons.A != oldstate.Buttons.A) || IsPressed(Keys.Enter, oldkb, newkb) || IsPressed(Keys.Space, oldkb, newkb)) Exec = true;
        }

        Vector2 shadow_offset = new Vector2(2, 4);

        public void Render(GraphicsDevice gd, SpriteBatch g, SpriteFont fnt, Texture2D background)
        {

            float scale = gd.Viewport.Height / 1080f;

            g.Begin(depthStencilState: DepthStencilState.None);

            g.Draw(background, gd.Viewport.Bounds, Color.White * 0.25f);

            int i = 0;

            Vector2 loc = new Vector2(gd.Viewport.Width, gd.Viewport.Height) * Position;

            foreach (MenuItem m in items)
            {
                string s = (m.sType == SwitchType.Range ? String.Format(m.Title, ((QuadFlags)(1 << Game1.currentflag)).ToString(), m.rangeval) : m.Title.ToUpper()); //m.Title.ToUpper(), 

                g.DrawString(fnt, s, loc + shadow_offset - new Vector2(m.Width / 2 * scale, 0), Color.Black,
                   0, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);

                g.DrawString(fnt, s, loc - new Vector2(m.Width / 2 * scale, 0),
                    (i == selection ? (m.Enabled ? Color.Red : Color.DarkRed) : (m.Enabled ? Color.White : Color.Gray)),
                   0, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);

                loc += new Vector2(0, 40 * scale);

                i++;
            }

            //loc = Position;

            g.End();
        }
    }
}
