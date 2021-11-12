using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Shared;
using CTRFramework.Sound;
using CTRFramework.Vram;
using ctrviewer.Engine;
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

namespace ctrviewer
{
    public enum LevelType
    {
        P1 = 0,
        P2 = 1,
        P4 = 2,
        TT = 3
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
        List<Scene> Scenes = new List<Scene>();

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

        public LevelType levelType = LevelType.P1;

        public Game1()
        {
            GameConsole.Write($"ctrviewer - {version}");

            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            graphics.HardwareModeSwitch = false;
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


            GameConsole.Write($"SwitchDisplayMode(): {graphics.PreferredBackBufferWidth}x{graphics.PreferredBackBufferHeight}");
        }

        public Viewport vpFull;
        public Viewport vpLeft;
        public Viewport vpRight;
        public Viewport vpTop;
        public Viewport vpBottom;

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

        public void UpdateEffects()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = eng.Settings.VertexLighting;
            effect.TextureEnabled = true;
            effect.DiffuseColor = eng.Settings.VertexLighting ? TimeOfDay : new Vector3(1f);

            alphaTestEffect = new AlphaTestEffect(GraphicsDevice);
            alphaTestEffect.AlphaFunction = CompareFunction.Greater;
            alphaTestEffect.ReferenceAlpha = 0;
            alphaTestEffect.VertexColorEnabled = eng.Settings.VertexLighting;
            alphaTestEffect.DiffuseColor = effect.DiffuseColor;


            effect.FogEnabled = true;
            effect.FogColor = DataConverter.ToVector3(eng.BackgroundColor);
            effect.FogStart = eng.Cameras[CameraType.DefaultCamera].FarClip / 4 * 3;
            effect.FogEnd = eng.Cameras[CameraType.DefaultCamera].FarClip;

            instanceEffect = new BasicEffect(graphics.GraphicsDevice);
            instanceEffect.VertexColorEnabled = true;
            instanceEffect.TextureEnabled = false;
            instanceEffect.DiffuseColor = eng.Settings.VertexLighting ? TimeOfDay * 0.5f : new Vector3(1f);
        }

        public void UpdateVSync()
        {
            graphics.SynchronizeWithVerticalRetrace = eng.Settings.VerticalSync;
            IsFixedTimeStep = eng.Settings.VerticalSync;
            graphics.ApplyChanges();
        }

        public void UpdateAntiAlias()
        {
            graphics.PreferMultiSampling = !graphics.PreferMultiSampling;
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

        protected override void Initialize()
        {
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


            IsMouseVisible = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            UpdateEffects();

            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
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

        protected override void LoadContent()
        {
            GameConsole.Write("LoadContent()");

            ContentVault.AddSound("menu_up", Content.Load<SoundEffect>("sfx\\menu_up"));
            ContentVault.AddSound("menu_down", Content.Load<SoundEffect>("sfx\\menu_down"));

            LoadGenericTextures();
            effect.Texture = ContentVault.Textures["test"];
            //effect.TextureEnabled = true;

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.Black });

            //load fonts
            GameConsole.Font = Content.Load<SpriteFont>("debug");
            font = Content.Load<SpriteFont>("File");

            BigFileExists = FindBigFile();

            //loadmenu
            menu = new Menu(font);
            MenuRootComponent.Font = font;

            UpdateSplitscreenViewports();

            LoadCones();

            LoadScenes(null);
            LoadLevel();
        }

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

        //convert this abomination to a model import
        public void AddCone(string name, Color c)
        {
            TriList modl = new TriList();
            modl.textureEnabled = false;

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

        private void LoadStuff(string[] scenes)
        {
            IsLoading = true;

            LoadScenes(scenes);
            LoadLevel();
            ResetCamera();

            IsLoading = false;
        }

        private void LoadStuff(List<Scene> scenes)
        {
            IsLoading = true;

            Scenes.Clear();
            Scenes = scenes;
            LoadLevel();
            ResetCamera();

            IsLoading = false;
        }

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

            foreach (Scene s in Scenes)
            {
                foreach (var t in s.ctrvram.textures)
                {
                    bool alpha = false;

                    ContentVault.AddTexture(t.Key, eng.Settings.GenerateMips ? MipHelper.LoadTextureFromBitmap(GraphicsDevice, t.Value, out alpha) : MipHelper.GetTexture2DFromBitmap(GraphicsDevice, t.Value, out alpha, mipmaps: false));

                    if (replacements.ContainsKey(t.Key))
                        ContentVault.AddReplacementTexture(t.Key, eng.Settings.GenerateMips ? MipHelper.LoadTextureFromFile(GraphicsDevice, replacements[t.Key], out alpha) : Texture2D.FromFile(GraphicsDevice, replacements[t.Key]));

                    if (alpha)
                        if (!ContentVault.alphalist.Contains(t.Key))
                            ContentVault.alphalist.Add(t.Key);
                }
            }
        }

