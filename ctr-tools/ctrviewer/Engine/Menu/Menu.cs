using CTRFramework;
using ctrviewer.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Gui
{
    public class Menu
    {
        public static Dictionary<string, List<MenuItem>> menus = new Dictionary<string, List<MenuItem>>();

        public static SpriteFont Font = null;

        //position in percentage where 0.5 is half the screen
        public Vector2 Position = new Vector2(0.5f, 0.35f);
        public bool Exec = false;

        public List<MenuItem> items = new List<MenuItem>();

        public bool Visible = false;

        public MenuItem SelectedItem => items[_selectedIndex];

        private int _selectedIndex;

        public int Selection
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                if (value >= items.Count) _selectedIndex = 0;
                if (value < 0) _selectedIndex = items.Count - 1;
            }
        }

        public MenuItem Find(string name)
        {
            foreach (var items in menus.Values)
                foreach (var item in items)
                    if (item.Name == name)
                        return item;

            //not found
            return null;
        }

        public Menu(SpriteFont font)
        {
            _selectedIndex = 0;

            LoadMenuItems();

            Font = font;

            CalcWidth();
        }

        public void SetMenu(SpriteFont font)
        {
            SetMenu(font, SelectedItem.Param);
        }

        public void SetMenu(SpriteFont font, string name)
        {
            Font = font;

            if (!menus.ContainsKey(name))
                throw new Exception("missing menu! " + name);

            items = menus[name];
            _selectedIndex = 0;

            CalcWidth();
        }

        private void CalcWidth()
        {
            foreach (var item in items)
                item.CalcWidth();
        }

        public void LoadMenuItems()
        {
            var settings = EngineSettings.Instance;

            #region [menu items]

            var flagtitles = new List<(int, string)>() { (-1, "None")};

            foreach (int i in Enum.GetValues(typeof(QuadFlags)))
            {
                if (i == 0 || i == -1) continue;
                flagtitles.Add((i, ((QuadFlags)i).ToString()));
            }

            menus.Add("level", new List<MenuItem>()
            {
                new BoolMenuItem() { Text = "Wireframe", Name = "wire", Value = settings.DrawWireframe },
                new BoolMenuItem() { Text = "Texture Replacements", Name = "newtex", Value = settings.UseTextureReplacements },
                new BoolMenuItem() { Text = "Vertex Lighting", Name = "vcolor", Value = settings.VertexLighting },
                new BoolMenuItem() { Text = "Backface Culling", Name = "nocull", Value = settings.BackFaceCulling },
                new BoolMenuItem() { Text = "Sky Box", Name = "skybox", Value = settings.ShowSky },
                new BoolMenuItem() { Text = "Water", Name = "water", Value = settings.ShowWater },
                new BoolMenuItem() { Text = "Invisible Meshes", Name = "invis", Value = settings.ShowInvisible },
                new BoolMenuItem() { Text = "Visibility Tree", Name = "visbox", Value = settings.VisData },
                new BoolMenuItem() { Text = "Render BSP Branches", Name = "visboxleaf", Value = settings.VisDataLeaves, Enabled = settings.VisData },
                new BoolMenuItem() { Text = "Game Objects", Name = "inst", Value = settings.ShowModels },
                new BoolMenuItem() { Text = "Bot Paths", Name = "paths", Value = settings.ShowBotPaths },
                new MenuItem("Toggle lod".ToUpper(), "toggle", "lod", true),
                new IntRangeMenuItem() { Text = "QuadFlag", Name = "flag", SelectedValue = 0, Values = flagtitles
                },
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            menus.Add("video", new List<MenuItem>()
            {
                new BoolMenuItem() { Text = "Windowed", Name = "window", Value = settings.Windowed },
                new BoolMenuItem() { Text = "vsync/fps lock", Name = "vsync", Value = settings.VerticalSync },
                new BoolMenuItem() { Text = "Antialias", Name = "antialias", Value = settings.AntiAlias },
                new BoolMenuItem() { Text = "Texture filtering", Name = "filter", Value = settings.EnableFiltering },
                new IntRangeMenuItem() { Text = "Anisotropy", Name = "aniso", Enabled = settings.EnableFiltering, SelectedValue = settings.AnisotropyLevel, Values = new List<(int, string)>() { (1, "1x"), (2, "2x"), (4, "4x"), (8, "8x"), (16, "16x") } },
                new BoolMenuItem() { Text = "Internal PSX Resolution", Name = "intpsx", Value = settings.InternalPSXResolution },
                new BoolMenuItem() { Text = "Stereoscopic mode", Name = "stereo", Value = settings.StereoPair },
                new BoolMenuItem() { Text = "crosseyed", Name = "crosseyed", Value = settings.StereoCrossEyed, Enabled = settings.StereoPair },
                new BoolMenuItem() { Text = "Generate mip maps", Name = "genmips", Value = settings.GenerateMips },
                new BoolMenuItem() { Text = "Show camera position", Name = "campos", Value = settings.ShowCamPos },
                new BoolMenuItem() { Text = "Show console", Name = "console", Value = settings.ShowConsole },
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            menus.Add("cupmenu", new List<MenuItem>() {
                new MenuItem("level type".ToUpper(), "link", "level_type", true),
                new MenuItem("wumpa cup".ToUpper(), "link", "cup_wumpa", true),
                new MenuItem("crystal cup".ToUpper(), "link", "cup_cryst", true),
                new MenuItem("nitro cup".ToUpper(), "link", "cup_nitro", true),
                new MenuItem("crash cup".ToUpper(), "link", "cup_crash", true),
                new MenuItem("bonus tracks".ToUpper(), "link", "bonus_levels", true),
                new MenuItem("battle arenas".ToUpper(), "link", "battle_arenas", true),
                new MenuItem("adventure".ToUpper(), "link", "adventure", true),
                new MenuItem("cutscenes".ToUpper(), "link", "cutscenes", true),
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            menus.Add("level_type", new List<MenuItem>()
            {
                new IntMenuItem((int)LevelType.Lod1P) { Text = "1 player", Name = LevelType.Lod1P.ToString() },
                new IntMenuItem((int)LevelType.Lod2P) { Text = "2 player", Name = LevelType.Lod2P.ToString() },
                new IntMenuItem((int)LevelType.Lod4P) { Text = "4 player", Name = LevelType.Lod4P.ToString() },
                new IntMenuItem((int)LevelType.LodRelic) { Text = "relic race", Name = LevelType.LodRelic.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_wumpa", new List<MenuItem>()
            {
                new IntMenuItem((int)Level.CrashCove * 8) { Text = "Crash Cove", Name = Level.CrashCove.ToString() },
                new IntMenuItem((int)Level.TigerTemple * 8) { Text = "Tiger Temple", Name = Level.TigerTemple.ToString() },
                new IntMenuItem((int)Level.BlizzardBluff * 8) { Text = "Blizzard Bluff", Name = Level.BlizzardBluff.ToString() },
                new IntMenuItem((int)Level.CocoPark * 8) { Text = "Coco Park", Name = Level.CocoPark.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_cryst", new List<MenuItem>()
            {
                new IntMenuItem((int)Level.RooTubes * 8) { Text = "Roo's Tubes", Name = Level.RooTubes.ToString() },
                new IntMenuItem((int)Level.DingoCanyon * 8) { Text = "Dingo Canyon", Name = Level.DingoCanyon.ToString()  },
                new IntMenuItem((int)Level.DragonMines * 8) { Text = "Dragon Mines", Name = Level.DragonMines.ToString() },
                new IntMenuItem((int)Level.SewerSpeedway * 8) { Text = "Sewer Speedway", Name = Level.SewerSpeedway.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_nitro", new List<MenuItem>()
            {
                new IntMenuItem((int)Level.MysteryCaves * 8) { Text = "Mystery Caves", Name = Level.MysteryCaves.ToString() },
                new IntMenuItem((int)Level.PapuPyramid * 8) { Text = "Papu's Pyramid", Name = Level.PapuPyramid.ToString()  },
                new IntMenuItem((int)Level.CortexCastle * 8) { Text = "Cortex Castle", Name = Level.CortexCastle.ToString() },
                new IntMenuItem((int)Level.TinyArena * 8) { Text = "Tiny Arena", Name = Level.TinyArena.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cup_crash", new List<MenuItem>()
            {
                new IntMenuItem((int)Level.PolarPass * 8) { Text = "Polar Pass", Name = Level.PolarPass.ToString() },
                new IntMenuItem((int)Level.NGinLabs * 8) { Text = "N. Gin Labs", Name = Level.NGinLabs.ToString()  },
                new IntMenuItem((int)Level.HotAirSkyway * 8) { Text = "Hot Air Skyway", Name = Level.HotAirSkyway.ToString() },
                new IntMenuItem((int)Level.SlideColiseum * 8) { Text = "Slide Coliseum", Name = Level.SlideColiseum.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("cutscenes", new List<MenuItem>()
            {
                new IntMenuItem(513) { Text = "Intro Box (ND)", Name = Cutscenes.IntroBox.ToString() },
                new IntMenuItem(515) { Text = "Race Today", Name = Cutscenes.RaceToday.ToString() },
                new IntMenuItem(518) { Text = "Canyon/Coco", Name = Cutscenes.CanyonCoco.ToString() },
                new IntMenuItem(521) { Text = "Pass/Tiny", Name = Cutscenes.PassTiny.ToString() },
                new IntMenuItem(524) { Text = "Temple/Polar", Name = Cutscenes.TemplePolar.ToString() },
                new IntMenuItem(527) { Text = "Skyway/Dingodile", Name = Cutscenes.SkywayDingodile.ToString() },
                new IntMenuItem(530) { Text = "Sewer/Cortex", Name = Cutscenes.SewerCortex.ToString() },
                new IntMenuItem(533) { Text = "Oxide 1", Name = Cutscenes.Oxide1.ToString() },
                new IntMenuItem(536) { Text = "Sleeping Crash", Name = Cutscenes.SleepingCrash.ToString() },
                new IntMenuItem(539) { Text = "Oxide 2", Name = Cutscenes.Oxide2.ToString() },
                new IntMenuItem(542) { Text = "Oxide 3", Name = Cutscenes.Oxide3.ToString() },
                new IntMenuItem(544) { Text = "Oxide 4", Name = Cutscenes.Oxide4.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            }); ;

            menus.Add("bonus_levels", new List<MenuItem>
            {
                new IntMenuItem((int)Level.OxideStation * 8) { Text = "Oxide Station", Name = Level.OxideStation.ToString() },
                new IntMenuItem((int)Level.TurboTrack * 8) { Text = "Turbo Track", Name = Level.TurboTrack.ToString()  },
                new IntMenuItem(217) { Text = "Character Selection", Name = "charselect" },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
             });

            menus.Add("battle_arenas", new List<MenuItem>
            {
                new IntMenuItem((int)Level.NitroCourt * 8) { Text = "Nitro Court", Name = Level.NitroCourt.ToString() },
                new IntMenuItem((int)Level.RampageRuins * 8) { Text = "Rampage Ruins", Name = Level.RampageRuins.ToString() },
                new IntMenuItem((int)Level.ParkingLot * 8) { Text = "Parking Lot", Name = Level.ParkingLot.ToString() },
                new IntMenuItem((int)Level.SkullRock * 8) { Text = "Skull Rock", Name = Level.SkullRock.ToString() },
                new IntMenuItem((int)Level.NorthBowl * 8) { Text = "North Bowl", Name = Level.NorthBowl.ToString() },
                new IntMenuItem((int)Level.RockyRoad * 8) { Text = "Rocky Road", Name = Level.RockyRoad.ToString() },
                new IntMenuItem((int)Level.LabBasement * 8) { Text = "Lab Basement", Name = Level.LabBasement.ToString() },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("adventure", new List<MenuItem>
            {
                new IntMenuItem(-1) { Text = "All Hubs At Once", Name = "allhubs" },
                new IntMenuItem(200) { Text = "Gem Valley", Name = "gemvalley" },
                new IntMenuItem(203) { Text = "N. Sanity Beach", Name = "nsanity" },
                new IntMenuItem(206) { Text = "Lost Ruins", Name = "lostruins" },
                new IntMenuItem(209) { Text = "Glacier Park", Name = "glacierpark" },
                new IntMenuItem(212) { Text = "Citadel City", Name = "citadelcity" },
                new MenuItem("back".ToUpper(), "link", "cupmenu", true)
            });

            menus.Add("main", new List<MenuItem>() {
                new MenuItem("resume".ToUpper(), "close", "", true),
                //new MenuItem("reload level".ToUpper(), "load", "", true),
                new MenuItem("load level".ToUpper(), "link", "cupmenu", Game1.BigFileExists),
                new MenuItem("level options".ToUpper(), "link", "level", true),
                new MenuItem("video options".ToUpper(), "link", "video", true),
                new MenuItem("time of day".ToUpper(), "link", "tod", true),
                new BoolMenuItem() { Text = "Kart mode", Name = "kart", Value = settings.KartMode },
                new MenuItem("quit".ToUpper(), "exit", "", true),
            });

            menus.Add("tod", new List<MenuItem>() {
                new MenuItem("day".ToUpper(), "tod_day", "", true),
                new MenuItem("evening".ToUpper(), "tod_evening", "", true),
                new MenuItem("night".ToUpper(), "tod_night", "", true),
                new MenuItem("back".ToUpper(), "link", "main", true)
            });

            #endregion

            items = menus["main"];

            //Selection = 0;
        }

        private void Next()
        {
            do Selection++;
            while (items[Selection].Action == "" && items[Selection].Enabled);

            //ContentVault.Sounds["menu_down"].Play(0.15f, 0, 0);
        }

        private void Previous()
        {
            do Selection--;
            while (items[Selection].Action == "" && items[Selection].Enabled);

            //ContentVault.Sounds["menu_up"].Play(0.15f, 0, 0);
        }

        public void Update()
        {
            if (!Visible) return;

            if (InputHandlers.Process(GameAction.MenuUp)) Previous();
            if (InputHandlers.Process(GameAction.MenuDown)) Next();

            if (SelectedItem.Enabled)
            {
                if (InputHandlers.Process(GameAction.MenuLeft))
                {
                    if (SelectedItem is IntRangeMenuItem)
                        SelectedItem.PressLeft();
                }
                if (InputHandlers.Process(GameAction.MenuRight))
                {
                    if (SelectedItem is IntRangeMenuItem)
                        SelectedItem.PressRight();
                }
            }

            //do not allow to enter menus if alt is pressed cause of fullscreen toggle
            if (!KeyboardHandler.IsAltPressed && InputHandlers.Process(GameAction.MenuConfirm))
                if (SelectedItem.Enabled)
                {
                    SelectedItem.DoClick();
                    Exec = true;
                }
        }

        Vector2 shadow_offset = new Vector2(2, 4);

        Color MenuItemBackColor = new Color(0, 0, 0, 128);
        Color MenuItemSelectedColor = new Color(128, 0, 0, 128);

        public void Draw(GraphicsDevice graphics, SpriteBatch batch, SpriteFont fnt, Texture2D background)
        {
            if (!Visible) return;

            graphics.BlendState = BlendState.Opaque;

            float scale = graphics.Viewport.Height / 1080f;

            batch.Draw(background, graphics.Viewport.Bounds, Color.Black * 0.25f);

            int i = 0;

            Vector2 loc = new Vector2(graphics.Viewport.Width, graphics.Viewport.Height) * Position;

            float maxwidth = 0;

            foreach (var m in items)
                if (m.Width > maxwidth)
                    maxwidth = m.Width;

            maxwidth *= 1.25f;

            if (maxwidth < graphics.Viewport.Width / 3)
                maxwidth = graphics.Viewport.Width / 3;

            foreach (MenuItem m in items)
            {
                string s = m.ToString();

                Vector2 backloc = loc - new Vector2(maxwidth / 2 * scale, 0);

                //draw menu item background
                batch.Draw(background, new Rectangle((int)backloc.X, (int)backloc.Y - 2, (int)(maxwidth * scale), (int)(40 * scale)),
                    i == Selection ? MenuItemSelectedColor : MenuItemBackColor);

                //draw menu item text shadow
                batch.DrawString(fnt, s, loc + shadow_offset - new Vector2(m.Width / 2 * scale, 0), Color.Black,
                   0, new Vector2(0, 0), scale, SpriteEffects.None, 1.0f);

                //draw menu item text
                batch.DrawString(fnt, s, loc - new Vector2(m.Width / 2 * scale, 0),
                   m.Enabled ? (m == SelectedItem ? Color.White : Game1.CtrMainFontColor) : Color.DarkGray,// (i == selection ? (m.Enabled ? Color.Red : Color.DarkRed) : (m.Enabled ? Color.White : Color.Gray)),
                   0, new Vector2(0, 0), scale, SpriteEffects.None, 0.5f);

                //next line
                loc.Y += 40 * scale;

                i++;
            }

            //draw logo
            batch.Draw(
                ContentVault.Textures["logo"],
                new Vector2((graphics.Viewport.Width / 2), 50 * graphics.Viewport.Height / 1080f),
                new Rectangle(0, 0, ContentVault.Textures["logo"].Width, ContentVault.Textures["logo"].Height),
                Color.White,
                0,
                new Vector2(ContentVault.Textures["logo"].Width / 2, 0),
                graphics.Viewport.Height / 1080f,
                SpriteEffects.None,
                0.5f
                );

            //draw framework version
            batch.DrawString(
                fnt,
                Game1.version,
                new Vector2(((graphics.Viewport.Width - fnt.MeasureString(Game1.version).X * graphics.Viewport.Height / 1080f) / 2), graphics.Viewport.Height - 60 * graphics.Viewport.Width / 1080f),
                Color.Aquamarine,
                0,
                new Vector2(0, 0),
                graphics.Viewport.Height / 1080f,
                SpriteEffects.None,
                 0.5f
                );
        }
    }
}
