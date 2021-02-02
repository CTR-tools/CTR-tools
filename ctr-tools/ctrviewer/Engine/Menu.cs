using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

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
        public int Value;
        public bool Enabled;
        public float Width;
        public SwitchType sType;

        public int rangeval;
        public int rangemax;

        public MenuItem(string t, string a, string p, bool e, SwitchType st = SwitchType.None, int rmax = 0, int intValue = 0)
        {
            Title = t;
            Action = a;
            Param = p;
            Enabled = e;
            sType = st;
            rangeval = 0;
            rangemax = rmax;
            Value = intValue;
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
            menus.Add("level", new List<MenuItem>()
            {
                new MenuItem("toggle wireframe".ToUpper(), "toggle", "wire", true),
                new MenuItem("toggle filtering".ToUpper(), "toggle", "filter", true),
                new MenuItem("toggle vertex lighting".ToUpper(), "toggle", "vcolor", true),
                new MenuItem("toggle skybox".ToUpper(), "toggle", "sky", true),
                new MenuItem("toggle water".ToUpper(), "toggle", "water", true),
                new MenuItem("toggle invisible".ToUpper(), "toggle", "invis", true),
                new MenuItem("toggle visdata".ToUpper(), "toggle", "visbox", true),
                new MenuItem("visdata leaves only".ToUpper(), "toggle", "visboxleaf", true),
                new MenuItem("toggle game objects".ToUpper(), "toggle", "inst", true),
                new MenuItem("toggle paths".ToUpper(), "toggle", "paths", true),
                new MenuItem("toggle lod".ToUpper(), "toggle", "lod", true),
                new MenuItem("<< quadflag: {0} >>".ToUpper(), "flag", "scroll", true, SwitchType.Range, 15),
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            menus.Add("video", new List<MenuItem>()
            {
                new MenuItem("toggle fullscreen".ToUpper(), "toggle", "window", true),
                new MenuItem("toggle vsync/fps lock".ToUpper(), "toggle", "vsync", true),
                new MenuItem("toggle antialias".ToUpper(), "toggle", "antialias", true),
                new MenuItem("toggle stereoscopic 3D mode".ToUpper(), "toggle", "stereo", true),
                new MenuItem("toggle mipmap generation on load".ToUpper(), "toggle", "genmips", true),
                new MenuItem("show camera position".ToUpper(), "toggle", "campos", true),
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            List<MenuItem> cupmenu = new List<MenuItem>();
            cupmenu.Add(new MenuItem("wumpa cup".ToUpper(), "link", "cup_wumpa", true));
            cupmenu.Add(new MenuItem("crystal cup".ToUpper(), "link", "cup_cryst", true));
            cupmenu.Add(new MenuItem("nitro cup".ToUpper(), "link", "cup_nitro", true));
            cupmenu.Add(new MenuItem("crash cup".ToUpper(), "link", "cup_crash", true));
            cupmenu.Add(new MenuItem("bonus tracks".ToUpper(), "link", "bonus_levels", true));
            cupmenu.Add(new MenuItem("battle arenas".ToUpper(), "link", "battle_arenas", true));
            cupmenu.Add(new MenuItem("adventure".ToUpper(), "link", "adventure", true));
            cupmenu.Add(new MenuItem("cutscenes".ToUpper(), "link", "cutscenes", true));
            cupmenu.Add(new MenuItem("back".ToUpper(), "link", "main", true));
            menus.Add("cupmenu", cupmenu);


            menus.Add("cup_wumpa", new List<MenuItem>()
            {
                new MenuItem("Crash Cove".ToUpper(), "loadbig", "", true, intValue: 3 * 8),
                new MenuItem("Tiger Temple".ToUpper(), "loadbig", "", true, intValue: 4 * 8),
                new MenuItem("Blizzard Bluff".ToUpper(), "loadbig", "", true, intValue: 2 * 8),
                new MenuItem("Coco Park".ToUpper(), "loadbig", "", true, intValue: 14 * 8),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_cryst", new List<MenuItem>()
            {
                new MenuItem("Roo's Tubes".ToUpper(), "loadbig", "", true, intValue: 6 * 8),
                new MenuItem("Dingo Canyon".ToUpper(), "loadbig", "", true, intValue: 0 * 8),
                new MenuItem("Dragon Mines".ToUpper(), "loadbig", "", true, intValue: 1 * 8),
                new MenuItem("Sewer Speedway".ToUpper(), "loadbig", "", true, intValue: 8 * 8),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_nitro", new List<MenuItem>()
            {
                new MenuItem("Mystery Caves".ToUpper(), "loadbig", "", true, intValue: 9 * 8),
                new MenuItem("Papu's Pyramid".ToUpper(), "loadbig", "", true, intValue: 5 * 8),
                new MenuItem("Cortex Castle".ToUpper(), "loadbig", "", true, intValue: 10 * 8),
                new MenuItem("Tiny Arena".ToUpper(), "loadbig", "", true, intValue: 15 * 8),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_crash", new List<MenuItem>()
            {
                new MenuItem("Polar Pass".ToUpper(), "loadbig", "", true, intValue: 12 * 8),
                new MenuItem("N. Gin Labs".ToUpper(), "loadbig", "", true, intValue: 11 * 8),
                new MenuItem("Hot Air Skyway".ToUpper(), "loadbig", "", true, intValue: 7 * 8),
                new MenuItem("Slide Colliseum".ToUpper(), "loadbig", "", true, intValue: 16 * 8),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cutscenes", new List<MenuItem>()
            {
                new MenuItem("Intro Box".ToUpper(), "loadbig", "", true, intValue: 513),
                new MenuItem("Race Today".ToUpper(), "loadbig", "", true, intValue: 515),
                new MenuItem("Canyon Coco".ToUpper(), "loadbig", "", true, intValue: 518),
                new MenuItem("Pass Tiny".ToUpper(), "loadbig", "", true, intValue: 521),
                new MenuItem("Temple Polar".ToUpper(), "loadbig", "", true, intValue: 524),
                new MenuItem("Skyway Dingodile".ToUpper(), "loadbig", "", true, intValue: 527),
                new MenuItem("Sewer Cortex".ToUpper(), "loadbig", "", true, intValue: 530),
                new MenuItem("Oxide 1".ToUpper(), "loadbig", "", true, intValue: 533),
                new MenuItem("Sleeping Crash".ToUpper(), "loadbig", "", true, intValue: 536),
                new MenuItem("Oxide 2".ToUpper(), "loadbig", "", true, intValue: 539),
                new MenuItem("Oxide 3".ToUpper(), "loadbig", "", true, intValue: 542),
                new MenuItem("Oxide 4".ToUpper(), "loadbig", "", true, intValue: 544),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            List<MenuItem> bonus_levels = new List<MenuItem>();
            bonus_levels.Add(new MenuItem("Oxide Station".ToUpper(), "loadbig", "", true, intValue: 13 * 8));
            bonus_levels.Add(new MenuItem("Turbo Track".ToUpper(), "loadbig", "", true, intValue: 17 * 8));
            bonus_levels.Add(new MenuItem("Character selection".ToUpper(), "loadbig", "", true, intValue: 217));
            bonus_levels.Add(new MenuItem("back".ToUpper(), "link", "cupmenu", true));
            menus.Add("bonus_levels", bonus_levels);

            List<MenuItem> battle_arenas = new List<MenuItem>();
            battle_arenas.Add(new MenuItem("Nitro Court".ToUpper(), "loadbig", "", true, intValue: 18 * 8));
            battle_arenas.Add(new MenuItem("Rampage Ruins".ToUpper(), "loadbig", "", true, intValue: 19 * 8));
            battle_arenas.Add(new MenuItem("Parking Lot".ToUpper(), "loadbig", "", true, intValue: 20 * 8));
            battle_arenas.Add(new MenuItem("Skull Rock".ToUpper(), "loadbig", "", true, intValue: 21 * 8));
            battle_arenas.Add(new MenuItem("North Bowl".ToUpper(), "loadbig", "", true, intValue: 22 * 8));
            battle_arenas.Add(new MenuItem("Rocky Road".ToUpper(), "loadbig", "", true, intValue: 23 * 8));
            battle_arenas.Add(new MenuItem("Lab Basement".ToUpper(), "loadbig", "", true, intValue: 24 * 8));
            battle_arenas.Add(new MenuItem("back".ToUpper(), "link", "cupmenu", true));
            menus.Add("battle_arenas", battle_arenas);

            List<MenuItem> adventure = new List<MenuItem>();
            adventure.Add(new MenuItem("Gem Valley".ToUpper(), "loadbig", "", true, intValue: 200));
            adventure.Add(new MenuItem("N. Sanity Beach".ToUpper(), "loadbig", "", true, intValue: 203));
            adventure.Add(new MenuItem("Lost Ruins".ToUpper(), "loadbig", "", true, intValue: 206));
            adventure.Add(new MenuItem("Glacier Park".ToUpper(), "loadbig", "", true, intValue: 209));
            adventure.Add(new MenuItem("Citadel City".ToUpper(), "loadbig", "", true, intValue: 212));
            adventure.Add(new MenuItem("All at once".ToUpper(), "loadbigadv", "", true, intValue: -1));
            adventure.Add(new MenuItem("back".ToUpper(), "link", "cupmenu", true));
            menus.Add("adventure", adventure);

            List<MenuItem> main = new List<MenuItem>();
            main.Add(new MenuItem("resume".ToUpper(), "close", "", true));
            //main.Add(new MenuItem("reload level".ToUpper(), "load", "", true));
            main.Add(new MenuItem("load level".ToUpper(), "link", "cupmenu", File.Exists("bigfile.big")));
            main.Add(new MenuItem("level options".ToUpper(), "link", "level", true));
            main.Add(new MenuItem("video options".ToUpper(), "link", "video", true));
            main.Add(new MenuItem("time of day".ToUpper(), "link", "tod", true));
            main.Add(new MenuItem("quit".ToUpper(), "exit", "", true));

            List<MenuItem> tod = new List<MenuItem>();
            tod.Add(new MenuItem("day".ToUpper(), "tod_day", "", true));
            tod.Add(new MenuItem("evening".ToUpper(), "tod_evening", "", true));
            tod.Add(new MenuItem("night".ToUpper(), "tod_night", "", true));
            tod.Add(new MenuItem("back".ToUpper(), "link", "main", true));
            menus.Add("tod", tod);

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
            while (items[Selection].Action == "" && items[Selection].Enabled);
        }

        public void Previous()
        {
            do
            {
                Selection--;
            }
            while (items[Selection].Action == "" && items[Selection].Enabled);
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
            if ((newstate.Buttons.A == ButtonState.Pressed && newstate.Buttons.A != oldstate.Buttons.A) || IsPressed(Keys.Enter, oldkb, newkb) || IsPressed(Keys.Space, oldkb, newkb))
                if (SelectedItem.Enabled)
                    Exec = true;
        }

        Vector2 shadow_offset = new Vector2(2, 4);

        public void Draw(GraphicsDevice gd, SpriteBatch g, SpriteFont fnt, Texture2D background)
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
