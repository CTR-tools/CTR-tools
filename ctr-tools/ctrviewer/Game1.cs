using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Shared;
using CTRFramework.Sound;
using CTRFramework.Vram;
using ctrviewer.Engine;
using ctrviewer.Engine.Gui;
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

        GraphicsDeviceManager graphics;
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

        public static PlayerIndex activeGamePad = PlayerIndex.One;

        public static Vector3 TimeOfDay = new Vector3(2f);
        List<Kart> karts = new List<Kart>();

        //meh
        public static int currentflag = 1;

        //get version only once, because we don't want this to be allocated every frame.
        public static string version = Meta.GetVersion();

        public static bool BigFileExists = false;

        public LevelType levelType = LevelType.Lod1P;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        public void SwitchDisplayMode()
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            if (eng.Settings.Windowed)
            {
                graphics.PreferredBackBufferWidth = graphics.PreferredBackBufferWidth * eng.Settings.WindowScale / 100;
                graphics.PreferredBackBufferHeight = graphics.PreferredBackBufferHeight * eng.Settings.WindowScale / 100;
            }

            UpdateSplitscreenViewports();

            UpdateInternalResolution();

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
        public void UpdateSplitscreenViewports()
        {
            GameConsole.Write("UpdateSplitscreenViewports()");

            vpFull.MaxDepth = graphics.GraphicsDevice.Viewport.MaxDepth;
            vpFull.MinDepth = graphics.GraphicsDevice.Viewport.MinDepth;
            vpFull.Width = graphics.PreferredBackBufferWidth;
            vpFull.Height = graphics.PreferredBackBufferHeight;
            vpFull.X = 0;
            vpFull.Y = 0;

            vpLeft.MaxDepth = graphics.GraphicsDevice.Viewport.MaxDepth;
            vpLeft.MinDepth = graphics.GraphicsDevice.Viewport.MinDepth;
            vpLeft.Width = graphics.PreferredBackBufferWidth / 2;
            vpLeft.Height = graphics.PreferredBackBufferHeight;
            vpLeft.X = 0;
            vpLeft.Y = 0;

            vpRight.MaxDepth = graphics.GraphicsDevice.Viewport.MaxDepth;
            vpRight.MinDepth = graphics.GraphicsDevice.Viewport.MinDepth;
            vpRight.Width = graphics.PreferredBackBufferWidth / 2;
            vpRight.Height = graphics.PreferredBackBufferHeight;
            vpRight.X = graphics.PreferredBackBufferWidth / 2;
            vpRight.Y = 0;

            vpTop.MaxDepth = graphics.GraphicsDevice.Viewport.MaxDepth;
            vpTop.MinDepth = graphics.GraphicsDevice.Viewport.MinDepth;
            vpTop.Width = graphics.PreferredBackBufferWidth;
            vpTop.Height = graphics.PreferredBackBufferHeight / 2;
            vpTop.X = 0;
            vpTop.Y = 0;

            vpBottom.MaxDepth = graphics.GraphicsDevice.Viewport.MaxDepth;
            vpBottom.MinDepth = graphics.GraphicsDevice.Viewport.MinDepth;
            vpBottom.Width = graphics.PreferredBackBufferWidth;
            vpBottom.Height = graphics.PreferredBackBufferHeight / 2;
            vpBottom.X = 0;
            vpBottom.Y = graphics.PreferredBackBufferHeight / 2;
        }

        /// <summary>
        /// Updates effect values based on settings.
        /// </summary>
        public void UpdateEffects()
        {
            if (effect == null)
                effect = new BasicEffect(graphics.GraphicsDevice);

            effect.VertexColorEnabled = eng.Settings.VertexLighting;
            effect.TextureEnabled = true;
            effect.DiffuseColor = eng.Settings.VertexLighting ? TimeOfDay : new Vector3(1f);
            effect.FogEnabled = true;
            effect.FogColor = DataConverter.ToVector3(eng.BackgroundColor);
            effect.FogStart = eng.Cameras[CameraType.DefaultCamera].FarClip / 4 * 3;
            effect.FogEnd = eng.Cameras[CameraType.DefaultCamera].FarClip;

            if (alphaTestEffect == null)
                alphaTestEffect = new AlphaTestEffect(GraphicsDevice);

            alphaTestEffect.AlphaFunction = CompareFunction.Greater;
            alphaTestEffect.ReferenceAlpha = 0;
            alphaTestEffect.VertexColorEnabled = eng.Settings.VertexLighting;
            alphaTestEffect.DiffuseColor = effect.DiffuseColor;

            if (instanceEffect == null)
                instanceEffect = new BasicEffect(graphics.GraphicsDevice);

            instanceEffect.VertexColorEnabled = true;
            instanceEffect.TextureEnabled = false;
            instanceEffect.DiffuseColor = effect.DiffuseColor;
        }

        public void UpdateVSync()
        {
            graphics.SynchronizeWithVerticalRetrace = eng.Settings.VerticalSync;
            IsFixedTimeStep = eng.Settings.VerticalSync;
            graphics.ApplyChanges();
        }

        public void UpdateAntiAlias()
        {
            graphics.PreferMultiSampling = eng.Settings.AntiAlias;
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = eng.Settings.AntiAliasLevel;

            if (eng.screenBuffer != null)
                eng.screenBuffer.GraphicsDevice.PresentationParameters.MultiSampleCount = eng.Settings.AntiAliasLevel;
        }

        public void SetTimeOfDay(PreferredTimeOfDay tod)
        {
            switch (tod)
            {
                case PreferredTimeOfDay.Night:
                    TimeOfDay = new Vector3(0.5f, 0.7f, 1.7f);
                    break;

                case PreferredTimeOfDay.Evening:
                    TimeOfDay = new Vector3(1.7f, 1.4f, 0.7f);
                    break;

                case PreferredTimeOfDay.Day:
                default:
                    TimeOfDay = new Vector3(2f);
                    break;
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

            eng.screenBuffer = new RenderTarget2D(
                GraphicsDevice,
                width,
                height,
                true,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
                );
        }

        public void UpdateInternalResolution()
        {
            if (EngineSettings.Instance.InternalPSXResolution)
                SetInternalResolution(512, 240);
            else
                SetInternalResolution();
        }

        /// <summary>
        /// Monogame: default initialize method
        /// </summary>
        protected override void Initialize()
        {
            GameConsole.Write($"ctrviewer - {version}");

            Content.RootDirectory = "Content";

            graphics.HardwareModeSwitch = false;

            eng = new MainEngine(this);

            eng.Settings.onWindowedChanged += SwitchDisplayMode;
            eng.Settings.onVertexLightingChanged += UpdateEffects;
            eng.Settings.onAntiAliasChanged += UpdateAntiAlias;
            eng.Settings.onVerticalSyncChanged += UpdateVSync;
            eng.Settings.onInternalPsxResolutionChanged += UpdateInternalResolution;
            eng.Settings.onFilteringChanged += Samplers.Refresh;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            UpdateAntiAlias();
            UpdateVSync();
            graphics.ApplyChanges();

            Window.AllowUserResizing = true;


            IsMouseVisible = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            UpdateEffects();

            for (var i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                if (GamePad.GetState(i).IsConnected)
                {
                    activeGamePad = i;
                    break;
                }
            }

            Samplers.Refresh();
            Samplers.InitRasterizers();

            SwitchDisplayMode();

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


            LoadGenericTextures();
            effect.Texture = ContentVault.Textures["test"];
            //effect.TextureEnabled = true;

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.White });

            //load fonts
            GameConsole.Font = Content.Load<SpriteFont>("debug");
            font = Content.Load<SpriteFont>("File");

            BigFileExists = FindBigFile();

            UpdateSplitscreenViewports();

            InitMenu();

            LoadCones();

            LoadScenesFromFolder();
            LoadLevel();
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

            menu.Find("window").Click += ToggleWindowed;
            menu.Find("intpsx").Click += ToggleInternalPsxResolution;
            menu.Find("vsync").Click += ToggleVsync;
            menu.Find("antialias").Click += ToggleAntialias;
            menu.Find("filter").Click += ToggleFiltering;

            foreach (var level in Enum.GetNames(typeof(Level)))
            {
                MenuItem item = menu.Find(level.ToString());
                if (item != null)
                    item.Click += LoadLevelAsync;
            }

            foreach (var level in Enum.GetNames(typeof(Cutscenes)))
            {
                MenuItem item = menu.Find(level.ToString());
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

        public void ToggleWindowed(object sender, EventArgs args)
        {
            eng.Settings.Windowed = (sender as BoolMenuItem).Value;
        }
        public void ToggleVsync(object sender, EventArgs args)
        {
            eng.Settings.VerticalSync = (sender as BoolMenuItem).Value;
        }
        public void ToggleAntialias(object sender, EventArgs args)
        {
            eng.Settings.AntiAlias = (sender as BoolMenuItem).Value;
        }
        public void ToggleFiltering(object sender, EventArgs args)
        {
            eng.Settings.EnableFiltering = (sender as BoolMenuItem).Value;
        }

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
        public void ToggleVisDataLeaves(object sender, EventArgs args)
        {
            eng.Settings.VisDataLeaves = (sender as BoolMenuItem).Value;
        }

        public void ToggleInternalPsxResolution(object sender, EventArgs args)
        {
            eng.Settings.InternalPSXResolution = (sender as BoolMenuItem).Value;
        }

        public void ToggleWireFrame(object sender, EventArgs args)
        {
            eng.Settings.DrawWireframe = (sender as BoolMenuItem).Value;
        }

        public void ToggleReplacements(object sender, EventArgs args)
        {
            eng.Settings.UseTextureReplacements = (sender as BoolMenuItem).Value;
        }

        public void ToggleVertexColors(object sender, EventArgs args)
        {
            eng.Settings.VertexLighting = (sender as BoolMenuItem).Value;
        }

        public void ToggleBackfaceCulling(object sender, EventArgs args)
        {
            eng.Settings.BackFaceCulling = (sender as BoolMenuItem).Value;
        }

        public void ToggleSkybox(object sender, EventArgs args)
        {
            eng.Settings.ShowSky = (sender as BoolMenuItem).Value;
        }

        public void ToggleWater(object sender, EventArgs args)
        {
            eng.Settings.ShowWater = (sender as BoolMenuItem).Value;
        }

        public void ToggleInvisible(object sender, EventArgs args)
        {
            eng.Settings.ShowInvisible = (sender as BoolMenuItem).Value;
        }
        public void ToggleGameObjects(object sender, EventArgs args)
        {
            eng.Settings.ShowModels = (sender as BoolMenuItem).Value;
        }

        public void ToggleBotPaths(object sender, EventArgs args)
        {
            eng.Settings.ShowBotPaths = (sender as BoolMenuItem).Value;
        }

        #endregion

        /// <summary>
        /// Loads various colored "cones".
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
            TriList modl = new TriList();
            modl.textureEnabled = false;

            //convert this abomination to a model import
            List<VertexPositionColorTexture> vptc = new List<VertexPositionColorTexture>();

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

            ContentVault.Models.Add(name, modl);
        }


        bool IsLoading = false;

        /// <summary>
        /// Loads all necessary textures and processes as required (generates mips, loads replacements, etc)
        /// </summary>
        private void LoadTextures()
        {
            GameConsole.Write("LoadTextures()");

            Dictionary<string, string> replacements = new Dictionary<string, string>();

            if (Directory.Exists("newtex"))
            {
                string[] files = Directory.GetFiles("newtex", "*.png", SearchOption.AllDirectories);

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
                int x = s.ctrvram.textures.Count;
                int i = 0;

                var lowtex = s.GetTexturesList(Detail.Med);
                var medtex = s.GetTexturesList(Detail.Low);

                foreach (var t in s.ctrvram.textures)
                {
                    if (lowtex.ContainsKey(t.Key) || medtex.ContainsKey(t.Key))
                    {
                        loadingStatus = $"generate mips: {i}/{x}";
                        LoadTexture(t, replacements);
                        i++;
                    }
                }
            }
        }

        private void LoadTexture(KeyValuePair<string, System.Drawing.Bitmap> t, Dictionary<string, string> replacements)
        {
            bool alpha = false;

            ContentVault.AddTexture(t.Key, eng.Settings.GenerateMips ? MipHelper.LoadTextureFromBitmap(GraphicsDevice, t.Value, out alpha) : MipHelper.GetTexture2DFromBitmap(GraphicsDevice, t.Value, out alpha, mipmaps: false));
            
            if (EngineSettings.Instance.UseTextureReplacements)
                if (replacements.ContainsKey(t.Key))
                    ContentVault.AddReplacementTexture(t.Key, eng.Settings.GenerateMips ? MipHelper.LoadTextureFromFile(GraphicsDevice, replacements[t.Key], out alpha) : Texture2D.FromFile(GraphicsDevice, replacements[t.Key]));

            if (alpha)
                if (!ContentVault.alphalist.Contains(t.Key))
                    ContentVault.alphalist.Add(t.Key);
        }


        void LoadGenericTextures()
        {
            ContentVault.AddTexture("test", Content.Load<Texture2D>("test"));
            ContentVault.AddTexture("flag", Content.Load<Texture2D>("flag"));
            ContentVault.AddTexture("logo", Content.Load<Texture2D>(IsChristmas ? "logo_xmas" : "logo"));
        }

        /// <summary>
        /// Loads all models from menu_models.lev
        /// </summary>
        private void TestLoadKart()
        {
            CtrScene cc = new CtrScene();

            if (big != null)
            {
                big.FileCursor = 216; //menu_models.lev
                cc = big.ReadEntry().ParseAs<CtrScene>();
            }
            else
            {
                if (File.Exists("menu_models.lev"))
                    cc = CtrScene.FromFile("menu_models.lev");
            }

            foreach (var model in cc.Models)
                ContentVault.AddModel(model.Name, DataConverter.ToTriList(model, 0.1f));
        }

        /// <summary>
        /// Loads all .ctr models found in relative modelpath.
        /// </summary>
        public void TestLoadExtrenalModels()
        {
            string mdlpath = Path.Combine(Meta.BasePath, Meta.ModelsPath);

            if (!Directory.Exists(mdlpath)) return;

            string[] models = Directory.GetFiles(mdlpath, "*.ctr");

            if (models.Length == 0) return;

            foreach (var s in models)
            {
                CtrModel c = CtrModel.FromFile(s);

                if (!ContentVault.Models.ContainsKey(c.Name))
                {
                    ContentVault.Models.Add(c.Name, DataConverter.ToTriList(c));
                    eng.external.Add(new InstancedModel(c.Name, Vector3.Zero, Vector3.Zero, new Vector3(0.1f)));
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
        private void LoadLevel()
        {
            GameConsole.Write("LoadLevel()");

            loadingStatus = "begin";

            RenderEnabled = false;

            //wait for the end of frame, in case we are still rendering.
            while (IsDrawing) { };

            //Dispose();
            eng.Clear();

            //making sure we have default stuff loaded. maybe should just allocate statically?
            LoadCones();
            LoadGenericTextures();

            TestLoadKart();
            TestLoadExtrenalModels();

            if (Scenes.Count > 0 && karts.Count == 0)
            {
                karts.Add(new Kart(
                    DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position),
                    new Vector3(-(float)Math.PI / 2f, 0, 0)));
            }

            if (karts.Count > 0)
            {
                karts[0].Position = DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position);
                karts[0].ModelName = eng.Settings.PlayerModel;
            }

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

            Stopwatch sw = new Stopwatch();
            sw.Start();


            GameConsole.Write("scenes parsed at: " + sw.Elapsed.TotalSeconds);

            //loading textures between scenes and conversion to monogame for alpha textures info
            LoadTextures();

            GameConsole.Write("textures extracted at: " + sw.Elapsed.TotalSeconds);

            loadingStatus = "converting scenes";

            foreach (var s in Scenes)
            {
                eng.MeshHigh.Add(CrashTeamRacingLoader.FromScene(s, Detail.High));
                eng.MeshMed.Add(CrashTeamRacingLoader.FromScene(s, Detail.Med));
                eng.MeshLow.Add(CrashTeamRacingLoader.FromScene(s, Detail.Low));
            }

            GameConsole.Write("converted scenes to monogame render at: " + sw.Elapsed.TotalSeconds);

            //force 1st scene sky and back color
            if (Scenes.Count > 0)
            {
                eng.BackgroundColor = DataConverter.ToColor(Scenes[0].header.backColor);
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


            foreach (var scene in Scenes)
                foreach (var model in scene.Models)
                    ContentVault.AddModel(model.Name, DataConverter.ToTriList(model));

            foreach (var s in Scenes)
            {
                foreach (var pa in s.header.startGrid)
                    eng.paths.Add(new InstancedModel("purplecone", DataConverter.ToVector3(pa.Position), Vector3.Zero, new Vector3(0.03f)));

                for (int i = 0; i < 8; i++)
                    if (s.header.startGrid[i].Position != System.Numerics.Vector3.Zero)
                        eng.instanced.Add(new InstancedModel(
                           ((CharIndex)i).ToString().ToLower(),
                           DataConverter.ToVector3(s.header.startGrid[i].Position),
                           new Vector3(
                                s.header.startGrid[i].Rotation.Y * (float)Math.PI * 2 + ((float)Math.PI / 2),
                                s.header.startGrid[i].Rotation.X * (float)Math.PI * 2,
                                s.header.startGrid[i].Rotation.Z * (float)Math.PI * 2),
                           new Vector3(1f)
                            )
                            );

                foreach (var ph in s.pickups)
                {
                    eng.instanced.Add(new InstancedModel(
                        (ph.ModelName == "warppad" ? "beam" : ph.ModelName),
                        DataConverter.ToVector3(ph.Pose.Position),
                        new Vector3(
                            (float)(ph.Pose.Rotation.Y * Math.PI * 2f),
                            (float)(ph.Pose.Rotation.X * Math.PI * 2f),
                            (float)(ph.Pose.Rotation.Z * Math.PI * 2f)
                        ),
                        new Vector3(ph.Scale.Y, ph.Scale.X, ph.Scale.Z) * (ph.ModelName == "warppad" ? 0.33f : 1)
                        ));
                }

                foreach (var n in s.restartPts)
                    eng.paths.Add(new InstancedModel("cyancone", DataConverter.ToVector3(n.Position), Vector3.Zero, new Vector3(0.03f)));

                if (s.nav.paths.Count == 3)
                {
                    foreach (NavFrame n in s.nav.paths[0].Frames)
                        eng.paths.Add(new InstancedModel("greencone", DataConverter.ToVector3(n.position), Vector3.Zero, new Vector3(0.03f)));
                    foreach (NavFrame n in s.nav.paths[1].Frames)
                        eng.paths.Add(new InstancedModel("yellowcone", DataConverter.ToVector3(n.position), Vector3.Zero, new Vector3(0.03f)));
                    foreach (NavFrame n in s.nav.paths[2].Frames)
                        eng.paths.Add(new InstancedModel("redcone", DataConverter.ToVector3(n.position), Vector3.Zero, new Vector3(0.03f)));
                }
            }

            GameConsole.Write("extracted dynamics an bsp at: " + sw.Elapsed.TotalSeconds);

            foreach (var s in Scenes)
            {
                if (s.visdata.Count > 0)
                    BspPopulate(s.visdata[0], s, 0);

                GameConsole.Write(s.Info());
            }

            sw.Stop();

            GameConsole.Write("level done: " + sw.Elapsed.TotalSeconds);

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
            List<VisData> childVisData = scene.GetVisDataChildren(visDat); // if node has children get those children
            if (childVisData.Count > 0)  // has any children?
            {
                foreach (var b in childVisData)
                {
                    if (b == null)
                        continue;

                    if (b.IsLeaf) // leaves don't have children
                    {
                        eng.bbox.Add(new WireBox(DataConverter.ToVector3(b.bbox.Min), DataConverter.ToVector3(b.bbox.Max), Color.Magenta, 1 / 100f));
                    }
                    else
                    {
                        // show those children in different color than the parent
                        if (!eng.bbox2.ContainsKey(level))
                            eng.bbox2.Add(level, new List<WireBox>());

                        eng.bbox2[level].Add(new WireBox(DataConverter.ToVector3(b.bbox.Min), DataConverter.ToVector3(b.bbox.Max), colorLevelsOfBsp[level % colorLevelsOfBsp.Length], 1 / 100f));
                        BspPopulate(b, scene, level + 1);
                    }
                }
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

            if (big == null)
            {
                if (File.Exists(eng.Settings.BigFileLocation))
                {
                    result = true;
                }
                else if (File.Exists("bigfile.big"))
                {
                    eng.Settings.BigFileLocation = "bigfile.big";
                    result = true;
                }
                else //scan drives
                {
                    var drv = DriveInfo.GetDrives();

                    GameConsole.Write($"drives: {drv.Length}");

                    result = false;

                    foreach (DriveInfo dInfo in drv)
                    {
                        GameConsole.Write(dInfo.Name);
                        string path = Path.Combine(dInfo.Name, "bigfile.big");
                        if (File.Exists(path))
                        {
                            eng.Settings.BigFileLocation = path;
                            result = true;
                            break;
                        }
                    }
                }
            }

            if (result == true && big == null)
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
        string loadingStatus = "done";

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
            if (big == null && !FindBigFile())
            {
                GameConsole.Write("Missing BIGFILE!");
                return;
            }

            List<CtrScene> scenes = new List<CtrScene>();

            for (int i = 0; i < absId.Length; i++)
            {
                //if it's a level, consider level type to load (1p, 2p, 4p, tt)
                if (absId[i] < 200)
                    absId[i] += (int)levelType * 2;

                big.FileCursor = absId[i];

                if (Path.GetExtension(big.GetFilename()) != ".vrm")
                    return;

                CtrVrm vrm = big.ReadEntry().ParseAs<CtrVrm>();

                big.NextFile();

                if (Path.GetExtension(big.GetFilename()) != ".lev")
                    return;

                CtrScene scene = big.ReadEntry().ParseAs<CtrScene>();
                scene.SetVram(vrm);

                scenes.Add(scene);

                loadingStatus = $"scenes: {i}/{absId.Length}";
            }

            Scenes = scenes;

            LoadLevel();
            ResetCamera();
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

        GamePadState oldgs = GamePad.GetState(activeGamePad);
        GamePadState newgs = GamePad.GetState(activeGamePad);

        /// <summary>
        /// Monogame: default update method
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            Window.Title = $"ctrviewer [{Math.Round(1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds)} FPS]";

            KeyboardHandler.Update();
            MouseHandler.Update();

            oldgs = newgs;
            newgs = GamePad.GetState(activeGamePad);

            //allow fullscreen toggle before checking for controls
            if (KeyboardHandler.IsComboPressed(Keys.RightAlt, Keys.Enter))
                eng.Settings.Windowed ^= true;

            if (!ControlsEnabled)
                return;

            if (IsActive)
            {
                if (KeyboardHandler.IsPressed(Keys.PrintScreen))
                    eng.TakeScreenShot();

                newmenu.Update(gameTime, MouseHandler.Position);

                if (eng.Settings.KartMode)
                    foreach (var kart in karts)
                    {
                        if (!menu.Visible)
                            if (Scenes.Count > 0)
                            {
                                if (KeyboardHandler.IsPressed(Keys.R))
                                    kart.Position = DataConverter.ToVector3(Scenes[0].header.startGrid[0].Position);

                                kart.Update(gameTime, Scenes);

                                eng.Cameras[CameraType.DefaultCamera].Position = kart.Position + new Vector3(0, 2f, 0) +
                                    Vector3.Transform(Vector3.Forward * 4f, Matrix.CreateFromYawPitchRoll(kart.Rotation.X, 0, -1f));

                                eng.Cameras[CameraType.DefaultCamera].SetRotation((float)Math.PI + kart.Rotation.X, 0);

                                eng.Cameras[CameraType.SkyCamera].SetRotation((float)Math.PI + kart.Rotation.X, 0);



                                eng.UpdateStereoCamera(CameraType.RightEyeCamera, eng.Settings.StereoPairSeparation);
                                eng.Cameras[CameraType.RightEyeCamera].SetRotation(eng.Cameras[CameraType.DefaultCamera].leftRightRot, eng.Cameras[CameraType.DefaultCamera].upDownRot);
                                eng.Cameras[CameraType.RightEyeCamera].Update(gameTime, updatemouse, true);

                                eng.UpdateStereoCamera(CameraType.LeftEyeCamera, eng.Settings.StereoPairSeparation);
                                eng.Cameras[CameraType.LeftEyeCamera].SetRotation(eng.Cameras[CameraType.DefaultCamera].leftRightRot, eng.Cameras[CameraType.DefaultCamera].upDownRot);
                                eng.Cameras[CameraType.LeftEyeCamera].Update(gameTime, updatemouse, true);
                            }
                    }


                if (newgs.Buttons.Start == ButtonState.Pressed && newgs.Buttons.Back == ButtonState.Pressed)
                    Exit();

                if (eng.Settings.StereoPair)
                {
                    if (newgs.IsButtonDown(Buttons.RightShoulder))
                        eng.Settings.StereoPairSeparation += 5;

                    if (newgs.IsButtonDown(Buttons.LeftShoulder))
                        eng.Settings.StereoPairSeparation -= 5;

                    if (eng.Settings.StereoPairSeparation < 0) eng.Settings.StereoPairSeparation = 0;

                    if (newgs.IsButtonDown(Buttons.RightShoulder) && newgs.IsButtonDown(Buttons.LeftShoulder))
                        eng.Settings.StereoPairSeparation = 130;
                }

                if (KeyboardHandler.IsPressed(Keys.OemTilde) || (newgs.IsButtonDown(Buttons.Back) && !oldgs.IsButtonDown(Buttons.Back)))
                {
                    eng.Settings.ShowConsole = !eng.Settings.ShowConsole;
                }

                if (KeyboardHandler.IsDown(Keys.OemMinus)) eng.Settings.FieldOfView--;
                if (KeyboardHandler.IsDown(Keys.OemPlus)) eng.Settings.FieldOfView++;

                if ((newgs.Buttons.Start == ButtonState.Pressed && oldgs.Buttons.Start != newgs.Buttons.Start) || KeyboardHandler.IsPressed(Keys.Escape))
                {
                    menu.Visible = !menu.Visible;
                }

                if (menu.Visible)
                {
                    menu.Update(oldgs, newgs);

                    //currentflag = menu.items.Find(x => x.Title == "current flag: {0}").rangeval;

                    if (menu.Exec)
                    {
                        switch (menu.SelectedItem.Action)
                        {
                            case "close":
                                menu.Visible = false;
                                break;
                            case "loadbig":
                                LoadLevelsFromBig(menu.SelectedItem.Value);//, 0, 2); 
                                break;
                            case "tod_day":
                                SetTimeOfDay(PreferredTimeOfDay.Day);
                                break;
                            case "tod_evening":
                                SetTimeOfDay(PreferredTimeOfDay.Evening);
                                break;
                            case "tod_night":
                                SetTimeOfDay(PreferredTimeOfDay.Night);
                                break;
                            case "link":
                                menu.SetMenu(font);
                                break;
                            case "toggle":
                                switch (menu.SelectedItem.Param)
                                {
                                    case "lod": eng.Settings.UseLowLod ^= true; break;
                                    case "console": eng.Settings.ShowConsole ^= true; break;
                                    case "campos": eng.Settings.ShowCamPos ^= true; break;
                                    case "genmips": eng.Settings.GenerateMips ^= true; break;
                                    case "window": eng.Settings.Windowed ^= true; break;
                                    case "stereo": eng.Settings.StereoPair ^= true; break;
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

                    if ((newgs.Buttons.B == ButtonState.Pressed && newgs.Buttons.B != oldgs.Buttons.B) ||
                        (newgs.Buttons.Y == ButtonState.Pressed && newgs.Buttons.Y != oldgs.Buttons.Y) ||
                        KeyboardHandler.IsPressed(Keys.Back))
                    {
                        bool togglemenu = true;

                        foreach (MenuItem m in menu.items)
                        {
                            if (m.Action == "link" && m.Text == "BACK")
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
            }

            base.Update(gameTime);
        }

        private void UpdateCameras(GameTime gameTime)
        {
            if (IsActive)
            {
                if (MouseHandler.IsLeftButtonPressed)
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


                        if (eng.Cameras[CameraType.DefaultCamera].speedScale < 0.1f)
                            eng.Cameras[CameraType.DefaultCamera].speedScale = 0.1f;

                        if (eng.Cameras[CameraType.DefaultCamera].speedScale > 5)
                            eng.Cameras[CameraType.DefaultCamera].speedScale = 5;
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
            if (cam == null)
                cam = eng.Cameras[CameraType.DefaultCamera];

            //if sky exists and enabled
            if (eng.sky != null && eng.Settings.ShowSky)
                eng.sky.DrawSky(graphics, eng.Cameras[CameraType.SkyCamera], effect, null);

            effect.View = cam.ViewMatrix;
            effect.Projection = cam.ProjectionMatrix;

            alphaTestEffect.View = effect.View;
            alphaTestEffect.Projection = effect.Projection;

            //render ctr models from external folder
            foreach (var v in eng.external)
                v.Draw(graphics, instanceEffect, null, cam);

            //maybe render game models
            if (eng.Settings.ShowModels)
            {
                //render all instanced models
                foreach (var v in eng.instanced)
                    v.Draw(graphics, instanceEffect, null, cam);

                //render karts
                //if (KartMode)
                foreach (Kart k in karts)
                    k.Draw(graphics, instanceEffect, null, cam);
            }

            //maybe render bot paths
            if (eng.Settings.ShowBotPaths)
            {
                //render bot paths
                foreach (var v in eng.paths)
                    v.Draw(graphics, instanceEffect, null, cam);
            }

            //Samplers.SetToDevice(graphics, EngineRasterizer.Default);

            //render level mesh depending on lod
            foreach (MGLevel qb in (eng.Settings.UseLowLod ? eng.MeshMed : eng.MeshHigh))
                qb.Draw(graphics, effect, alphaTestEffect);

            //maybe render visdata wireboxes
            if (eng.Settings.VisData)
            {
                //texture enabled makes visdata invisible
                effect.TextureEnabled = false;

                foreach (var x in eng.bbox)
                    x.Draw(graphics, effect);

                if (eng.Settings.VisDataLeaves)
                    foreach (var key in eng.bbox2.Keys)
                        foreach (var x in eng.bbox2[key])
                            x.Draw(graphics, effect);
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

                IsDrawing = false;

                return;
            }

            //maybe we should switch to screen buffer
            if (EngineSettings.Instance.InternalPSXResolution)
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

            //if we're using internal resolution, draw rendertarget to screenbuffer, applying 16bits postfx
            if (EngineSettings.Instance.InternalPSXResolution)
            {
                GraphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: ContentVault.GetShader("16bits"));
                spriteBatch.Draw(eng.screenBuffer, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }

            //level done.

            //start drawing menu stuff

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp);

            //draw menu (visibility check is inside this method)
            menu.Draw(GraphicsDevice, spriteBatch, font, tint);

            //print instructions if nothing is loaded yet
            if (Scenes.Count == 0 && !IsLoading)
                spriteBatch.DrawString(font,
                    "Crash Team Racing level viewer\r\n\r\n" +
                    "No levels loaded.\r\n" +
                    "Put LEV/VRM files in levels folder,\r\n" +
                    "or put BIGFILE.BIG in root folder,\r\n" +
                    "or insert/mount CTR CD and use load level menu.\r\n\r\n" +
                    "Press ESC to open menu.",
                    new Vector2(20 * graphics.GraphicsDevice.Viewport.Height / 1080f, 20 * graphics.GraphicsDevice.Viewport.Height / 1080f),
                    CtrMainFontColor,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);

            //spriteBatch.DrawString(font, $"{newms.ToString()}", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString($"{newms.ToString()}").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            //print fov value, if it's changing
            if (KeyboardHandler.IsAnyDown(Keys.OemMinus, Keys.OemPlus))
                spriteBatch.DrawString(font, String.Format("FOV {0}", eng.Cameras[CameraType.DefaultCamera].ViewAngle.ToString("0.##")), new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(String.Format("FOV {0}", eng.Cameras[CameraType.DefaultCamera].ViewAngle.ToString("0.##"))).X - 20, 20), CtrMainFontColor);

            //print speed scale, if it's changing
            if (GamePad.GetState(0).Triggers.Left > 0 || GamePad.GetState(0).Triggers.Right > 0)
                spriteBatch.DrawString(
                    font,
                    $"Speed scale: {eng.Cameras[CameraType.DefaultCamera].speedScale.ToString("0.##")}",
                    new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString($"Speed scale: {eng.Cameras[CameraType.DefaultCamera].speedScale.ToString("0.##")}").X - 20, 20),
                    CtrMainFontColor);

            //print camera position, if enabled
            if (eng.Settings.ShowCamPos)
                spriteBatch.DrawString(font, $"({eng.Cameras[CameraType.DefaultCamera].Position.X.ToString("0.00")}, {eng.Cameras[CameraType.DefaultCamera].Position.Y.ToString("0.00")}, {eng.Cameras[CameraType.DefaultCamera].Position.Z.ToString("0.00")})", new Vector2(20, 20), CtrMainFontColor,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);

            //print kart mode info, if it's enabled
            if (eng.Settings.KartMode)
                if (karts.Count > 0)
                    spriteBatch.DrawString(font, $"Kart mode: WASD - move, PageUp/PageDown - up/down\r\nsp: {karts[0].Speed}", new Vector2(20, 20), CtrMainFontColor,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);

            //draw console, if enabled
            if (eng.Settings.ShowConsole)
                GameConsole.Draw(graphics.GraphicsDevice, spriteBatch);

            if (gameTime.IsRunningSlowly)
                spriteBatch.DrawString(font, $"IsRunningSlowly", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 20), CtrMainFontColor,
                0,
                Vector2.Zero,
                graphics.GraphicsDevice.Viewport.Height / 1080f,
                SpriteEffects.None,
                0.5f);

            //newmenu.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            //menu stuff done.

            //reset depth state to default cause spritebatch uses none
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);

            IsDrawing = false;
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