        void LoadGenericTextures()
        {
            ContentVault.AddTexture("test", Content.Load<Texture2D>("test"));
            ContentVault.AddTexture("flag", Content.Load<Texture2D>("flag"));
            ContentVault.AddTexture("logo", Content.Load<Texture2D>(IsChristmas ? "logo_xmas" : "logo"));
        }

        private void TestLoadKart()
        {
            Scene cc = new Scene();

            if (big != null)
            {
                big.FileCursor = 216; //menu_models.lev
                cc = big.ReadEntry().ParseAs<Scene>();
            }
            else
            {
                if (File.Exists("menu_models.lev"))
                    cc = Scene.FromFile("menu_models.lev");
            }

            foreach (var model in cc.Models)
                ContentVault.AddModel(model.Name, DataConverter.ToTriList(model, 0.1f));
        }

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


        private void LoadScenes(string[] filelist)
        {
            if (filelist == null)
                filelist = new string[] { };

            Scenes.Clear();

            if (filelist.Length == 0)
            {
                if (Directory.Exists(@"levels\"))
                    filelist = Directory.GetFiles(@"levels\", "*.lev");
            }

            if (filelist.Length == 0)
            {
                GameConsole.Write("no files");
                return;
            }

            foreach (var filename in filelist)
            {
                Scenes.Add(Scene.FromFile(filename, false));
            }
        }

        private void LoadLevel()
        {
            GameConsole.Write("LoadLevel()");

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

            Stopwatch sw = new Stopwatch();
            sw.Start();


            GameConsole.Write("scenes parsed at: " + sw.Elapsed.TotalSeconds);

            //loading textures between scenes and conversion to monogame for alpha textures info
            LoadTextures();

            GameConsole.Write("textures extracted at: " + sw.Elapsed.TotalSeconds);

            foreach (Scene s in Scenes)
            {
                eng.MeshHigh.Add(CrashTeamRacingLoader.FromScene(s, Detail.Med));
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

            foreach (Scene scene in Scenes)
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

            foreach (Scene s in Scenes)
            {
                foreach (var pa in s.header.startGrid)
                    eng.paths.Add(new InstancedModel("purplecone", DataConverter.ToVector3(pa.Position), Vector3.Zero, new Vector3(0.03f)));

                if (loadedLevel != -1)
                    for (int i = 0; i < 8; i++)
                    {
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
                    }

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

            foreach (Scene s in Scenes)
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

        private void BspPopulate(VisData visDat, Scene scene, int level)
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

        protected override void UnloadContent()
        {
            eng.Dispose();
            ContentVault.Clear();
        }

        public bool updatemouse = false;
        public static bool ForceNoCulling = false;
        public static bool HideInvisible = true;
        public static bool HideWater = false;
        public static bool RenderEnabled = true;
        public static bool ControlsEnabled = true;
        public static bool IsDrawing = false;

        bool captureMouse = false;

        GamePadState oldgs = GamePad.GetState(activeGamePad);
        GamePadState newgs = GamePad.GetState(activeGamePad);

        MouseState oldms = new MouseState();
        MouseState newms = new MouseState();


        protected override void Update(GameTime gameTime)
        {
            Window.Title = $"ctrviewer [{Math.Round(1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds)} FPS]";

            KeyboardHandler.Update();

            //if (loading == null)
            //    LoadGame();


            oldgs = newgs;
            oldms = newms;

            newms = Mouse.GetState();
            newgs = GamePad.GetState(activeGamePad);

            if (IsActive)
            {
                if (KeyboardHandler.IsPressed(Keys.PrintScreen))
                    eng.TakeScreenShot();

                newmenu.Update(gameTime, new Point(newms.X, newms.Y));

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

                if (KeyboardHandler.IsComboPressed(Keys.RightAlt, Keys.Enter))
                {
                    eng.Settings.Windowed = !eng.Settings.Windowed;
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
                            //case "load":
                            //    LoadGame();
                            //    InMenu = false;
                            //    break;
                            case "loadbig":
                                LoadLevelsFromBig(menu.SelectedItem.Value);//, 0, 2); 
                                break;
                            case "loadbigadv":
                                LoadLevelsFromBig(200, 203, 206, 209, 212);
                                break;
                            case "setlod1":
                                SetLodAndReload(LevelType.P1);
                                break;
                            case "setlod2":
                                SetLodAndReload(LevelType.P2);
                                break;
                            case "setlod4":
                                SetLodAndReload(LevelType.P4);
                                break;
                            case "setlodtt":
                                SetLodAndReload(LevelType.TT);
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
                            case "setleveltype":
                                levelType = (LevelType)menu.SelectedItem.Value;
                                break;
                            case "toggle":
                                switch (menu.SelectedItem.Param)
                                {
                                    case "newtex": eng.Settings.UseTextureReplacements = !eng.Settings.UseTextureReplacements; break;
                                    case "inst": eng.Settings.ShowModels = !eng.Settings.ShowModels; break;
                                    case "paths": eng.Settings.ShowBotsPath = !eng.Settings.ShowBotsPath; break;
                                    case "lod": eng.Settings.UseLowLod = !eng.Settings.UseLowLod; break;
                                    case "antialias": eng.Settings.AntiAlias = !eng.Settings.AntiAlias; break;
                                    case "invis": HideInvisible = !HideInvisible; break;
                                    case "water": HideWater = !HideWater; break;
                                    case "console": eng.Settings.ShowConsole = !eng.Settings.ShowConsole; break;
                                    case "campos": eng.Settings.ShowCamPos = !eng.Settings.ShowCamPos; break;
                                    case "visbox": eng.Settings.VisData = !eng.Settings.VisData; break;
                                    case "nocull": ForceNoCulling = !ForceNoCulling; break;
                                    case "visboxleaf": eng.Settings.VisDataLeaves = !eng.Settings.VisDataLeaves; break;
                                    case "filter": eng.Settings.EnableFiltering = !eng.Settings.EnableFiltering; break;
                                    case "wire": eng.Settings.DrawWireframe = !eng.Settings.DrawWireframe; break;
                                    case "genmips": eng.Settings.GenerateMips = !eng.Settings.GenerateMips; break;
                                    case "window": eng.Settings.Windowed = !eng.Settings.Windowed; break;
                                    case "vcolor": eng.Settings.VertexLighting = !eng.Settings.VertexLighting; break;
                                    case "stereo": eng.Settings.StereoPair = !eng.Settings.StereoPair; break;
                                    case "sky": eng.Settings.ShowSky = !eng.Settings.ShowSky; break;
                                    case "vsync": eng.Settings.VerticalSync = !eng.Settings.VerticalSync; break;
                                    case "kart": eng.Settings.KartMode = !eng.Settings.KartMode; break;
                                    case "psxres": eng.Settings.InternalPSXResolution = !eng.Settings.InternalPSXResolution; break;
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
                            if (m.Action == "link" && m.Title == "BACK")
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
                        eng.Cameras[CameraType.DefaultCamera].Update(gameTime, false, false, newms, oldms);
                        eng.Cameras[CameraType.SkyCamera].Update(gameTime, false, false, newms, oldms);
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
                if (newms.LeftButton == ButtonState.Pressed)
                {
                    if (captureMouse)
                    {
                        IsMouseVisible = false;
                        //Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                        updatemouse = true;

                        if (newms.X <= 0)
                        {
                            Mouse.SetPosition(graphics.PreferredBackBufferWidth - 2, newms.Y);
                            newms = Mouse.GetState();
                            oldms = newms;
                        }
                        else if (newms.X >= graphics.PreferredBackBufferWidth - 1)
                        {
                            Mouse.SetPosition(1, newms.Y);
                            newms = Mouse.GetState();
                            oldms = newms;
                        }

                        if (newms.Y <= 0)
                        {
                            Mouse.SetPosition(newms.X, graphics.PreferredBackBufferHeight - 2);
                            newms = Mouse.GetState();
                            oldms = newms;
                        }
                        else if (newms.Y >= graphics.PreferredBackBufferHeight - 1)
                        {
                            Mouse.SetPosition(newms.X, 1);
                            newms = Mouse.GetState();
                            oldms = newms;
                        }


                        if (newms.ScrollWheelValue > oldms.ScrollWheelValue)
                        {
                            eng.Cameras[CameraType.DefaultCamera].speedScale += 0.1f;
                        }

                        if (newms.ScrollWheelValue < oldms.ScrollWheelValue)
                        {
                            eng.Cameras[CameraType.DefaultCamera].speedScale -= 0.1f;
                        }


                        if (eng.Cameras[CameraType.DefaultCamera].speedScale < 0.1f)
                            eng.Cameras[CameraType.DefaultCamera].speedScale = 0.1f;

                        if (eng.Cameras[CameraType.DefaultCamera].speedScale > 5)
                            eng.Cameras[CameraType.DefaultCamera].speedScale = 5;
                    }
                    else
                    {
                        if (0 <= newms.X &&
                            newms.X <= graphics.PreferredBackBufferWidth &&
                            0 <= newms.Y &&
                            newms.Y <= graphics.PreferredBackBufferHeight)
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


            eng.Cameras[CameraType.SkyCamera].Update(gameTime, updatemouse, false, newms, oldms);
            eng.Cameras[CameraType.DefaultCamera].Update(gameTime, updatemouse, true, newms, oldms);

            eng.UpdateStereoCamera(CameraType.RightEyeCamera, eng.Settings.StereoPairSeparation);
            eng.Cameras[CameraType.RightEyeCamera].Update(gameTime, updatemouse, true, newms, oldms);

            eng.UpdateStereoCamera(CameraType.LeftEyeCamera, eng.Settings.StereoPairSeparation);
            eng.Cameras[CameraType.LeftEyeCamera].Update(gameTime, updatemouse, true, newms, oldms);
        }

        private void DrawLevel(FirstPersonCamera cam = null)
        {
            if (!RenderEnabled) return;

            if (cam == null)
                cam = eng.Cameras[CameraType.DefaultCamera];

            //if we have a sky and sky is enabled
            if (eng.sky != null && eng.Settings.ShowSky)
            {
                effect.View = eng.Cameras[CameraType.SkyCamera].ViewMatrix;
                effect.Projection = eng.Cameras[CameraType.SkyCamera].ProjectionMatrix;

                effect.DiffuseColor /= 2;
                eng.sky.DrawSky(graphics, effect, null);
                effect.DiffuseColor *= 2;

                alphaTestEffect.DiffuseColor = effect.DiffuseColor;

                //clear z buffer
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);
            }

            effect.View = cam.ViewMatrix;
            effect.Projection = cam.ProjectionMatrix;

            alphaTestEffect.View = effect.View;
            alphaTestEffect.Projection = effect.Projection;

            //render ctr models from external folder
            foreach (var v in eng.external)
                v.Draw(graphics, instanceEffect, null, cam);


            if (eng.Settings.ShowModels || eng.Settings.ShowBotsPath)
            {
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

                if (eng.Settings.ShowBotsPath)
                {
                    //render bot paths
                    foreach (var v in eng.paths)
                        v.Draw(graphics, instanceEffect, null, cam);
                }
            }

            //Samplers.SetToDevice(graphics, EngineRasterizer.Default);

            //render level mesh depending on lod
            foreach (MGLevel qb in (eng.Settings.UseLowLod ? eng.MeshLow : eng.MeshHigh))
                qb.Draw(graphics, effect, alphaTestEffect);


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

        private bool FindBigFile()
        {
            bool result = false;

            if (big == null)
            {
                if (File.Exists(eng.Settings.BigFileLocation))
                {
                    result = true;
                }
                else if (File.Exists(".\\bigfile.big"))
                {
                    eng.Settings.BigFileLocation = ".\\bigfile.big";
                    result = true;
                }
                else //scan drives
                {
                    var drv = DriveInfo.GetDrives();

                    GameConsole.Write("drives: " + drv.Length);

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

        private void SetLodAndReload(LevelType ltype = LevelType.P1)
        {
            levelType = ltype;

            if (loadedLevel != -1)
                LoadLevelsFromBig(loadedLevel);
        }

        int loadedLevel = -1;

        /// <summary>
        /// Loads scenes from BIG file.
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

            List<Scene> scenes = new List<Scene>();

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

                Scene scene = big.ReadEntry().ParseAs<Scene>();
                scene.SetVram(vrm);

                scenes.Add(scene);
            }

            LoadStuff(scenes);
        }

        protected override void Draw(GameTime gameTime)
        {
            //remember we're busy drawing stuff
            IsDrawing = true;


            if (EngineSettings.Instance.InternalPSXResolution)
                GraphicsDevice.SetRenderTarget(eng.screenBuffer);

            GraphicsDevice.Clear(eng.BackgroundColor);

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

            if (EngineSettings.Instance.InternalPSXResolution)
            {
                GraphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                spriteBatch.Draw(eng.screenBuffer, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp);

            menu.Draw(GraphicsDevice, spriteBatch, font, tint);


            if (IsLoading)
                spriteBatch.DrawString(font, "LOADING...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString("LOADING...").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            if (Scenes.Count == 0 && !IsLoading)
                spriteBatch.DrawString(font,
                    "Crash Team Racing level viewer\r\n\r\n" +
                    "No levels loaded.\r\n" +
                    "Put LEV/VRM files in levels folder,\r\n" +
                    "or put BIGFILE.BIG in root folder,\r\n" +
                    "or insert/mount CTR CD and use load level menu.",
                    new Vector2(20 * graphics.GraphicsDevice.Viewport.Height / 1080f, 20 * graphics.GraphicsDevice.Viewport.Height / 1080f),
                    Color.Yellow,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);

            //spriteBatch.DrawString(font, $"{newms.ToString()}", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString($"{newms.ToString()}").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);


            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) || Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                spriteBatch.DrawString(font, String.Format("FOV {0}", eng.Cameras[CameraType.DefaultCamera].ViewAngle.ToString("0.##")), new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(String.Format("FOV {0}", eng.Cameras[CameraType.DefaultCamera].ViewAngle.ToString("0.##"))).X - 20, 20), Color.Yellow);

            if (GamePad.GetState(0).Triggers.Left > 0 || GamePad.GetState(0).Triggers.Right > 0)
                spriteBatch.DrawString(
                    font,
                    $"Speed scale: {eng.Cameras[CameraType.DefaultCamera].speedScale.ToString("0.##")}",
                    new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString($"Speed scale: {eng.Cameras[CameraType.DefaultCamera].speedScale.ToString("0.##")}").X - 20, 20),
                    Color.Yellow);

            if (eng.Settings.ShowCamPos)
                spriteBatch.DrawString(font, $"({eng.Cameras[CameraType.DefaultCamera].Position.X.ToString("0.00")}, {eng.Cameras[CameraType.DefaultCamera].Position.Y.ToString("0.00")}, {eng.Cameras[CameraType.DefaultCamera].Position.Z.ToString("0.00")})", new Vector2(20, 20), Color.Yellow,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);


            if (eng.Settings.KartMode)
                if (karts.Count > 0)
                    spriteBatch.DrawString(font, $"Kart mode: WASD - move, PageUp/PageDown - up/down\r\nsp: {karts[0].Speed}", new Vector2(20, 20), Color.Yellow,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);

            if (eng.Settings.ShowConsole)
                GameConsole.Draw(graphics.GraphicsDevice, spriteBatch);

            newmenu.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            //reset depth state to default cause spritebatch uses none
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // base.Draw(gameTime);

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

        protected override void Dispose(bool disposing)
        {
            Scenes.Clear();
            eng.Dispose();
            ContentVault.Clear();
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            EngineSettings.Save();
            base.OnExiting(sender, args);
        }
    }
}