using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    public enum SwitchType
    {
        None, Toggle, Range, Set
    }

    class Menu
    {
        public static Dictionary<string, List<MenuItem>> menus = new Dictionary<string, List<MenuItem>>();


        public Vector2 Position = new Vector2(0.5f, 0.35f);
        public bool Exec = false;

        public List<MenuItem> items = new List<MenuItem>();

        public bool Visible = false;

        public MenuItem SelectedItem
        {
            get => items[selection];
        }

        public int Selection
        {
            get => selection;
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

            #region menuitems
            menus.Add("level", new List<MenuItem>()
            {
                new MenuItem("toggle wireframe".ToUpper(), "toggle", "wire", true),
                new MenuItem("toggle newtex".ToUpper(), "toggle", "newtex", true),
                new MenuItem("toggle vertex lighting".ToUpper(), "toggle", "vcolor", true),
                new MenuItem("toggle double sided".ToUpper(), "toggle", "nocull", true),
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
                new MenuItem("toggle filtering".ToUpper(), "toggle", "filter", true),
                new MenuItem("internal psx resolution".ToUpper(), "toggle", "psxres", true),
                new MenuItem("toggle stereoscopic 3D mode".ToUpper(), "toggle", "stereo", true),
                new MenuItem("toggle mipmap generation on load".ToUpper(), "toggle", "genmips", true),
                new MenuItem("show camera position".ToUpper(), "toggle", "campos", true),
                new MenuItem("show console".ToUpper(), "toggle", "console", true),
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            List<MenuItem> cupmenu = new List<MenuItem>();
            cupmenu.Add(new MenuItem("level type".ToUpper(), "link", "level_type", true));
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

            menus.Add("level_type", new List<MenuItem>()
            {
                new MenuItem("1 player".ToUpper(), "setlod1", "", true, intValue: 0),
                new MenuItem("2 players".ToUpper(), "setlod2", "", true, intValue: 1),
                new MenuItem("4 players".ToUpper(), "setlod4", "", true, intValue: 2),
                new MenuItem("relic race".ToUpper(), "setlodtt", "", true, intValue: 3),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

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
                new MenuItem("Slide Coliseum".ToUpper(), "loadbig", "", true, intValue: 16 * 8),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cutscenes", new List<MenuItem>()
            {
                new MenuItem("Intro Box (ND)".ToUpper(), "loadbig", "", true, intValue: 513),
                new MenuItem("Race Today".ToUpper(), "loadbig", "", true, intValue: 515),
                new MenuItem("Canyon/Coco".ToUpper(), "loadbig", "", true, intValue: 518),
                new MenuItem("Pass/Tiny".ToUpper(), "loadbig", "", true, intValue: 521),
                new MenuItem("Temple/Polar".ToUpper(), "loadbig", "", true, intValue: 524),
                new MenuItem("Skyway/Dingodile".ToUpper(), "loadbig", "", true, intValue: 527),
                new MenuItem("Sewer/Cortex".ToUpper(), "loadbig", "", true, intValue: 530),
                new MenuItem("Oxide 1".ToUpper(), "loadbig", "", true, intValue: 533),
                new MenuItem("Sleeping Crash".ToUpper(), "loadbig", "", true, intValue: 536),
                new MenuItem("Oxide 2".ToUpper(), "loadbig", "", true, intValue: 539),
                new MenuItem("Oxide 3".ToUpper(), "loadbig", "", true, intValue: 542),
                new MenuItem("Oxide 4".ToUpper(), "loadbig", "", true, intValue: 544),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("bonus_levels", new List<MenuItem>
            {
                new MenuItem("Oxide Station".ToUpper(), "loadbig", "", true, intValue: 13 * 8),
                new MenuItem("Turbo Track".ToUpper(), "loadbig", "", true, intValue: 17 * 8),
                new MenuItem("Character selection".ToUpper(), "loadbig", "", true, intValue: 217),
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
             });

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
            adventure.Add(new MenuItem("All at once".ToUpper(), "loadbigadv", "", true, intValue: -1));
            adventure.Add(new MenuItem("Gem Valley".ToUpper(), "loadbig", "", true, intValue: 200));
            adventure.Add(new MenuItem("N. Sanity Beach".ToUpper(), "loadbig", "", true, intValue: 203));
            adventure.Add(new MenuItem("Lost Ruins".ToUpper(), "loadbig", "", true, intValue: 206));
            adventure.Add(new MenuItem("Glacier Park".ToUpper(), "loadbig", "", true, intValue: 209));
            adventure.Add(new MenuItem("Citadel City".ToUpper(), "loadbig", "", true, intValue: 212));
            adventure.Add(new MenuItem("back".ToUpper(), "link", "cupmenu", true));
            menus.Add("adventure", adventure);

            List<MenuItem> main = new List<MenuItem>();
            main.Add(new MenuItem("resume".ToUpper(), "close", "", true));
            //main.Add(new MenuItem("reload level".ToUpper(), "load", "", true));
            main.Add(new MenuItem("load level".ToUpper(), "link", "cupmenu", Game1.BigFileExists)); // File.Exists("bigfile.big")));
            main.Add(new MenuItem("level options".ToUpper(), "link", "level", true));
            main.Add(new MenuItem("video options".ToUpper(), "link", "video", true));
            main.Add(new MenuItem("time of day".ToUpper(), "link", "tod", true));
            main.Add(new MenuItem("kart mode".ToUpper(), "toggle", "kart", true));
            main.Add(new MenuItem("quit".ToUpper(), "exit", "", true));

            menus.Add("tod", new List<MenuItem>() {
                new MenuItem("day".ToUpper(), "tod_day", "", true),
                new MenuItem("evening".ToUpper(), "tod_evening", "", true),
                new MenuItem("night".ToUpper(), "tod_night", "", true),
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

#endregion

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

            //ContentVault.Sounds["menu_down"].Play(0.15f, 0, 0);
        }

        public void Previous()
        {
            do
            {
                Selection--;
            }
            while (items[Selection].Action == "" && items[Selection].Enabled);

            //ContentVault.Sounds["menu_up"].Play(0.15f, 0, 0);
        }

        public void Update(GamePadState oldstate, GamePadState newstate)
        {
            if (!Visible)
                return;

            if ((newstate.DPad.Up == ButtonState.Pressed && newstate.DPad.Up != oldstate.DPad.Up) || KeyboardHandler.IsAnyPressed(Keys.W, Keys.Up)) Previous();
            if ((newstate.DPad.Down == ButtonState.Pressed && newstate.DPad.Down != oldstate.DPad.Down) || KeyboardHandler.IsAnyPressed(Keys.S, Keys.Down)) Next();

            if (newstate.DPad.Left == ButtonState.Pressed && newstate.DPad.Left != oldstate.DPad.Left || KeyboardHandler.IsAnyPressed(Keys.A, Keys.Left))
            {
                if (SelectedItem.sType == SwitchType.Range)
                {
                    SelectedItem.rangeval--;
                    if (SelectedItem.rangeval < 0)
                        SelectedItem.rangeval = SelectedItem.rangemax;

                    Game1.currentflag = SelectedItem.rangeval;
                }
            }
            if ((newstate.DPad.Right == ButtonState.Pressed && newstate.DPad.Right != oldstate.DPad.Right) || KeyboardHandler.IsAnyPressed(Keys.D, Keys.Right))
            {
                if (SelectedItem.sType == SwitchType.Range)
                {
                    SelectedItem.rangeval++;
                    if (SelectedItem.rangeval > SelectedItem.rangemax)
                        SelectedItem.rangeval = 0;

                    Game1.currentflag = SelectedItem.rangeval;
                }
            }
            if ((newstate.Buttons.A == ButtonState.Pressed && newstate.Buttons.A != oldstate.Buttons.A) || KeyboardHandler.IsAnyPressed(Keys.Enter, Keys.Space))
                if (SelectedItem.Enabled)
                    Exec = true;
        }

        Vector2 shadow_offset = new Vector2(2, 4);

        public void Draw(GraphicsDevice gd, SpriteBatch g, SpriteFont fnt, Texture2D background)
        {
            if (!Visible) return;

            float scale = gd.Viewport.Height / 1080f;

            //g.Begin(depthStencilState: DepthStencilState.None);

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

            //draw logo
            g.Draw(
                ContentVault.Textures["logo"],
                new Vector2((gd.Viewport.Width / 2), 50 * gd.Viewport.Height / 1080f),
                new Rectangle(0, 0, ContentVault.Textures["logo"].Width, ContentVault.Textures["logo"].Height),
                Color.White,
                0,
                new Vector2(ContentVault.Textures["logo"].Width / 2, 0),
                gd.Viewport.Height / 1080f,
                SpriteEffects.None,
                0.5f
                );

            //draw framework version
            g.DrawString(
                fnt,
                Game1.version,
                new Vector2(((gd.Viewport.Width - fnt.MeasureString(Game1.version).X * gd.Viewport.Height / 1080f) / 2), gd.Viewport.Height - 60 * gd.Viewport.Width / 1080f),
                Color.Aquamarine,
                0,
                new Vector2(0, 0),
                gd.Viewport.Height / 1080f,
                SpriteEffects.None,
                 0.5f
                );
        }
    }
}
