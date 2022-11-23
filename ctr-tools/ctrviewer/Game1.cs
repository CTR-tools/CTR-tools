using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Shared;
using CTRFramework.Sound;
using CTRFramework.Vram;
using ctrviewer.Engine;
using ctrviewer.Engine.Gui;
using ctrviewer.Engine.Input;
using ctrviewer.Engine.Render;
using ctrviewer.Engine.Testing;
using ctrviewer.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ctrviewer
{
    public enum LevelType
    {
        Lod1P = 0,
        Lod2P = 1,
        Lod4P = 2,
        LodRelic = 3
    }

    public enum PreferredTimeOfDay
    {
        Day,
        Evening,
        Night
    }

    public partial class Game1 : Game
    {
        MainEngine eng;

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        //ctr scenes
        List<CtrScene> Scenes = new List<CtrScene>();

        BigFileReader big;
        Howl howl;

        Menu menu;
        MenuRootComponent newmenu = new MenuRootComponent();

        //effects
        BasicEffect effect;                 //used for static level mesh
        BasicEffect instanceEffect;         //used for instanced mesh
        AlphaTestEffect alphaTestEffect;    //used for alpha textures pass

        public static Vector3 TimeOfDay = new Vector3(2f);
        List<Kart> karts = new List<Kart>();


        //meh
        public static int currentflag = -1;

        //get version only once, because we don't want this to be allocated every frame.
        public static string version = Meta.GetVersion();

        public static bool BigFileExists = false;

        public LevelType levelType = LevelType.Lod1P;

        public void SwitchDisplayMode() => SwitchDisplayMode(graphics);

        public void SetResolution(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            if (eng.Settings.Windowed)
            {
                graphics.PreferredBackBufferWidth = graphics.PreferredBackBufferWidth * eng.Settings.WindowScale / 100;
                graphics.PreferredBackBufferHeight = graphics.PreferredBackBufferHeight * eng.Settings.WindowScale / 100;
            }

        }

        public void SwitchDisplayMode(GraphicsDeviceManager graphics)
        {
            SetResolution(graphics);

            UpdateInternalResolution();

            UpdateSplitscreenViewports(graphics);

            graphics.IsFullScreen = !eng.Settings.Windowed;
            graphics.ApplyChanges();

            if (menu != null)
                (menu.Find("window") as BoolMenuItem).Value = eng.Settings.Windowed;

            GameConsole.Write($"SwitchDisplayMode(): {graphics.PreferredBackBufferWidth}x{graphics.PreferredBackBufferHeight}");
        }

        public Viewport vpFull;
        public Viewport vpLeft;
        public Viewport vpRight;
        public Viewport vpTop;
        public Viewport vpBottom;

        /// <summary>
        /// Creates viewport objects. Full is for full screen, rest is for half screen split vertically or horizontally.
        /// </summary>
        public void UpdateSplitscreenViewports(GraphicsDeviceManager graphics, RenderTarget2D renderTarget = null)
        {
            GameConsole.Write("UpdateSplitscreenViewports()");

            int width = renderTarget != null ? renderTarget.Width : graphics.PreferredBackBufferWidth;
            int height = renderTarget != null ? renderTarget.Height : graphics.PreferredBackBufferHeight;

            float minDepth = graphics.GraphicsDevice.Viewport.MinDepth;
            float maxDepth = graphics.GraphicsDevice.Viewport.MaxDepth;

            vpFull = new Viewport()
            {
                Bounds = new Rectangle(0, 0, width, height),
                MaxDepth = maxDepth,
                MinDepth = minDepth
            };

            vpLeft = new Viewport()
            {
                Bounds = new Rectangle(0, 0, width / 2, height),
                MaxDepth = maxDepth,
                MinDepth = minDepth
            };

            vpRight = new Viewport()
            {
                Bounds = new Rectangle(width / 2, 0, width / 2, height),
                MaxDepth = maxDepth,
                MinDepth = minDepth
            };

            vpTop = new Viewport()
            {
                Bounds = new Rectangle(0, 0, width, height / 2),
                MaxDepth = maxDepth,
                MinDepth = minDepth
            };

            vpBottom = new Viewport()
            {
                Bounds = new Rectangle(0, height / 2, width, height / 2),
                MaxDepth = maxDepth,
                MinDepth = minDepth
            };
        }

        /// <summary>
        /// Updates effect values based on settings.
        /// </summary>
        public void UpdateEffects()
        {
            if (effect is null)
                effect = new BasicEffect(graphics.GraphicsDevice);

            effect.VertexColorEnabled = eng.Settings.VertexLighting;
            effect.TextureEnabled = true;
            effect.DiffuseColor = eng.Settings.VertexLighting ? TimeOfDay : new Vector3(1f);
            effect.FogEnabled = true;
            effect.FogColor = eng.BackgroundColor.ToVector3();
            effect.FogStart = eng.Cameras[CameraType.DefaultCamera].FarClip / 4 * 3;
            effect.FogEnd = eng.Cameras[CameraType.DefaultCamera].FarClip;

            if (alphaTestEffect is null)
                alphaTestEffect = new AlphaTestEffect(GraphicsDevice);

            alphaTestEffect.AlphaFunction = CompareFunction.Greater;
            alphaTestEffect.ReferenceAlpha = 0;
            alphaTestEffect.VertexColorEnabled = eng.Settings.VertexLighting;
            alphaTestEffect.DiffuseColor = effect.DiffuseColor;

            if (instanceEffect is null)
                instanceEffect = new BasicEffect(graphics.GraphicsDevice);

            instanceEffect.VertexColorEnabled = true;
            instanceEffect.TextureEnabled = true;
            instanceEffect.DiffuseColor = effect.DiffuseColor;
        }

        public void UpdateVSync()
        {
            graphics.SynchronizeWithVerticalRetrace = eng.Settings.VerticalSync;
            IsFixedTimeStep = eng.Settings.VerticalSync;

            graphics.ApplyChanges();
        }


        Vector3 todNight = new Vector3(0.1f, 0.3f, 0.5f) * 2;
        Vector3 todEvening = new Vector3(0.85f, 0.7f, 0.35f) * 2;
        Vector3 todDay = new Vector3(1) * 2;

        public void SetTimeOfDay(PreferredTimeOfDay tod)
        {
            switch (tod)
            {
                case PreferredTimeOfDay.Night: TimeOfDay = todNight; break;
                case PreferredTimeOfDay.Evening: TimeOfDay = todEvening; break;
                case PreferredTimeOfDay.Day: default: TimeOfDay = todDay; break;
            }

            UpdateEffects();
        }

        public void SetInternalResolution(int width = 0, int height = 0)
        {
            if (width == 0 || height == 0)
            {
                width = graphics.PreferredBackBufferWidth;
                height = graphics.PreferredBackBufferHeight;
            }

            eng.userContext.BufferSize = new Vector2(width, height);
            eng.CreateScreenBuffer();

            UpdateSplitscreenViewports(graphics, eng.screenBuffer);
        }

        public void UpdateInternalResolution()
        {
            if (EngineSettings.Instance.InternalPSXResolution)
                SetInternalResolution(512, 216);
            else
                SetInternalResolution();
        }

        public void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            UpdateSplitscreenViewports(graphics);
            SwitchDisplayMode();
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        public void UpdateFps30(object sender, EventArgs args)
        {
            EngineSettings.Instance.Fps30 = (sender as BoolMenuItem).Value;
        }

        public void UpdateTargetFramerate()
        {
            double temp = 1000d / (EngineSettings.Instance.Fps30 ? (double)30 : (double)60) * 10000d;
            TargetElapsedTime = new TimeSpan((long)temp);
        }

        /// <summary>
        /// Monogame: default initialize method
        /// </summary>
        protected override void Initialize()
        {
            GameConsole.Write($"ctrviewer - {version}");

            UpdateTargetFramerate();

            Content.RootDirectory = "Content";

            InactiveSleepTime = new TimeSpan(0);


            //hardware switch is disabled due to wrong resolution + crash after AA change
            graphics.HardwareModeSwitch = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            eng = new MainEngine(this);

            eng.Settings.onWindowedChanged += SwitchDisplayMode;
            eng.Settings.onVertexLightingChanged += UpdateEffects;
            eng.Settings.onAntiAliasChanged += eng.UpdateAntiAliasAndBuffer;
            eng.Settings.onVerticalSyncChanged += UpdateVSync;
            eng.Settings.onInternalPsxResolutionChanged += UpdateInternalResolution;
            eng.Settings.onFilteringChanged += Samplers.Refresh;
            eng.Settings.onAnisotropyChanged += Samplers.Refresh;
            eng.Settings.onFps30Changed += UpdateTargetFramerate;

            eng.UpdateAntiAlias();
            UpdateVSync();
            //graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            IsMouseVisible = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            UpdateEffects();

            GamePadHandler.Update();

            Samplers.Refresh();
            Samplers.InitRasterizers();

            SwitchDisplayMode(graphics);

            base.Initialize();
        }

        public bool IsChristmas => (DateTime.Now.Month == 12 && DateTime.Now.Day >= 20) || (DateTime.Now.Month == 1 && DateTime.Now.Day <= 7);

        Texture2D tint;

        /// <summary>
        /// Monogame: default content loading method
        /// </summary>
        protected override void LoadContent()
        {
            GameConsole.Write("LoadContent()");

            ContentVault.AddSound("menu_up", Content.Load<SoundEffect>("sfx\\menu_up"));
            ContentVault.AddSound("menu_down", Content.Load<SoundEffect>("sfx\\menu_down"));

            ContentVault.AddShader("16bits", Content.Load<Effect>("shaders\\16bits"));
            ContentVault.AddShader("scanlines", Content.Load<Effect>("shaders\\scanlines"));


            var rotateLeft = new SimpleAnimation();
            rotateLeft.Keys.Clear();
            rotateLeft.Keys.Add(new AnimationKey() { Parent = rotateLeft, Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1f), Time = 0 });
            rotateLeft.Keys.Add(new AnimationKey() { Parent = rotateLeft, Position = new Vector3(0, 0, 0), Rotation = new Vector3(3.1415f * 2, 0, 0), Scale = new Vector3(1), Time = 8000 });
            rotateLeft.State = rotateLeft.Keys[0];

            var rotateRight = new SimpleAnimation();
            rotateRight.Keys.Clear();
            rotateRight.Keys.Add(new AnimationKey() { Parent = rotateRight, Position = new Vector3(0, 0, 0), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(1f), Time = 0 });
            rotateRight.Keys.Add(new AnimationKey() { Parent = rotateRight, Position = new Vector3(0, 0, 0), Rotation = new Vector3(-3.1415f * 2, 0, 0), Scale = new Vector3(1), Time = 8000 });
            rotateRight.State = rotateRight.Keys[0];

            ContentVault.AddVectorAnim("rotate_left", rotateLeft);
            ContentVault.AddVectorAnim("rotate_right", rotateRight);

            LoadGenericTextures();
            effect.Texture = ContentVault.Textures["test"];
            //effect.TextureEnabled = true;

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.White });

            //load fonts
            GameConsole.Font = Content.Load<SpriteFont>("debug");
            font = Content.Load<SpriteFont>("File");

            BigFileExists = FindBigFile();

            InitMenu();

            LoadCones();

            LoadScenesFromFolder();

            if (Scenes.Count > 0)
                LoadAllScenes();
        }


        #region menustuff
        public void InitMenu()
        {
            //loadmenu
            menu = new Menu(font);
            MenuRootComponent.Font = font;

            menu.Find("wire").Click += ToggleWireFrame;
            menu.Find("newtex").Click += ToggleReplacements;
            menu.Find("vcolor").Click += ToggleVertexColors;
            menu.Find("nocull").Click += ToggleBackfaceCulling;
            menu.Find("skybox").Click += ToggleSkybox;
            menu.Find("water").Click += ToggleWater;
            menu.Find("invis").Click += ToggleInvisible;
            menu.Find("inst").Click += ToggleGameObjects;
            menu.Find("paths").Click += ToggleBotPaths;
            menu.Find("visbox").Click += ToggleVisData;
            menu.Find("visboxleaf").Click += ToggleVisDataLeaves;
            menu.Find("kart").Click += ToggleKartMode;

            menu.Find("window").Click += ToggleWindowed;
            menu.Find("intpsx").Click += ToggleInternalPsxResolution;
            menu.Find("vsync").Click += ToggleVsync;
            menu.Find("antialias").Click += ToggleAntialias;
            menu.Find("filter").Click += ToggleFiltering;
            menu.Find("stereo").Click += ToggleStereoscopic;
            menu.Find("crosseyed").Click += ToggleCrosseyed;
            menu.Find("campos").Click += ToggleCamPos;
            menu.Find("genmips").Click += ToggleMips;
            menu.Find("console").Click += ToggleConsole;

            menu.Find("tod2").Click += HandleTodChange;
            menu.Find("tod2").PressedLeft += HandleTodChange;
            menu.Find("tod2").PressedRight += HandleTodChange;

            menu.Find("flag").Click += ChangeFlag;
            menu.Find("flag").PressedLeft += ChangeFlag;
            menu.Find("flag").PressedRight += ChangeFlag;

            menu.Find("aniso").Click += UpdateAniso;
            menu.Find("aniso").PressedLeft += UpdateAniso;
            menu.Find("aniso").PressedRight += UpdateAniso;

            menu.Find("fps30").Click += UpdateFps30;

            foreach (var level in Enum.GetNames(typeof(Level)))
            {
                var item = menu.Find(level.ToString());
                if (item != null)
                    item.Click += LoadLevelAsync;
            }

            foreach (var level in Enum.GetNames(typeof(Cutscenes)))
            {
                var item = menu.Find(level.ToString());
                if (item != null)
                    item.Click += LoadLevelAsync;
            }

            foreach (var name in new string[] {
                "charselect",
                "gemvalley",
                "nsanity",
                "lostruins",
                "glacierpark",
                "citadelcity",
                "allhubs"
            })
                menu.Find(name).Click += LoadLevelAsync;

            foreach (var lod in Enum.GetNames(typeof(LevelType)))
                menu.Find(lod.ToString()).Click += SetLodAndReload;
        }

        #region [click events]
        public void ChangeFlag(object sender, EventArgs args)
        {
            Game1.currentflag = (sender as IntRangeMenuItem).Value;
            GameConsole.Write($"flag is now: {currentflag.ToString("X4")}");
        }

        public void HandleTodChange(object sender, EventArgs args)
        {
            SetTimeOfDay((PreferredTimeOfDay)((sender as IntRangeMenuItem).Value));
        }

        public void ToggleKartMode(object sender, EventArgs args) => eng.Settings.KartMode = (sender as BoolMenuItem).Value;
        public void ToggleCamPos(object sender, EventArgs args) => eng.Settings.ShowCamPos = (sender as BoolMenuItem).Value;
        public void ToggleMips(object sender, EventArgs args) => eng.Settings.GenerateMips = (sender as BoolMenuItem).Value;

        public void ToggleConsole(object sender, EventArgs args) => eng.Settings.ShowConsole = (sender as BoolMenuItem).Value;

        public void ToggleWindowed(object sender, EventArgs args) => eng.Settings.Windowed = (sender as BoolMenuItem).Value;

        public void ToggleStereoscopic(object sender, EventArgs args)
        {
            eng.Settings.StereoPair = (sender as BoolMenuItem).Value;
            menu.Find("crosseyed").Enabled = eng.Settings.StereoPair;
        }

        public void ToggleCrosseyed(object sender, EventArgs args)
        {
            eng.Settings.StereoCrossEyed = (sender as BoolMenuItem).Value;
            UpdateCameras(new GameTime());
        }


        public void ToggleVsync(object sender, EventArgs args)
        {
            eng.Settings.VerticalSync = (sender as BoolMenuItem).Value;
            menu.Find("fps30").Enabled = eng.Settings.VerticalSync;
        }

        public void ToggleAntialias(object sender, EventArgs args) => eng.Settings.AntiAlias = (sender as BoolMenuItem).Value;

        public void ToggleFiltering(object sender, EventArgs args)
        {
            eng.Settings.EnableFiltering = (sender as BoolMenuItem).Value;
            menu.Find("aniso").Enabled = eng.Settings.EnableFiltering;
        }

        public void UpdateAniso(object sender, EventArgs args) => eng.Settings.AnisotropyLevel = (sender as IntRangeMenuItem).SelectedValue;

        public async void LoadLevelAsync(object sender, EventArgs args)
        {
            IsLoading = true;
            ControlsEnabled = false;

            Task loadlevel = new Task(() =>
            {
                int levelId = (sender as IntMenuItem).Value;
                if (levelId > -1)
                    LoadLevelsFromBig(levelId);
                else
                    LoadLevelsFromBig(200, 203, 206, 209, 212);
            });

            loadlevel.Start();

            await loadlevel;

            IsLoading = false;
            ControlsEnabled = true;
        }

        public void SetLodAndReload(object sender, EventArgs args)
        {
            SetLodAndReload((LevelType)(sender as IntMenuItem).Value);
        }

        public void ToggleVisData(object sender, EventArgs args)
        {
            eng.Settings.VisData = (sender as BoolMenuItem).Value;
            (menu.Find("visboxleaf") as BoolMenuItem).Enabled = eng.Settings.VisData;
        }

        public void ToggleVisDataLeaves(object sender, EventArgs args) => eng.Settings.VisDataLeaves = (sender as BoolMenuItem).Value;

        public void ToggleInternalPsxResolution(object sender, EventArgs args) => eng.Settings.InternalPSXResolution = (sender as BoolMenuItem).Value;

        public void ToggleWireFrame(object sender, EventArgs args) => eng.Settings.DrawWireframe = (sender as BoolMenuItem).Value;

        public void ToggleReplacements(object sender, EventArgs args) => eng.Settings.UseTextureReplacements = (sender as BoolMenuItem).Value;

        public void ToggleVertexColors(object sender, EventArgs args) => eng.Settings.VertexLighting = (sender as BoolMenuItem).Value;

        public void ToggleBackfaceCulling(object sender, EventArgs args) => eng.Settings.BackFaceCulling = (sender as BoolMenuItem).Value;

        public void ToggleSkybox(object sender, EventArgs args) => eng.Settings.ShowSky = (sender as BoolMenuItem).Value;

        public void ToggleWater(object sender, EventArgs args) => eng.Settings.ShowWater = (sender as BoolMenuItem).Value;

        public void ToggleInvisible(object sender, EventArgs args) => eng.Settings.ShowInvisible = (sender as BoolMenuItem).Value;

        public void ToggleGameObjects(object sender, EventArgs args) => eng.Settings.ShowModels = (sender as BoolMenuItem).Value;

        public void ToggleBotPaths(object sender, EventArgs args) => eng.Settings.ShowBotPaths = (sender as BoolMenuItem).Value;

        #endregion
        #endregion

        /// <summary>
        /// Loads various colored "cones" used for point indication (like bot paths).
        /// </summary>
        public void LoadCones()
        {
            AddCone("greencone", Color.Green);
            AddCone("yellowcone", Color.Yellow);
            AddCone("redcone", Color.Red);
            AddCone("purplecone", Color.Purple);
            AddCone("cyancone", Color.Cyan);
            AddCone("limecone", Color.Lime);
            AddCone("goldcone", Color.Gold);
            AddCone("bluecone", Color.Blue);
            AddCone("browncone", Color.Brown);
        }

        /// <summary>
        /// Creates "cone" used for botpaths.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="c"></param>
        public void AddCone(string name, Color c)
        {
            if (ContentVault.Models.ContainsKey(name)) return;

            var modl = new TriList();
            modl.textureEnabled = false;

            //convert this abomination to a model import
            var vptc = new List<VertexPositionColorTexture>();

            Color c1 = Color.Lerp(Color.LightGray, c, 0.5f);
            Color c2 = Color.Lerp(Color.Black, c, 0.5f);

            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(0, 0, 0), c2, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), c1, new Vector2(0, 0)));
            modl.PushQuad(vptc);

            vptc.Clear();
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, 10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(0, 0, 0), c2, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), c1, new Vector2(0, 0)));
            modl.PushQuad(vptc);

            vptc.Clear();
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, 10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), c1, new Vector2(0, 0)));
            modl.PushQuad(vptc);

            modl.Seal();

            var coll = new TriListCollection() { modl };

            ContentVault.Models.Add(name, coll);
        }

        /*
        public void PushVertex(List<VertexPositionColorTexture> list, Vector3 pos, Color color, Vector2 uv)
        {
            list.Add(new VertexPositionColorTexture());
        }
        */

        public void GenerateBasicSky(string name, Color top, Color bottom, Gradient[] grad, int size = 10)
        {
            if (top.A == 0 && top.B == 0)
            {
                GameConsole.Write("no backcolors to use");
                //dont return, we still want to draw grad
                //return;
            }

            var modl = new TriList();
            modl.textureEnabled = false;

            //convert this abomination to a model import
            var vptc = new List<VertexPositionColorTexture>();

            //top half
            if (top.A > 0)
            {
                vptc.Add(new VertexPositionColorTexture(new Vector3(size, 0, -size), DataConverter.ToColor(grad[0].ColorFrom), Vector2.Zero));
                vptc.Add(new VertexPositionColorTexture(new Vector3(-size, 0, -size), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(0, size, 0), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(-size, 0, size), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                modl.PushQuad(vptc);
                vptc.Clear();

                vptc.Add(new VertexPositionColorTexture(new Vector3(-size, 0, size), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(size, 0, size), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(0, size, 0), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(size, 0, -size), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
                modl.PushQuad(vptc);
                vptc.Clear();
            }

            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].To / 25f, 5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].To / 25f, -5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].From / 25f, 5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].From / 25f, -5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].To / 25f, 5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].To  / 25f, -5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].From  / 25f, 5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].From  / 25f, -5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].To  / 25f, 5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].To  / 25f, -5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].From  / 25f, 5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].From  / 25f, -5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();


            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].To / 25f, -5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].To / 25f, -5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].From / 25f, -5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].From / 25f, -5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].To / 25f, -5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].To / 25f, -5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].From / 25f, -5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].From / 25f, -5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].To / 25f, -5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].To / 25f, -5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].From / 25f, -5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].From / 25f, -5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();


            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].To / 25f, -5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].To / 25f, 5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].From / 25f, -5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].From / 25f, 5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].To / 25f, -5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].To / 25f, 5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].From / 25f, -5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].From / 25f, 5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].To / 25f, -5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].To / 25f, 5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].From / 25f, -5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].From / 25f, 5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();


            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].To / 25f, 5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].To / 25f, 5), DataConverter.ToColor(grad[2].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[2].From / 25f, 5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[2].From / 25f, 5), DataConverter.ToColor(grad[2].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].To / 25f, 5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].To / 25f, 5), DataConverter.ToColor(grad[1].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[1].From / 25f, 5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[1].From / 25f, 5), DataConverter.ToColor(grad[1].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();

            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].To / 25f, 5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].To / 25f, 5), DataConverter.ToColor(grad[0].ColorTo), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-5, grad[0].From / 25f, 5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(5, grad[0].From / 25f, 5), DataConverter.ToColor(grad[0].ColorFrom), new Vector2(0, 0)));
            modl.PushQuad(vptc);
            vptc.Clear();



            //bottom half
            if (bottom.A > 0)
            {
                vptc.Add(new VertexPositionColorTexture(new Vector3(size, 0, -size), bottom, new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(0, -size, 0), bottom, new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(-size, 0, -size), bottom, new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(-size, 0, size), bottom, new Vector2(0, 0)));
                modl.PushQuad(vptc);
                vptc.Clear();

                vptc.Add(new VertexPositionColorTexture(new Vector3(-size, 0, size), bottom, new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(0, -size, 0), bottom, new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(size, 0, size), bottom, new Vector2(0, 0)));
                vptc.Add(new VertexPositionColorTexture(new Vector3(size, 0, -size), bottom, new Vector2(0, 0)));
                modl.PushQuad(vptc);
                vptc.Clear();
            }

            modl.Seal();

            var coll = new TriListCollection() { modl };

            ContentVault.Models.Add(name, coll);
        }


        bool IsLoading = false;

        string newtexPath = Helpers.PathCombine(Meta.BasePath, Meta.NewtexName);

        /// <summary>
        /// Loads all necessary textures and processes as required (generates mips, loads replacements, etc)
        /// Should be called after all scenes are already loaded to Scenes array.
        /// </summary>
        private Task LoadTextures()
        {
            GameConsole.Write("LoadTextures()");

            var tasks = new List<Task>();
            var replacements = new Dictionary<string, string>();

            //try to load all png replacement textures, if newtex folder exists
            if (Directory.Exists(newtexPath) && EngineSettings.Instance.UseTextureReplacements)
            {
                string[] files = Directory.GetFiles(newtexPath, "*.png", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    string f = Path.GetFileNameWithoutExtension(file);

                    if (replacements.ContainsKey(f))
                    {
                        GameConsole.Write($"Duplicate replacement: {file}");
                        continue;
                    }

                    replacements.Add(f, file);
                }
            }

            foreach (var s in Scenes)
            {
                var lowtex = s.GetTexturesList(Detail.Low);
                var medtex = s.GetTexturesList(Detail.Med);
                var mdltex = s.GetTexturesList(Detail.Models);

                foreach (var t in s.ctrvram.textures)
                    if (lowtex.ContainsKey(t.Key) || medtex.ContainsKey(t.Key) || mdltex.ContainsKey(t.Key))
                        tasks.Add(LoadTextureAsync(t, replacements));
            }

            return Task.WhenAll(tasks);
        }

        private Task LoadTextureAsync(KeyValuePair<string, System.Drawing.Bitmap> t, Dictionary<string, string> replacements = null)
        {
            Task task = new Task(() => LoadTexture(t, replacements));
            task.Start();

            return task;
        }

        private void LoadTexture(KeyValuePair<string, System.Drawing.Bitmap> t, Dictionary<string, string> replacements = null)
        {
            bool alpha = false;

            var texture = eng.Settings.GenerateMips ?
                MipHelper.LoadTextureFromBitmap(GraphicsDevice, t.Value, out alpha) :
                MipHelper.GetTexture2DFromBitmap(GraphicsDevice, t.Value, out alpha, mipmaps: false);

            ContentVault.AddTexture(t.Key, texture);

            if (eng.Settings.UseTextureReplacements && replacements != null)
                if (replacements.ContainsKey(t.Key))
                {
                    var replacement = eng.Settings.GenerateMips ?
                        MipHelper.LoadTextureFromFile(GraphicsDevice, replacements[t.Key], out alpha) :
                        Texture2D.FromFile(GraphicsDevice, replacements[t.Key]);

                    ContentVault.AddReplacementTexture(t.Key, replacement);
                }

            if (alpha)
                if (!ContentVault.alphalist.Contains(t.Key))
                    ContentVault.alphalist.Add(t.Key);
        }

        private void LoadGenericTextures()
        {
            ContentVault.AddTexture("test", Content.Load<Texture2D>("test"));
            ContentVault.AddTexture("flag", Content.Load<Texture2D>("flag"));
            ContentVault.AddTexture("logo", Content.Load<Texture2D>(IsChristmas ? "logo_xmas" : "logo"));

            //and shader!
            ContentVault.AddShader("16bits", Content.Load<Effect>("shaders\\16bits"));
            ContentVault.AddShader("scanlines", Content.Load<Effect>("shaders\\scanlines"));
        }

        /// <summary>
        /// Loads scene and vram by index. Please note that vram file comes first, then goes scene file.
        /// </summary>
        /// <param name="index">File index in the BIG file.</param>
        /// <returns>CtrScene instance.</returns>
        private CtrScene LoadSceneFromBig(int index)
        {
            var vram = big.ReadEntry(index).ParseAs<CtrVrm>();
            var scene = big.ReadEntry(index + 1).ParseAs<CtrScene>();
            scene.SetVram(vram);

            return scene;
        }

        CtrScene menu_models;

        /// <summary>
        /// Tries to load and adds menu_models scene to the list. Fails silently.
        /// </summary>
        private void LoadMenuModelsScene()
        {
            if (big is null) return;

            if (menu_models is null)
                menu_models = LoadSceneFromBig(215);

            if (menu_models != null)
                Scenes.Add(menu_models);
        }

        /// <summary>
        /// Loads all .ctr models found in relative modelpath.
        /// </summary>
        public void TestLoadExtrenalModels()
        {
            string mdlpath = Helpers.PathCombine(Meta.BasePath, Meta.ModelsPath);

            if (!Directory.Exists(mdlpath)) return;

            string[] files = Directory.GetFiles(mdlpath, "*.ctr");

            int posX = 0;

            foreach (var filename in files)
            {
                var model = CtrModel.FromFile(filename);

                if (!ContentVault.Models.ContainsKey(model.Name))
                {
                    ContentVault.Models.Add(model.Name, DataConverter.ToTriListCollection(model));
                    eng.external.Add(new InstancedModel(model.Name, new Vector3(posX, 0, 0), Vector3.Zero, new Vector3(0.1f)) { anim = ContentVault.GetVectorAnim("rotate_left") });
                    posX += 2;
                }
            }
        }

        /// <summary>
        /// Loads all .lev files from levels folder.
        /// </summary>
        private void LoadScenesFromFolder()
        {
            if (!Directory.Exists("levels"))
                return;

            string[] filelist = Directory.GetFiles("levels", "*.lev");

            if (filelist.Length == 0)
            {
                GameConsole.Write("no levs to load in levels folder.");
                return;
            }

            foreach (var filename in filelist)
                Scenes.Add(CtrScene.FromFile(filename, false));
        }

        /// <summary>
        /// Converts scene array to monogame engine data.
        /// </summary>
        private void LoadAllScenes()
        {
            GameConsole.Write("LoadLevel()");

            RenderEnabled = false;

            loadingStatus = "disposing...";

            eng.Clear();

            loadingStatus = "loading necessary data...";

            //making sure we have default stuff loaded. maybe should just allocate statically?
            LoadCones();
            LoadGenericTextures();
            LoadMenuModelsScene();
            TestLoadExtrenalModels();

            /*
            //loads custom models using assimp
            if (Directory.Exists("custom"))
            {
                string[] files = Directory.GetFiles("custom", "*");

                foreach (var file in files)
                {
                    try
                    {
                        eng.MeshHigh.Add(RawLevelLoader.FromFile(file));
                    }
                    catch
                    {
                        GameConsole.Write($"Failed to import {file}.");
                    }
                }
            }
            */

            loadingStatus = "loading textures...";

            //loading textures between scenes and conversion to monogame for alpha textures info
            LoadTextures().Wait();

            loadingStatus = "converting scenes...";

            foreach (var s in Scenes)
            {
                //eng.MeshHigh.Add(CrashTeamRacingLoader.FromScene(s, Detail.High));
                eng.MeshMed.Add(CrashTeamRacingLoader.FromScene(s, Detail.Med));
                eng.MeshLow.Add(CrashTeamRacingLoader.FromScene(s, Detail.Low));
            }

            //force 1st scene sky and back color
            if (Scenes.Count > 0)
            {
                eng.BackgroundColor = DataConverter.ToColor(Scenes[0].header.backColor);

                GenerateBasicSky(
                    "backsky",
                    DataConverter.ToColor(Scenes[0].header.bgColorTop),
                    DataConverter.ToColor(Scenes[0].header.bgColorBottom),
                    Scenes[0].header.glowGradients,
                    10
                    );

                if (Scenes[0].skybox != null)
                    eng.sky = new MGLevel(Scenes[0].skybox);
            }

            foreach (var scene in Scenes)
            {
                if (scene.spawnGroups != null)
                {
                    foreach (var pa in scene.spawnGroups.Entries)
                        eng.instanced.Add(new InstancedModel("limecone", DataConverter.ToVector3(pa.Position), Vector3.Zero, new Vector3(0.03f)));
                }

                if (scene.header.ptru2 != PsxPtr.Zero)
                {
                    foreach (var v in scene.posu2)
                    {
                        eng.instanced.Add(new InstancedModel("goldcone", DataConverter.ToVector3(v, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                    }
                }
            }

            loadingStatus = "converting models";

            foreach (var scene in Scenes)
                foreach (var model in scene.Models)
                    ContentVault.AddModel(model.Name, DataConverter.ToTriListCollection(model));

            foreach (var s in Scenes)
            {
                //put grid line char models
                for (int i = 0; i < 8; i++)
                    if (s.header.startGrid[i].Position != System.Numerics.Vector3.Zero)
                        eng.instanced.Add(new InstancedModel(
                           ((CharIndex)i).ToString().ToLower(),
                           DataConverter.ToVector3(s.header.startGrid[i].Position),
                           new Vector3(
                                s.header.startGrid[i].Rotation.Y * (float)Math.PI * 2 + ((float)Math.PI / 2),
                                s.header.startGrid[i].Rotation.X * (float)Math.PI * 2,
                                s.header.startGrid[i].Rotation.Z * (float)Math.PI * 2),
                           new Vector3(0.06f)
                            )
                            );

                //put all instanced models
                foreach (var ph in s.Instances)
                {
                    var model = new InstancedModel(
                        ph.ModelName,
                        DataConverter.ToVector3(ph.Pose.Position),
                        new Vector3(
                            ph.Pose.Rotation.Y,
                            ph.Pose.Rotation.X,
                            ph.Pose.Rotation.Z
                        ) * (float)(Math.PI * 2f),
                        new Vector3(ph.Scale.Y, ph.Scale.X, ph.Scale.Z)
                    );

                    //treat special case for hubs
                    if (ph.ModelName == "warppad")
                    {
                        model.ModelName = "beam";
                        model.Scale *= 0.33f;
                    }

                    eng.instanced.Add(model);
                }

                loadingStatus = "put paths...";


                //put all restart points
                foreach (var n in s.respawnPts)
                {
                    eng.paths.Add(new InstancedModel("cyancone", DataConverter.ToVector3(n.Pose.Position), Vector3.Zero, new Vector3(0.03f)));
                }



                //spawn kart if needed
                if (Scenes.Count > 0 && karts.Count == 0)
                {
                    karts.Add(new Kart(
                        DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position),
                        new Vector3(-(float)Math.PI / 2f, 0, 0)));
                }

                ContentVault.AddVectorAnim("defaultCameraPath", DataConverter.ToSimpleAnimation(Scenes[0].respawnPts));

                //update kart
                if (Scenes.Count > 0 && karts.Count > 0)
                {
                    karts[0].Position = DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position);
                    karts[0].ModelName = eng.Settings.PlayerModel;
                    //karts[0].path = ContentVault.GetVectorAnim("defaultCameraPath");
                }

                //put all botpaths
                if (s.nav.paths.Count == 3)
                {
                    foreach (var n in s.nav.paths[0].Frames)
                        eng.paths.Add(new InstancedModel("greencone", DataConverter.ToVector3(n.position), Vector3.Zero, new Vector3(0.03f)));
                    foreach (var n in s.nav.paths[1].Frames)
                        eng.paths.Add(new InstancedModel("yellowcone", DataConverter.ToVector3(n.position), Vector3.Zero, new Vector3(0.03f)));
                    foreach (var n in s.nav.paths[2].Frames)
                        eng.paths.Add(new InstancedModel("redcone", DataConverter.ToVector3(n.position), Vector3.Zero, new Vector3(0.03f)));
                }
            }

            loadingStatus = "populate bsp...";

            eng.bspBranches = new LineCollection();
            eng.bspLeaves.Clear();

            foreach (var s in Scenes)
            {
                if (s.visdata.Count > 0)
                    BspPopulate(s.visdata[0], s, 0);

                //add instance boxes
                foreach (var node in s.instvisdata)
                {
                    eng.bspBranches.PushBox(
                        DataConverter.ToVector3(node.bbox.Min),
                        DataConverter.ToVector3(node.bbox.Max),
                        node.flag.HasFlag(VisDataFlags.NoCollision) ? Color.Green : Color.Red,
                        1 / 100f
                    );
                }
            }

            eng.bspBranches.Seal();

            foreach (var value in eng.bspLeaves.Values)
                value.Seal();

            loadingStatus = "updating effects...";

            UpdateEffects();

            RenderEnabled = true;
        }

        /// <summary>
        /// Colors for different visibility WireBox levels
        /// </summary>
        private readonly Color[] colorLevelsOfBsp =
        {
            new Color(1.0f,1.0f,1.0f,1.0f),
            new Color(1.0f,1.0f,0.7f,1.0f),
            new Color(1.0f,0.7f,0.7f,1.0f),
            new Color(0.7f,0.7f,0.7f,1.0f),
            new Color(0.7f,0.7f,0.5f,1.0f),
            new Color(0.7f,0.5f,0.5f,1.0f),
            new Color(0.5f,0.5f,0.5f,1.0f),
            new Color(0.5f,0.5f,0.3f,1.0f),
            new Color(0.5f,0.3f,0.3f,1.0f),
            new Color(0.3f,0.3f,0.3f,1.0f),
            new Color(0.3f,0.3f,0.0f,1.0f),
            new Color(0.3f,0.0f,0.0f,1.0f),
            new Color(0.0f,0.0f,0.0f,1.0f)
        };

        /// <summary>
        /// Converts visibilty data to a list of wireboxes.
        /// </summary>
        /// <param name="visDat"></param>
        /// <param name="scene"></param>
        /// <param name="level"></param>
        private void BspPopulate(VisData visDat, CtrScene scene, int level)
        {
            var childVisData = scene.GetVisDataChildren(visDat); // if node has children get those children

            // has any children?
            //if (childVisData.Count == 0) return;

            foreach (var node in childVisData)
            {
                if (node is null) continue;

                if (node.IsLeaf) // leaves don't have children
                {
                    eng.bspBranches.PushBox(
                        DataConverter.ToVector3(node.bbox.Min),
                        DataConverter.ToVector3(node.bbox.Max),
                        node.ptrInstanceNodes != 0 ? Color.Yellow : Color.Magenta,
                        1 / 100f
                    );

                    continue;
                }

                // show those children in different color than the parent
                if (!eng.bspLeaves.ContainsKey(level))
                    eng.bspLeaves.Add(level, new LineCollection());

                eng.bspLeaves[level].PushBox(
                    DataConverter.ToVector3(node.bbox.Min),
                    DataConverter.ToVector3(node.bbox.Max),
                    colorLevelsOfBsp[level % colorLevelsOfBsp.Length], 1 / 100f
                );

                BspPopulate(node, scene, level + 1);
            }
        }

        /// <summary>
        /// Looks up for bigfile.big in various locations including: settings path, root path, all system disks roots.
        /// If found, initializes BigFileReader. 
        /// </summary>
        /// <returns>Boolean lookup result.</returns>
        private bool FindBigFile()
        {
            bool result = false;

            if (big is null)
            {
                //check file in settings, most of the time this will be it
                if (File.Exists(eng.Settings.BigFileLocation))
                {
                    result = true;
                }
                //maybe it's in root folder?
                else if (File.Exists(Meta.BigFileName))
                {
                    eng.Settings.BigFileLocation = Meta.BigFileName;
                    result = true;
                }
                else //scan all drives, could be mounted CDROM image, or even physical?
                {
                    foreach (var dInfo in DriveInfo.GetDrives())
                    {
                        string path = Helpers.PathCombine(dInfo.Name, Meta.BigFileName);

                        if (File.Exists(path))
                        {
                            eng.Settings.BigFileLocation = path;
                            result = true;
                            break;
                        }
                    }
                }
            }

            if (!File.Exists(eng.Settings.BigFileLocation))
                return false;

            if (result == true && big is null)
                big = BigFileReader.FromFile(eng.Settings.BigFileLocation);

            GameConsole.Write(result ? $"Bigfile location: {eng.Settings.BigFileLocation}" : "Bigfile not found.");

            return result;
        }

        /// <summary>
        /// Changes LoD settings and reloads the selected level.
        /// </summary>
        /// <param name="ltype"></param>
        private void SetLodAndReload(LevelType ltype = LevelType.Lod1P)
        {
            levelType = ltype;

            if (loadedLevel != -1)
                LoadLevelsFromBig(loadedLevel);
        }

        int loadedLevel = -1;
        string loadingStatus = "...";

        /// <summary>
        /// Loads scenes from BIG file by file ID. Make sure you're passing correct file indices.
        /// </summary>
        /// <param name="absId">Array of absolute file indices.</param>
        private void LoadLevelsFromBig(params int[] absId)
        {
            loadedLevel = absId.Length > 1 ? -1 : absId[0];

            if (loadedLevel > 200)
                loadedLevel = -1;

            //test whether big file is ready
            if (big is null && !FindBigFile())
            {
                GameConsole.Write("Missing BIGFILE!");
                return;
            }

            Scenes.Clear();

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < absId.Length; i++)
            {
                loadingStatus = $"loading scene: {i + 1}/{absId.Length}";

                //if it's a level, consider level type to load (1p, 2p, 4p, tt)
                if (absId[i] < 200)
                    absId[i] += (int)levelType * 2;

                Scenes.Add(LoadSceneFromBig(absId[i]));
            }

            LoadAllScenes();
            ResetCamera();

            sw.Stop();

            loadingStatus = "finished.";

            GameConsole.Write($"loading done in {sw.Elapsed.TotalSeconds} seconds");
        }

        /// <summary>
        /// Resets cameras to the default P1 scene position. Uses 1st loaded scene.
        /// </summary>
        public void ResetCamera()
        {
            if (Scenes.Count > 0)
            {
                eng.Cameras[CameraType.DefaultCamera].Position = DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position);
                eng.Cameras[CameraType.LeftEyeCamera].Position = eng.Cameras[CameraType.DefaultCamera].Position;
                eng.Cameras[CameraType.RightEyeCamera].Position = eng.Cameras[CameraType.DefaultCamera].Position;

                float x = (float)(Scenes[0].header.startGrid[0].Rotation.X * Math.PI * 2f);
                float y = (float)(Scenes[0].header.startGrid[0].Rotation.Y * Math.PI * 2f - Math.PI / 2f);

                foreach (var camera in eng.Cameras.Values)
                    camera.SetRotation(y, x);

                UpdateCameras(new GameTime());
            }
        }

        public bool updatemouse = false;
        public static bool RenderEnabled = true;
        public static bool ControlsEnabled = true;
        public static bool IsDrawing = false;

        bool captureMouse = false;

        int selectedChar = 0;

        /// <summary>
        /// Monogame: default update method
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            //update input before window active check to avoid random camera jumpscares
            InputHandlers.Update();

            if (!IsActive) return;

            if (!graphics.IsFullScreen)
                Window.Title = $"ctrviewer [{Math.Round(1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds)} FPS]";

            //allow fullscreen toggle before checking for controls
            if (KeyboardHandler.IsComboPressed(Keys.RightAlt, Keys.Enter))
                eng.Settings.Windowed ^= true;

            if (!ControlsEnabled) return;


            if (InputHandlers.Process(GameAction.Screenshot))
                eng.TakeScreenShot();

            newmenu.Update(gameTime);

            if (eng.Settings.KartMode)
                foreach (var kart in karts)
                {
                    if (!menu.Visible)
                        if (Scenes.Count > 0)
                        {
                            if (KeyboardHandler.IsPressed(Keys.R))
                                kart.Position = DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position);

                            kart.Update(gameTime, Scenes);

                            eng.Cameras[CameraType.DefaultCamera].Position = kart.Position + new Vector3(0f, 1.2f, 0) +
                                Vector3.Transform(Vector3.Forward * 2f, Matrix.CreateFromYawPitchRoll(kart.Rotation.X, 0, -4f));

                            eng.Cameras[CameraType.DefaultCamera].SetRotation((float)Math.PI + kart.Rotation.X, 0);

                            eng.Cameras[CameraType.SkyCamera].SetRotation((float)Math.PI + kart.Rotation.X, 0);


                            eng.UpdateStereoCamera(CameraType.LeftEyeCamera, eng.Settings.StereoPairSeparation);
                            eng.Cameras[CameraType.LeftEyeCamera].SetRotation(eng.Cameras[CameraType.DefaultCamera].leftRightRot, eng.Cameras[CameraType.DefaultCamera].upDownRot);
                            eng.Cameras[CameraType.LeftEyeCamera].Update(gameTime, updatemouse, true);

                            eng.UpdateStereoCamera(CameraType.RightEyeCamera, eng.Settings.StereoPairSeparation);
                            eng.Cameras[CameraType.RightEyeCamera].SetRotation(eng.Cameras[CameraType.DefaultCamera].leftRightRot, eng.Cameras[CameraType.DefaultCamera].upDownRot);
                            eng.Cameras[CameraType.RightEyeCamera].Update(gameTime, updatemouse, true);
                        }
                }


            if (InputHandlers.Process(GameAction.ForceQuit))
                Exit();

            if (eng.Settings.StereoPair)
            {
                if (GamePadHandler.IsDown(Buttons.RightShoulder) || KeyboardHandler.IsDown(Keys.OemOpenBrackets))
                    eng.Settings.StereoPairSeparation += (float)(100 * gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

                if (GamePadHandler.IsDown(Buttons.LeftShoulder) || KeyboardHandler.IsDown(Keys.OemCloseBrackets))
                    eng.Settings.StereoPairSeparation -= (float)(100 * gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

                if (eng.Settings.StereoPairSeparation < 0) eng.Settings.StereoPairSeparation = 0;

                if (GamePadHandler.IsDown(Buttons.RightShoulder) && GamePadHandler.IsDown(Buttons.LeftShoulder))
                    eng.Settings.StereoPairSeparation = 10;
            }

            if (InputHandlers.Process(GameAction.ToggleConsole))
                eng.Settings.ShowConsole ^= true;

            if (KeyboardHandler.IsPressed(Keys.O) || GamePadHandler.IsPressed(Buttons.LeftShoulder))
            {
                selectedChar--;

                if (selectedChar < 0)
                    selectedChar = 14;

                if (karts.Count > 0)
                    karts[0].ModelName = ((CharIndex)selectedChar).ToString().ToLower();
            }

            if (KeyboardHandler.IsPressed(Keys.P) || GamePadHandler.IsPressed(Buttons.RightShoulder))
            {
                selectedChar++;

                if (selectedChar > 14)
                    selectedChar = 0;

                if (karts.Count > 0)
                    karts[0].ModelName = ((CharIndex)selectedChar).ToString().ToLower();
            }

            if (KeyboardHandler.IsDown(Keys.OemMinus)) eng.Settings.FieldOfView--;
            if (KeyboardHandler.IsDown(Keys.OemPlus)) eng.Settings.FieldOfView++;

            if (InputHandlers.Process(GameAction.MenuToggle))
                menu.Visible ^= true;

            if (menu.Visible)
            {
                menu.Update();

                if (menu.Exec)
                {
                    switch (menu.SelectedItem.Action)
                    {
                        case "settings":
                            var info = new ProcessStartInfo() { Arguments = Meta.SettingsFile, FileName = "notepad" };
                            Process.Start(info);
                            break;
                        case "close":
                            menu.Visible = false;
                            break;
                        case "loadbig":
                            LoadLevelsFromBig(menu.SelectedItem.Value);
                            break;
                        case "link":
                            menu.SetMenu(font, menu.SelectedItem.Param);
                            break;
                        case "toggle":
                            switch (menu.SelectedItem.Param)
                            {
                                case "lod": eng.Settings.UseLowLod ^= true; break;
                                case "kart": eng.Settings.KartMode ^= true; break;
                                default: GameConsole.Write("unimplemented toggle: " + menu.SelectedItem.Param); break;
                            }
                            break;

                        case "exit":
                            Exit();
                            break;
                    }

                    menu.Exec = !menu.Exec;
                }

                //handle "go back"
                if (InputHandlers.Process(GameAction.MenuBack))
                {
                    bool togglemenu = true;

                    foreach (var m in menu.items)
                    {
                        if (m.Action == "link" && m.Text.ToUpper() == "BACK")
                        {
                            menu.SetMenu(font, m.Param);
                            togglemenu = false;
                        }
                    }

                    if (togglemenu) menu.Visible = !menu.Visible;
                }
            }
            else
            {
                eng.Update(gameTime);

                if (karts.Count > 0)
                    karts[0].Update(gameTime);

                if (eng.Settings.KartMode)
                {
                    eng.Cameras[CameraType.DefaultCamera].Update(gameTime, false, false);
                    eng.Cameras[CameraType.SkyCamera].Update(gameTime, false, false);
                }
                else
                {
                    if (ControlsEnabled)
                    {
                        UpdateCameras(gameTime);
                    }
                }
            }


            base.Update(gameTime);
        }

        private void UpdateCameras(GameTime gameTime)
        {
            if (IsActive)
            {
                eng.Cameras[CameraType.DefaultCamera].speedScale -= 0.1f * GamePadHandler.State.Triggers.Left;
                eng.Cameras[CameraType.DefaultCamera].speedScale += 0.1f * GamePadHandler.State.Triggers.Right;

                if (MouseHandler.IsLeftButtonHeld)
                {
                    if (captureMouse)
                    {
                        IsMouseVisible = false;
                        //Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                        updatemouse = true;

                        if (MouseHandler.X <= 0)
                        {
                            Mouse.SetPosition(graphics.PreferredBackBufferWidth - 2, MouseHandler.Y);
                            MouseHandler.Reset();
                        }
                        else if (MouseHandler.X >= graphics.PreferredBackBufferWidth - 1)
                        {
                            Mouse.SetPosition(1, MouseHandler.Y);
                            MouseHandler.Reset();
                        }

                        if (MouseHandler.Y <= 0)
                        {
                            Mouse.SetPosition(MouseHandler.X, graphics.PreferredBackBufferHeight - 2);
                            MouseHandler.Reset();
                        }
                        else if (MouseHandler.Y >= graphics.PreferredBackBufferHeight - 1)
                        {
                            Mouse.SetPosition(MouseHandler.X, 1);
                            MouseHandler.Reset();
                        }


                        if (MouseHandler.IsScrollingUp)
                            eng.Cameras[CameraType.DefaultCamera].speedScale += 0.1f;

                        if (MouseHandler.IsScrollingDown)
                            eng.Cameras[CameraType.DefaultCamera].speedScale -= 0.1f;
                    }
                    else
                    {
                        if (0 <= MouseHandler.X &&
                            MouseHandler.X <= graphics.PreferredBackBufferWidth &&
                            0 <= MouseHandler.Y &&
                            MouseHandler.Y <= graphics.PreferredBackBufferHeight)
                            captureMouse = true;
                        updatemouse = true;
                    }
                }
                else
                {
                    IsMouseVisible = true;
                    updatemouse = false;
                    captureMouse = false;
                }
            }
            else
            {
                IsMouseVisible = true;
                updatemouse = false;
                captureMouse = false;
            }

            if (eng.Cameras[CameraType.DefaultCamera].speedScale < 0.1f)
                eng.Cameras[CameraType.DefaultCamera].speedScale = 0.1f;

            if (eng.Cameras[CameraType.DefaultCamera].speedScale > 5)
                eng.Cameras[CameraType.DefaultCamera].speedScale = 5;

            eng.Cameras[CameraType.SkyCamera].Update(gameTime, updatemouse, false);
            eng.Cameras[CameraType.DefaultCamera].Update(gameTime, updatemouse, true);

            eng.UpdateStereoCamera(CameraType.RightEyeCamera, eng.Settings.StereoPairSeparation);
            eng.Cameras[CameraType.RightEyeCamera].Update(gameTime, updatemouse, true);

            eng.UpdateStereoCamera(CameraType.LeftEyeCamera, eng.Settings.StereoPairSeparation);
            eng.Cameras[CameraType.LeftEyeCamera].Update(gameTime, updatemouse, true);
        }

        /// <summary>
        /// Draws everything level related using the provided camera.
        /// </summary>
        /// <param name="cam">FirstPersonCamera instance.</param>
        private void DrawLevel(FirstPersonCamera cam = null)
        {
            if (!RenderEnabled) return;

            //if we got no camera passed, fall back to default
            if (cam is null)
                cam = eng.Cameras[CameraType.DefaultCamera];


            if (ContentVault.Models.ContainsKey("backsky"))
            {
                effect.Projection = eng.Cameras[CameraType.SkyCamera].ProjectionMatrix;
                effect.View = eng.Cameras[CameraType.SkyCamera].ViewMatrix;

                ContentVault.Models["backsky"].Draw(graphics, effect, null);

                //clear z buffer to make sure skybox is behind everything
                graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);
            }

            //if sky exists and enabled
            if (eng.sky != null && eng.Settings.ShowSky)
                eng.sky.DrawSky(graphics, eng.Cameras[CameraType.SkyCamera], effect, null);


            effect.View = cam.ViewMatrix;
            effect.Projection = cam.ProjectionMatrix;

            alphaTestEffect.View = effect.View;
            alphaTestEffect.Projection = effect.Projection;


            //render ctr models from external folder
            foreach (var v in eng.external)
                v.Draw(graphics, instanceEffect, alphaTestEffect, cam);

            //maybe render game models
            if (eng.Settings.ShowModels)
            {
                //render all instanced models
                foreach (var v in eng.instanced)
                    v.Draw(graphics, instanceEffect, alphaTestEffect, cam);

                //render karts
                //if (KartMode)
                foreach (var k in karts)
                    k.Draw(graphics, instanceEffect, alphaTestEffect, cam);
            }

            //maybe render bot paths
            if (eng.Settings.ShowBotPaths)
            {
                //render bot paths
                foreach (var v in eng.paths)
                    v.Draw(graphics, instanceEffect, null, cam);
            }

            //render level mesh depending on lod
            foreach (var qb in (eng.Settings.UseLowLod ? eng.MeshLow : eng.MeshMed))
                qb.Draw(graphics, effect, alphaTestEffect);

            //maybe render visdata wireboxes
            if (eng.Settings.VisData)
            {
                //texture enabled makes visdata invisible
                effect.TextureEnabled = false;

                eng.bspBranches.Draw(graphics, effect);

                if (eng.Settings.VisDataLeaves)
                    foreach (var value in eng.bspLeaves.Values)
                        value.Draw(graphics, effect);
            }
        }


        public static Color CtrMainFontColor = Color.Yellow;

        /*
        //this can only be used with custom version of spritebatch that accepts color array and assigns individual corner vcolors
        public static Color[] CtrMainFontColor = new Color[4] { 
            new Color(255, 192, 0, 0),
            new Color(255, 192, 0, 0),
            Color.Red,
            Color.Red
        };
        */

        private void DrawLoadingScreen(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(tint, new Rectangle(
                0,
                graphics.PreferredBackBufferHeight / 2 - 10,
                graphics.PreferredBackBufferWidth,
                96
                ), Color.Black);
            spriteBatch.DrawString(font, "LOADING...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString("LOADING...").X / 2), graphics.PreferredBackBufferHeight / 2), CtrMainFontColor);
            spriteBatch.DrawString(font, loadingStatus, new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString(loadingStatus).X / 2), graphics.PreferredBackBufferHeight / 2 + 40), CtrMainFontColor);
            spriteBatch.End();

        }

        /// <summary>
        /// Monogame: default draw method
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            IsDrawing = true;

            //if we're loading, only draw the loading info.
            if (IsLoading)
            {
                DrawLoadingScreen(gameTime, spriteBatch);
                IsDrawing = false;
                return;
            }

            //reset samplers to default
            Samplers.SetToDevice(graphics, EngineSampler.Default);

            //maybe we should switch to screen buffer
            //if (EngineSettings.Instance.InternalPSXResolution)
            GraphicsDevice.SetRenderTarget(eng.screenBuffer);

            //clear the backgound
            GraphicsDevice.Clear(eng.BackgroundColor);

            //if we're using stereoscopic effect, draw level twice for left and right viewport
            if (eng.Settings.StereoPair)
            {
                graphics.GraphicsDevice.Viewport = vpLeft;
                eng.UpdateProjectionMatrices();
                DrawLevel(eng.Cameras[CameraType.LeftEyeCamera]);

                graphics.GraphicsDevice.Viewport = vpRight;
                eng.UpdateProjectionMatrices();
                DrawLevel(eng.Cameras[CameraType.RightEyeCamera]);

                graphics.GraphicsDevice.Viewport = vpFull;
                eng.UpdateProjectionMatrices();
            }
            else
            {
                DrawLevel();
            }

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.PointClamp, effect: eng.Settings.InternalPSXResolution ? ContentVault.GetShader("16bits") : null);
            spriteBatch.Draw(eng.screenBuffer, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
       
            //level done.


            //start drawing menu stuff

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.AnisotropicClamp);

            //draw menu (visibility check is inside this method)
            menu.Draw(GraphicsDevice, spriteBatch, font, tint);

            //print instructions if nothing is loaded yet
            if (!IsLoading && Scenes.Count == 0)
                DrawString(
                    "Crash Team Racing level viewer\r\n\r\n" +
                    "No levels loaded.\r\n" +
                    "Put LEV/VRM files in levels folder,\r\n" +
                    "or put BIGFILE.BIG in root folder,\r\n" +
                    "or insert/mount CTR CD and use load level menu.\r\n\r\n" +
                    "Press ESC to open menu.",
                    new Vector2(20 * graphics.GraphicsDevice.Viewport.Height / 1080f, 20 * graphics.GraphicsDevice.Viewport.Height / 1080f)
                );

            //print fov value, if it's changing
            if (InputHandlers.Process(GameAction.FovUp) || InputHandlers.Process(GameAction.FovDown))
            {
                string text = $"FOV: {eng.Cameras[CameraType.DefaultCamera].ViewAngle.ToString("0.##")}";
                DrawString(text, new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(text).X - 20, 20));
            }

            //print speed scale, if it's changing
            if (GamePadHandler.State.Triggers.Left > 0 || GamePadHandler.State.Triggers.Right > 0)
            {
                string text = $"Camera speed scale: {eng.Cameras[CameraType.DefaultCamera].speedScale.ToString("0.##")}";
                DrawString(text, new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(text).X - 20, 20));
            }

            //print camera position, if enabled
            if (eng.Settings.ShowCamPos)
            {
                var campos = eng.Cameras[CameraType.DefaultCamera].Position;

                DrawString(
                    $"({campos.X.ToString("0.00")}, {campos.Y.ToString("0.00")}, {campos.Z.ToString("0.00")})",
                    new Vector2(20, 20)
                    );
            }
        
            //print kart mode info, if it's enabled
            if (eng.Settings.KartMode && karts.Count > 0)
                DrawString(
                    $"Kart mode: WASD - move, PageUp/PageDown - up/down\r\nsp: {(karts[0].Speed * 100).ToString("0.00")}", 
                    new Vector2(20, 20)
                );

            //draw console, if enabled
            if (eng.Settings.ShowConsole)
                GameConsole.Draw(graphics.GraphicsDevice, spriteBatch);


            //newmenu.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            //menu stuff done.


            //reset depth state to default cause spritebatch uses none
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);

            IsDrawing = false;
        }
 
        private void DrawString(string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(
                font, 
                text,
                position,
                color,
                0,
                Vector2.Zero,
                graphics.GraphicsDevice.Viewport.Height / 1080f,
                SpriteEffects.None,
                0.5f);
        }

        private void DrawString(string text, Vector2 position) => DrawString(text, position, CtrMainFontColor);

        private void MaybeDrawString(bool condition, string text, Vector2 position)
        {
            if (condition)
                DrawString(text, position);
        }

        enum TextAlign
        {
            Auto,
            Left,
            Center,
            Right
        }

        private void DrawText(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, TextAlign align = TextAlign.Auto)
        {

            position.X *= GraphicsDevice.Viewport.Width;
            position.Y *= GraphicsDevice.Viewport.Height;

            if (position.X < 0)
            {
                if (align == TextAlign.Auto)
                    align = TextAlign.Left;

                position.X += GraphicsDevice.Viewport.Width;
            }

            if (position.Y < 0)
                position.Y += GraphicsDevice.Viewport.Height;

            switch (align)
            {
                case TextAlign.Center: position.X -= font.MeasureString(text).X / 2; break;
                case TextAlign.Right: position.X -= font.MeasureString(text).X; break;
                case TextAlign.Left: break;
            }

            spriteBatch.DrawString(font, text, position, color,
                   0,
                   Vector2.Zero,
                   graphics.PreferredBackBufferHeight / 1080f,
                   SpriteEffects.None,
                   0.5f
            );
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            //make sure settings are saved before we exit
            EngineSettings.Save();
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Monogame: default unloading method
        /// </summary>
        protected override void UnloadContent()
        {
            Dispose(true);
        }

        /// <summary>
        /// Cleanup loaded stuff.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            eng.Dispose();
            ContentVault.Clear();
            Content.Unload();
            Scenes.Clear();
        }
    }
}