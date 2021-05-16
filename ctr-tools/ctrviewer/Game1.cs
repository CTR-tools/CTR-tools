using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Shared;
using CTRFramework.Sound;
using ctrviewer.Engine;
using ctrviewer.Engine.Render;
using ctrviewer.Engine.Testing;
using Microsoft.Xna.Framework;
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
        P1 = 0,
        P2 = 1,
        P4 = 2,
        Relic = 3
    }

    public enum PreferredTimeOfDay
    {
        Day,
        Evening,
        Night
    }

    public partial class Game1 : Game
    {
        public MainEngine eng;

        public static bool BigFileExists = false;

        public LevelType levelType = LevelType.P1;

        public MenuRootComponent newmenu = new MenuRootComponent();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;


        List<Kart> karts = new List<Kart>();

        //ctr scenes
        List<Scene> scn = new List<Scene>();


        Menu menu;

        //effects
        BasicEffect effect;                 //used for static level mesh
        BasicEffect instanceEffect;         //used for instanced mesh
        AlphaTestEffect alphaTestEffect;    //used for alpha textures pass



        public static PlayerIndex activeGamePad = PlayerIndex.One;


        //meh
        public static int currentflag = 1;

        //get version only once, because we don't want this to be allocated every frame.
        public static string version = Meta.GetVersion();

        public Game1()
        {
            GameConsole.Write($"ctrviewer - {version}");

            //OBJ.FixCulture();

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
        }

        public static Vector3 TimeOfDay = new Vector3(2f);

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


        protected override void Initialize()
        {
            eng = new MainEngine(this);

            eng.Settings.onWindowedChanged += SwitchDisplayMode;
            eng.Settings.onVertexLightingChanged += UpdateEffects;
            eng.Settings.onAntiAliasChanged += UpdateAntiAlias;
            eng.Settings.onVerticalSyncChanged += UpdateVSync;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            UpdateAntiAlias();
            UpdateVSync();
            graphics.ApplyChanges();

            IsMouseVisible = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            UpdateEffects();

            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                GamePadState state = GamePad.GetState(i);
                if (state.IsConnected)
                {
                    activeGamePad = i;
                    break;
                }
            }

            Samplers.Refresh();
            Samplers.InitRasterizers();

            SwitchDisplayMode();

            newmenu = new MenuRootComponent();

            /*
MenuButton btn = new MenuButton();
btn.Size = new Point(100, 100);
btn.Position = new Point(100, 50);
btn.OnClick += (s, e) => {
    GameConsole.Write($"hello from {btn.Text}");
    btn.buttonState = xButtonState.Disabled;
};
btn.Text = "Hi there, i'm a button.";

newmenu.Children.Add(btn);
*/

            base.Initialize();
        }


        public bool IsChristmas() => (DateTime.Now.Month == 12 && DateTime.Now.Day >= 20) || (DateTime.Now.Month == 1 && DateTime.Now.Day <= 7);

        void LoadGenericTextures()
        {
            ContentVault.AddTexture("test", Content.Load<Texture2D>("test"));
            ContentVault.AddTexture("flag", Content.Load<Texture2D>("flag"));
            ContentVault.AddTexture("logo", Content.Load<Texture2D>(IsChristmas() ? "logo_xmas" : "logo"));
        }


        Texture2D tint;

        protected override void LoadContent()
        {
            GameConsole.Write("LoadContent()");

            LoadGenericTextures();
            effect.Texture = ContentVault.Textures["test"];
            //effect.TextureEnabled = true;

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.Black });

            //load fonts
            GameConsole.font = Content.Load<SpriteFont>("debug");
            font = Content.Load<SpriteFont>("File");

            BigFileExists = FindBigFile();

            //loadmenu
            menu = new Menu(font);
            MenuRootComponent.Font = font;

            UpdateSplitscreenViewports();

            LoadCones();
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
            GameConsole.Write($"AddCone({name}, {c.ToString()})");

            QuadList modl = new QuadList();

            List<VertexPositionColorTexture> vptc = new List<VertexPositionColorTexture>();

            Color c1 = Color.Lerp(Color.LightGray, c, 0.5f);
            Color c2 = Color.Lerp(Color.Black, c, 0.5f);

            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(0, 0, 0), c2, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), c1, new Vector2(0, 0)));
            modl.PushQuad(vptc);

            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, 10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(0, 0, 0), c2, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), c1, new Vector2(0, 0)));
            modl.PushQuad(vptc);

            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, 10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, -10), c1, new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), c1, new Vector2(0, 0)));
            modl.PushQuad(vptc);

            modl.Seal();

            ContentVault.Models.Add(name, modl);
        }


        bool IsLoading = false;

        private void LoadStuff(string[] lev)
        {
            IsLoading = true;

            LoadLevel(lev);
            ResetCamera();

            IsLoading = false;
        }

        private void LoadTextures()
        {
            GameConsole.Write("LoadTextures()");

            foreach (Scene s in scn)
            {
                foreach (var t in s.ctrvram.textures)
                {

                    //first look for texture replacement
                    string path = $".\\newtex\\{t.Key}.png";

                    bool alpha = false;

                    if (File.Exists(path))
                    {
                        if (!ContentVault.Textures.ContainsKey(t.Key))
                        {
                            ContentVault.Textures.Add(t.Key, eng.Settings.GenerateMips ? MipHelper.LoadTextureFromFile(GraphicsDevice, path, out alpha) : Texture2D.FromFile(GraphicsDevice, path));
                            continue;
                        }
                    }

                    if (!ContentVault.Textures.ContainsKey(t.Key))
                        ContentVault.Textures.Add(t.Key, eng.Settings.GenerateMips ? MipHelper.LoadTextureFromBitmap(GraphicsDevice, t.Value, out alpha) : MipHelper.GetTexture2DFromBitmap(GraphicsDevice, t.Value, out alpha, mipmaps: false));

                    if (alpha)
                        if (!ContentVault.alphalist.Contains(t.Key))
                            ContentVault.alphalist.Add(t.Key);
                }
            }
        }


        private void TestLoadKart()
        {
            if (File.Exists("karts.lev"))
            {
                Scene karts = Scene.FromFile("karts.lev");

                foreach (CtrModel m in karts.Models)
                {
                    if (!ContentVault.Tris.ContainsKey(m.Name) && m.Name == "selectkart")
                    {
                        List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

                        foreach (var x in m.Entries[0].verts)
                            li.Add(DataConverter.ToVptc(x, new Vector2b(0, 0), 0.01f));

                        TriList t = new TriList();
                        t.textureEnabled = false;
                        t.textureName = "test";
                        t.scrollingEnabled = false;
                        t.PushTri(li);
                        t.Seal();

                        ContentVault.Tris.Add(m.Name, t);
                    }
                }

                //karts.Add(new Kart("selectkart", MGConverter.ToVector3(scn[0].header.startGrid[0].Position), Vector3.Left, 0.5f));
            }
        }

        public void TestLoadExtrenalModels()
        {
            string mdlpath = Path.Combine(Meta.BasePath, Meta.ModelsPath);

            if (Directory.Exists(mdlpath))
            {
                string[] models = Directory.GetFiles(mdlpath, "*.ctr");

                foreach (var s in models)
                {
                    CtrModel c = CtrModel.FromFile(s);

                    if (!ContentVault.Tris.ContainsKey(c.Name))
                    {
                        List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

                        foreach (var x in c.Entries[0].verts)
                            li.Add(DataConverter.ToVptc(x, new Vector2b(0, 0), 0.01f));

                        TriList t = new TriList();
                        t.textureEnabled = false;
                        t.textureName = "test";
                        t.scrollingEnabled = false;
                        t.PushTri(li);
                        t.Seal();

                        ContentVault.Tris.Add(c.Name, t);

                        eng.external.Add(new InstancedModel(c.Name, Vector3.Zero, Vector3.Zero, new Vector3(0.1f)));
                    }
                }
            }
        }


        private void LoadLevel(string[] lev)
        {
            GameConsole.Write("LoadLevel()");

            if (lev == null)
                lev = new string[] { };

            RenderEnabled = false;

            //wait for the end of frame, in case we are still rendering.
            while (IsDrawing) { };


            //Dispose();
            eng.Clear();
            scn.Clear();

            //making sure we have default stuff loaded. maybe should just allocate statically?
            LoadCones();
            LoadGenericTextures();

            TestLoadKart();
            TestLoadExtrenalModels();


            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (lev.Length == 0)
            {
                if (Directory.Exists(@"levels\"))
                    lev = Directory.GetFiles(@"levels\", "*.lev");
            }

            if (lev.Length == 0)
            {
                GameConsole.Write("no files");
                return;
            }

            foreach (string s in lev)
            {
                scn.Add(Scene.FromFile(s, false));
            }

            GameConsole.Write("scenes parsed at: " + sw.Elapsed.TotalSeconds);

            //loading textures between scenes and conversion to monogame for alpha textures info
            LoadTextures();

            foreach (Scene s in scn)
            {
                eng.MeshHigh.Add(new MGLevel(s, Detail.Med));
                eng.MeshLow.Add(new MGLevel(s, Detail.Low));
            }

            GameConsole.Write("converted scenes to monogame render at: " + sw.Elapsed.TotalSeconds);

            //force 1st scene sky and back color
            if (scn.Count > 0)
            {
                eng.BackgroundColor = DataConverter.ToColor(scn[0].header.backColor);
                if (scn[0].skybox != null)
                    eng.sky = new MGLevel(scn[0].skybox);
            }

            foreach (Scene s in scn)
            {
                if (s.unkadv != null)
                {
                    foreach (var pa in s.unkadv.smth)
                        eng.instanced.Add(new InstancedModel("limecone", DataConverter.ToVector3(pa.Position, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                }

                if (s.header.ptru2 != 0)
                {
                    foreach (var v in s.posu2)
                    {
                        eng.instanced.Add(new InstancedModel("goldcone", DataConverter.ToVector3(v, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                    }
                }

                if (s.header.ptrTrialData != UIntPtr.Zero)
                {
                    foreach (var v in s.posu1)
                    {
                        eng.instanced.Add(new InstancedModel("browncone", DataConverter.ToVector3(v.Position, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                    }
                }
            }


            foreach (Scene s in scn)
                foreach (CtrModel m in s.Models)
                {
                    if (!ContentVault.Tris.ContainsKey(m.Name))
                    {
                        List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

                        foreach (var x in m.Entries[0].verts)
                            li.Add(DataConverter.ToVptc(x, new Vector2b(0, 0), 0.01f));

                        TriList t = new TriList();
                        t.textureEnabled = false;
                        t.textureName = "test";
                        t.scrollingEnabled = false;
                        t.PushTri(li);
                        t.Seal();

                        ContentVault.Tris.Add(m.Name, t);
                    }

                }


            GameConsole.Write("extracted dynamics at: " + sw.Elapsed.TotalSeconds);

            foreach (Scene s in scn)
            {
                foreach (var pa in s.header.startGrid)
                    eng.instanced.Add(new InstancedModel("purplecone", DataConverter.ToVector3(pa.Position), Vector3.Zero, new Vector3(0.03f)));

                foreach (var ph in s.pickups)
                {
                    GameConsole.Write(ph.Scale.ToString());

                    eng.instanced.Add(new InstancedModel(
                        ph.ModelName,
                        DataConverter.ToVector3(ph.Pose.Position),
                        new Vector3(
                            (float)(ph.Pose.Rotation.Y * Math.PI * 2f),
                            (float)(ph.Pose.Rotation.X * Math.PI * 2f),
                            (float)(ph.Pose.Rotation.Z * Math.PI * 2f)
                        ),
                        new Vector3(ph.Scale.Y, ph.Scale.X, ph.Scale.Z)
                        ));
                }

                foreach (var n in s.restartPts)
                    eng.paths.Add(new InstancedModel("cyancone", DataConverter.ToVector3(n.Position), Vector3.Zero, new Vector3(0.03f)));

                if (s.nav.paths.Count == 3)
                {
                    foreach (NavFrame n in s.nav.paths[0].frames)
                        eng.paths.Add(new InstancedModel("greencone", DataConverter.ToVector3(n.position, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                    foreach (NavFrame n in s.nav.paths[1].frames)
                        eng.paths.Add(new InstancedModel("yellowcone", DataConverter.ToVector3(n.position, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                    foreach (NavFrame n in s.nav.paths[2].frames)
                        eng.paths.Add(new InstancedModel("redcone", DataConverter.ToVector3(n.position, 0.01f), Vector3.Zero, new Vector3(0.03f)));
                }
            }

            GameConsole.Write("textures extracted at: " + sw.Elapsed.TotalSeconds);


            foreach (Scene s in scn)
            {
                if (s.visdata.Count > 0)
                    BspDraw(s.visdata[0], s, 0);

                GameConsole.Write(s.Info());
            }

            sw.Stop();

            GameConsole.Write("textures loaded. level done: " + sw.Elapsed.TotalSeconds);

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

        private void BspDraw(VisData visDat, Scene scene, int level)
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
                        BspDraw(b, scene, level + 1);
                    }
                }
            }
        }

        public void ResetCamera()
        {
            if (scn.Count > 0)
            {
                eng.Cameras[CameraType.DefaultCamera].Position = DataConverter.ToVector3(scn[0].header.startGrid[0].Position);
                eng.Cameras[CameraType.LeftEyeCamera].Position = eng.Cameras[CameraType.DefaultCamera].Position;
                eng.Cameras[CameraType.LeftEyeCamera].Position = eng.Cameras[CameraType.DefaultCamera].Position;

                float x = (float)(scn[0].header.startGrid[0].Rotation.X * Math.PI * 2f);
                float y = (float)(scn[0].header.startGrid[0].Rotation.Y * Math.PI * 2f - Math.PI / 2f);

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
        public static bool InMenu = false;
        public static bool HideInvisible = true;
        public static bool HideWater = false;
        public static bool RenderEnabled = true;
        public static bool ControlsEnabled = true;
        public static bool IsDrawing = false;


        GamePadState oldgs = GamePad.GetState(activeGamePad);
        GamePadState newgs = GamePad.GetState(activeGamePad);

        KeyboardState oldkb = new KeyboardState();
        KeyboardState newkb = new KeyboardState();


        public float rotation = 0;

        protected override void Update(GameTime gameTime)
        {
            Window.Title = $"ctrviewer [{Math.Round(1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds)} FPS]";

            //if (loading == null)
            //    LoadGame();

            //x += 0.01f ;
            //if (x > Math.PI * 2)
            //    x = 0;
            //camera.SetRotation(x, y);
            //Console.WriteLine(x);

            newms = Mouse.GetState();

            if (IsActive)
            {

                newmenu.Update(gameTime, new Point(newms.X, newms.Y));

                newgs = GamePad.GetState(activeGamePad);
                newkb = Keyboard.GetState();

                foreach (Kart k in karts)
                    k.Update(gameTime);

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

                if ((newkb.IsKeyDown(Keys.Enter) && newkb.IsKeyDown(Keys.RightAlt)) && !(oldkb.IsKeyDown(Keys.Enter) && newkb.IsKeyDown(Keys.RightAlt)))
                {
                    eng.Settings.Windowed = !eng.Settings.Windowed;
                }

                if (newkb.IsKeyDown(Keys.OemTilde) && !oldkb.IsKeyDown(Keys.OemTilde))
                {
                    eng.Settings.ShowConsole = !eng.Settings.ShowConsole;
                }

                if (newkb.IsKeyDown(Keys.OemMinus)) eng.Settings.FieldOfView--;
                if (newkb.IsKeyDown(Keys.OemPlus)) eng.Settings.FieldOfView++;

                if ((newgs.Buttons.Start == ButtonState.Pressed && oldgs.Buttons.Start != newgs.Buttons.Start) ||
                    (newkb.IsKeyDown(Keys.Escape) && newkb.IsKeyDown(Keys.Escape) != oldkb.IsKeyDown(Keys.Escape)))
                {
                    InMenu = !InMenu;
                }

                if (InMenu)
                {
                    menu.Update(oldgs, newgs, oldkb, newkb);

                    //currentflag = menu.items.Find(x => x.Title == "current flag: {0}").rangeval;

                    if (menu.Exec)
                    {
                        switch (menu.SelectedItem.Action)
                        {
                            case "close":
                                InMenu = false;
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
                                    case "inst": eng.Settings.ShowModels = !eng.Settings.ShowModels; break;
                                    case "paths": eng.Settings.ShowBotsPath = !eng.Settings.ShowBotsPath; break;
                                    case "lod": eng.Settings.UseLowLod = !eng.Settings.UseLowLod; break;
                                    case "antialias": eng.Settings.AntiAlias = !eng.Settings.AntiAlias; break;
                                    case "invis": HideInvisible = !HideInvisible; break;
                                    case "water": HideWater = !HideWater; break;
                                    case "console": eng.Settings.ShowConsole = !eng.Settings.ShowConsole; break;
                                    case "campos": eng.Settings.ShowCamPos = !eng.Settings.ShowCamPos; break;
                                    case "visbox": eng.Settings.VisData = !eng.Settings.VisData; break;
                                    case "visboxleaf": eng.Settings.VisDataLeaves = !eng.Settings.VisDataLeaves; break;
                                    case "filter": Samplers.EnableFiltering = !Samplers.EnableFiltering; Samplers.Refresh(); break;
                                    case "wire": Samplers.EnableWireframe = !Samplers.EnableWireframe; break;
                                    case "genmips": eng.Settings.GenerateMips = !eng.Settings.GenerateMips; break;
                                    case "window": eng.Settings.Windowed = !eng.Settings.Windowed; break;
                                    case "vcolor": eng.Settings.VertexLighting = !eng.Settings.VertexLighting; break;
                                    case "stereo": eng.Settings.StereoPair = !eng.Settings.StereoPair; break;
                                    case "sky": eng.Settings.ShowSky = !eng.Settings.ShowSky; break;
                                    case "vsync": eng.Settings.VerticalSync = !eng.Settings.VerticalSync; break;
                                    default: GameConsole.Write("unimplemented toggle: " + menu.SelectedItem.Param); break;
                                }
                                break;

                            case "exit":
                                Exit();
                                break;
                        }

                        menu.Exec = !menu.Exec;
                    }

                    if (newgs.Buttons.B == ButtonState.Pressed && newgs.Buttons.B != oldgs.Buttons.B)
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

                        if (togglemenu) InMenu = !InMenu;
                    }
                }
                else
                {
                    foreach (MGLevel mg in eng.MeshHigh)
                        mg.Update(gameTime);

                    foreach (InstancedModel im in eng.instanced)
                        im.Update(gameTime);

                    foreach (InstancedModel im in eng.paths)
                        im.Update(gameTime);

                    if (ControlsEnabled)
                    {
                        UpdateCameras(gameTime);
                    }
                }

                oldgs = newgs;
                oldkb = newkb;
            }

            oldms = newms;

            base.Update(gameTime);
        }


        MouseState oldms = new MouseState();
        MouseState newms = new MouseState();

        bool captureMouse = false;

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


        //public static bool twoSided = false;

        private void DrawLevel(FirstPersonCamera cam = null)
        {
            if (cam == null)
                cam = eng.Cameras[CameraType.DefaultCamera];

            if (RenderEnabled)
            {
                //if (loading != null && gameLoaded)
                //{
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

                foreach (var v in eng.external)
                    v.Draw(graphics, instanceEffect, null, cam);

                if (eng.Settings.ShowModels || eng.Settings.ShowBotsPath)
                {
                    Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);

                    if (eng.Settings.ShowModels)
                    {
                        foreach (var v in eng.instanced)
                            v.Draw(graphics, instanceEffect, null, cam);

                        //render karts
                        foreach (Kart k in karts)
                            k.Draw(graphics, instanceEffect, null, cam);
                    }

                    if (eng.Settings.ShowBotsPath)
                    {
                        foreach (var v in eng.paths)
                            v.Draw(graphics, instanceEffect, null, cam);
                    }

                    Samplers.SetToDevice(graphics, EngineRasterizer.Default);
                }

                Samplers.SetToDevice(graphics, EngineRasterizer.Default);

                //render depending on lod
                foreach (MGLevel qb in (eng.Settings.UseLowLod ? eng.MeshLow : eng.MeshHigh))
                    qb.Draw(graphics, effect, alphaTestEffect);


                if (eng.Settings.VisData)
                {
                    //GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);

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
        }


        Task loading;


        private void LoadGame()
        {
            LoadStuff(null);
            loading = Task.Run(() => { });
            //loading = Task.Run(() => LoadStuff());
            //loading.Wait();
        }

        BigFileReader big;
        Howl howl;

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
                big = new BigFileReader(File.OpenRead(eng.Settings.BigFileLocation));

            return result;
        }


        private void LoadLevelsFromBig(params int[] absId)
        {
            if (big == null && !FindBigFile())
            {
                GameConsole.Write("Missing BIGFILE!");
                return;
            }

            List<string> files = new List<string>();

            for (int i = 0; i < absId.Length; i++)
            {
                if (absId[i] < 200)
                    absId[i] += (int)levelType * 2;

                big.FileCursor = absId[i];

                if (Path.GetExtension(big.GetFilename()) != ".vrm")
                    return;

                big.ReadEntry().Save(Meta.BasePath);

                big.NextFile();

                if (Path.GetExtension(big.GetFilename()) != ".lev")
                    return;

                big.ReadEntry().Save(Meta.BasePath);

                files.Add(Path.Combine(Meta.BasePath, big.GetFilename()));
            }

            LoadStuff(files.ToArray());
        }

        protected override void Draw(GameTime gameTime)
        {
            //remember we're busy drawing stuff
            IsDrawing = true;

            GraphicsDevice.Clear(eng.BackgroundColor);

            //graphics.GraphicsDevice.Viewport = vpFull;
            //DrawLevel();

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

            if (InMenu)
                menu.Draw(GraphicsDevice, spriteBatch, font, tint);

            spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

            if (InMenu)
            {
                spriteBatch.Draw(
                    ContentVault.Textures["logo"],
                    new Vector2((graphics.GraphicsDevice.Viewport.Width / 2), 50 * graphics.GraphicsDevice.Viewport.Height / 1080f),
                    new Rectangle(0, 0, ContentVault.Textures["logo"].Width, ContentVault.Textures["logo"].Height),
                    Color.White,
                    0,
                    new Vector2(ContentVault.Textures["logo"].Width / 2, 0),
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f
                    );

                spriteBatch.DrawString(
                    font,
                    version,
                    new Vector2(((graphics.PreferredBackBufferWidth - font.MeasureString(version).X * graphics.GraphicsDevice.Viewport.Height / 1080f) / 2), graphics.PreferredBackBufferHeight - 60 * graphics.GraphicsDevice.Viewport.Height / 1080f),
                    Color.Aquamarine,
                    0,
                    new Vector2(0, 0),
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                     0.5f
                    );
            }

            if (IsLoading)
                spriteBatch.DrawString(font, "LOADING...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString("LOADING...").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            if (scn.Count == 0 && !IsLoading)
                spriteBatch.DrawString(font, $"No levels loaded.\r\nPut LEV/VRM files in levels folder.\r\n...or put BIGFILE.BIG in root folder\r\nand use load level menu.".ToString(),
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


            //spriteBatch.DrawString(font, String.Format("sp: {0}\r\nac:{1}", karts[0].Speed, karts[0].Accel), new Vector2(20, 20), Color.Yellow);

            if (eng.Settings.ShowConsole)
                GameConsole.Draw(graphics.GraphicsDevice, spriteBatch);

            //draw calls count
            //spriteBatch.DrawString(font, GraphicsDevice.Metrics.DrawCount.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString(GraphicsDevice.Metrics.DrawCount.ToString()).X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            newmenu.Draw(gameTime, spriteBatch);

            spriteBatch.End();


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
            scn.Clear();
            eng.Dispose();
            ContentVault.Clear();
            scn.Clear();
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            eng.Settings.Save();
            base.OnExiting(sender, args);
        }
    }
}