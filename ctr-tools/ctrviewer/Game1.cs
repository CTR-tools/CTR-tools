﻿using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Shared;
using CTRFramework.Sound;
using ctrviewer.Engine;
using ctrviewer.Engine.Render;
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
    public class Game1 : Game
    {
        public static List<string> alphalist = new List<string>();

        EngineSettings settings;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static Dictionary<string, TriList> instTris = new Dictionary<string, TriList>();
        public static Dictionary<string, QuadList> instmodels = new Dictionary<string, QuadList>();


        List<InstancedModel> external = new List<InstancedModel>();
        List<InstancedModel> instanced = new List<InstancedModel>();
        List<InstancedModel> paths = new List<InstancedModel>();

        List<Kart> karts = new List<Kart>();


        List<VertexPositionColorTexture[]> bbox = new List<VertexPositionColorTexture[]>();
        Dictionary<int, List<VertexPositionColorTexture[]>> bbox2 = new Dictionary<int, List<VertexPositionColorTexture[]>>();

        Menu menu;

        //effects
        BasicEffect effect;                 //used for static level mesh
        BasicEffect instanceEffect;         //used for instanced mesh
        AlphaTestEffect alphaTestEffect;    //used for alpha textures pass

        //cameras
        FirstPersonCamera camera;
        FirstPersonCamera rightCamera;
        FirstPersonCamera leftCamera;
        FirstPersonCamera skycamera;

        //ctr scenes
        List<Scene> scn = new List<Scene>();

        //hi and low scenes converted to monogame
        List<MGLevel> MeshHigh = new List<MGLevel>();
        List<MGLevel> MeshLow = new List<MGLevel>();

        //sky
        MGLevel sky;
        Color backColor = Color.Blue;


        public static PlayerIndex activeGamePad = PlayerIndex.One;


        //meh
        public static int currentflag = 1;

        //get version only once, because we don't want this to be allocated every frame.
        public static string version = Meta.GetVersion();


        public Game1()
        {
            GameConsole.Write($"ctrviewer - {version}");

            OBJ.FixCulture();

            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            graphics.HardwareModeSwitch = false;

            settings = EngineSettings.Load();
            settings.onWindowedChanged = SwitchDisplayMode;
            settings.onVertexLightingChanged = UpdateEffects;
            settings.onAntiAliasChanged = UpdateAntiAlias;
            settings.onVerticalSyncChanged = UpdateVSync;
            settings.onFieldOfViewChanged = UpdateFOV;
        }

        public void SwitchDisplayMode()
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            if (settings.Windowed)
            {
                graphics.PreferredBackBufferWidth = graphics.PreferredBackBufferWidth * settings.WindowScale / 100;
                graphics.PreferredBackBufferHeight = graphics.PreferredBackBufferHeight * settings.WindowScale / 100;
            }

            UpdateSplitscreenViewports();

            graphics.IsFullScreen = !settings.Windowed;
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
            effect.VertexColorEnabled = settings.VertexLighting;
            effect.TextureEnabled = true;
            effect.DiffuseColor = settings.VertexLighting ? TimeOfDay : new Vector3(1f);

            alphaTestEffect = new AlphaTestEffect(GraphicsDevice);
            alphaTestEffect.AlphaFunction = CompareFunction.Greater;
            alphaTestEffect.ReferenceAlpha = 0;
            alphaTestEffect.VertexColorEnabled = settings.VertexLighting;
            alphaTestEffect.DiffuseColor = effect.DiffuseColor;


            effect.FogEnabled = true;
            effect.FogColor = new Vector3(backColor.R / 255f, backColor.G / 255f, backColor.B / 255f);
            effect.FogStart = camera.FarClip / 4 * 3;
            effect.FogEnd = camera.FarClip;

            instanceEffect = new BasicEffect(graphics.GraphicsDevice);
            instanceEffect.VertexColorEnabled = true;
            instanceEffect.TextureEnabled = false;
        }

        public static Vector3 TimeOfDay = new Vector3(2f);

        public void UpdateVSync()
        {
            graphics.SynchronizeWithVerticalRetrace = settings.VerticalSync;
            IsFixedTimeStep = settings.VerticalSync;
            graphics.ApplyChanges();
        }

        public void UpdateAntiAlias()
        {
            graphics.PreferMultiSampling = !graphics.PreferMultiSampling;
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = settings.AntiAliasLevel;
        }

        public void UpdateFOV()
        {
            camera.ViewAngle = settings.FieldOfView;
            skycamera.ViewAngle = settings.FieldOfView;
            rightCamera.ViewAngle = settings.FieldOfView;
            leftCamera.ViewAngle = settings.FieldOfView;
        }

        protected override void Initialize()
        {
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            UpdateAntiAlias();
            UpdateVSync();
            graphics.ApplyChanges();

            IsMouseVisible = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new FirstPersonCamera(this);
            rightCamera = new FirstPersonCamera(this);
            leftCamera = new FirstPersonCamera(this);
            skycamera = new FirstPersonCamera(this);

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

            base.Initialize();
        }


        void LoadGenericTextures()
        {
            textures.Add("test", Content.Load<Texture2D>("test"));
            textures.Add("flag", Content.Load<Texture2D>("flag"));

            if ((DateTime.Now.Month == 12 && DateTime.Now.Day >= 20) || (DateTime.Now.Month == 1 && DateTime.Now.Day <= 7))
            {
                textures.Add("logo", Content.Load<Texture2D>("logo_xmas"));
            }
            else
            {
                textures.Add("logo", Content.Load<Texture2D>("logo"));
            }
        }


        Texture2D tint;

        protected override void LoadContent()
        {
            GameConsole.Write("LoadContent()");
            GameConsole.font = Content.Load<SpriteFont>("debug");

            LoadGenericTextures();

            effect.Texture = textures["test"];
            //effect.TextureEnabled = true;

            font = Content.Load<SpriteFont>("File");

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.Black });

            menu = new Menu(font);

            UpdateSplitscreenViewports();

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

            instmodels.Add(name, modl);
        }


        bool gameLoaded = false;

        private void LoadStuff(string[] lev)
        {
            gameLoaded = false;

            LoadLevel(lev);
            ResetCamera();

            gameLoaded = true;
        }

        private void LoadTextures()
        {
            GameConsole.Write("LoadTextures()");

            foreach (Scene s in scn)
            {
                foreach (var t in s.ctrvram.textures)
                {
                    //first look for texture replacement
                    string path = $".\\levels\\newtex\\{t.Key}.png";

                    bool alpha = false;

                    if (File.Exists(path))
                    {
                        if (!textures.ContainsKey(t.Key))
                        {
                            textures.Add(t.Key, settings.GenerateMips ? MipHelper.LoadTextureFromFile(GraphicsDevice, path, out alpha) : Texture2D.FromFile(GraphicsDevice, path));
                            continue;
                        }
                    }

                    if (!textures.ContainsKey(t.Key))
                        textures.Add(t.Key, settings.GenerateMips ? MipHelper.LoadTextureFromBitmap(GraphicsDevice, t.Value, out alpha) : MipHelper.GetTexture2DFromBitmap(GraphicsDevice, t.Value, out alpha, mipmaps: false));

                    if (alpha)
                        if (!alphalist.Contains(t.Key))
                            alphalist.Add(t.Key);
                }
            }
        }


        private void LoadLevel(string[] lev)
        {
            GameConsole.Write("LoadLevel()");

            if (lev == null)
                lev = new string[] { };

            Dispose();
            LoadGenericTextures(); //making sure we have default textures loaded. maybe should just allocate statically?

            if (File.Exists("karts.lev"))
            {
                Scene karts = Scene.FromFile("karts.lev");

                foreach (CtrModel m in karts.Models)
                {
                    if (!instTris.ContainsKey(m.Name) && m.Name == "selectkart")
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

                        instTris.Add(m.Name, t);
                    }

                }
            }

            string[] models = Directory.GetFiles(Path.Combine(Meta.BasePath, Meta.ModelsPath), "*.ctr");

            foreach (var s in models)
            {
                CtrModel c = CtrModel.FromFile(s);

                if (!instTris.ContainsKey(c.Name))
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

                    instTris.Add(c.Name, t);

                    external.Add(new InstancedModel(c.Name, Vector3.Zero, Vector3.Zero, 0.1f));
                }
            }


            RenderEnabled = false;

            //wait for the end of frame, in case we are still rendering.
            while (IsDrawing) { };

            Stopwatch sw = new Stopwatch();
            sw.Start();

            GameConsole.Write("LoadLevel()");

            string[] files = new string[] { };

            if (lev.Length == 0)
            {
                if (Directory.Exists(@"levels\"))
                    files = Directory.GetFiles(@"levels\", "*.lev");
            }
            else
            {
                files = lev;
            }

            if (files.Length == 0)
            {
                GameConsole.Write("no files");
                return;
            }

            foreach (string s in files)
            {
                scn.Add(Scene.FromFile(s));
            }

            GameConsole.Write("scenes parsed at: " + sw.Elapsed.TotalSeconds);

            //loading textures between scenes and conversion to monogame for alpha textures info
            LoadTextures();

            foreach (Scene s in scn)
            {
                MeshHigh.Add(new MGLevel(s, Detail.Med));
                MeshLow.Add(new MGLevel(s, Detail.Low));
            }

            GameConsole.Write("converted scenes to monogame render at: " + sw.Elapsed.TotalSeconds);

            //force 1st scene sky and back color
            if (scn.Count > 0)
            {
                backColor = DataConverter.ToColor(scn[0].header.backColor);
                if (scn[0].skybox != null)
                    sky = new MGLevel(scn[0].skybox);
            }

            foreach (Scene s in scn)
            {
                if (s.unkadv != null)
                {
                    foreach (var pa in s.unkadv.smth)
                        instanced.Add(new InstancedModel("limecone", DataConverter.ToVector3(pa.Position, 0.01f), Vector3.Zero, 0.03f));
                }

                if (s.header.ptru2 != 0)
                {
                    foreach (var v in s.posu2)
                    {
                        instanced.Add(new InstancedModel("goldcone", DataConverter.ToVector3(v, 0.01f), Vector3.Zero, 0.03f));
                    }
                }

                if (s.header.ptrTrialData != 0)
                {
                    foreach (var v in s.posu1)
                    {
                        instanced.Add(new InstancedModel("browncone", DataConverter.ToVector3(v.Position, 0.01f), Vector3.Zero, 0.03f));
                    }
                }
            }


            foreach (Scene s in scn)
                foreach (CtrModel m in s.Models)
                {
                    if (!instTris.ContainsKey(m.Name))
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

                        instTris.Add(m.Name, t);
                    }

                }


            //karts.Add(new Kart("selectkart", MGConverter.ToVector3(scn[0].header.startGrid[0].Position), Vector3.Left, 0.5f));



            GameConsole.Write("extracted dynamics at: " + sw.Elapsed.TotalSeconds);

            foreach (Scene s in scn)
            {
                foreach (var pa in s.header.startGrid)
                    instanced.Add(new InstancedModel("purplecone", DataConverter.ToVector3(pa.Position, 0.01f), Vector3.Zero, 0.03f));

                foreach (var ph in s.pickups)
                    instanced.Add(new InstancedModel(
                        ph.ModelName,
                        DataConverter.ToVector3(ph.Position, 0.01f),
                        Vector3.Zero,//new Vector3((float)(ph.Angle.X / 4094f * Math.PI * 2), (float)(ph.Angle.Y / 4094f * Math.PI * 2), (float)(ph.Angle.Z / 4094f * Math.PI * 2)),
                        0.05f));

                foreach (var n in s.restartPts)
                    paths.Add(new InstancedModel("cyancone", DataConverter.ToVector3(n.Position, 0.01f), Vector3.Zero, 0.03f));

                if (s.nav.paths.Count == 3)
                {
                    foreach (NavFrame n in s.nav.paths[0].frames)
                        paths.Add(new InstancedModel("greencone", DataConverter.ToVector3(n.position, 0.01f), Vector3.Zero, 0.03f));
                    foreach (NavFrame n in s.nav.paths[1].frames)
                        paths.Add(new InstancedModel("yellowcone", DataConverter.ToVector3(n.position, 0.01f), Vector3.Zero, 0.03f));
                    foreach (NavFrame n in s.nav.paths[2].frames)
                        paths.Add(new InstancedModel("redcone", DataConverter.ToVector3(n.position, 0.01f), Vector3.Zero, 0.03f));
                }
            }



            //foreach (Scene s in scn)
            //    s.ExportTexturesAll(Path.Combine(Meta.BasePath, "levels\\tex"));


            GameConsole.Write("textures extracted at: " + sw.Elapsed.TotalSeconds);

            //files = Directory.GetFiles("tex", "*.png");

            foreach (Scene s in scn)
            {
                BspDraw(s.visdata[0], s, 0);
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
            if (childVisData != null && childVisData.Count > 0)  // has any children?
            {
                foreach (var b in childVisData)
                {
                    if (b == null)
                        continue;

                    if (b.IsLeaf) // leaves don't have children
                    {
                        bbox.Add(DataConverter.ToLineList(b.bbox, Color.Magenta));
                    }
                    else
                    {
                        // show those children in different color than the parent
                        if (!bbox2.ContainsKey(level))
                            bbox2.Add(level, new List<VertexPositionColorTexture[]>());

                        bbox2[level].Add(DataConverter.ToLineList(b.bbox, colorLevelsOfBsp[level % colorLevelsOfBsp.Length]));
                        BspDraw(b, scene, level + 1);
                    }
                }
            }
        }

        public void ResetCamera()
        {
            if (scn.Count > 0)
            {
                camera.Position = DataConverter.ToVector3(scn[0].header.startGrid[0].Position, 0.01f);
                rightCamera.Position = camera.Position;
                leftCamera.Position = camera.Position;

                camera.SetRotation((float)(scn[0].header.startGrid[0].Rotation.X / 4096f * Math.PI * 2), (float)(scn[0].header.startGrid[0].Rotation.Y / 4096 * Math.PI * 2));
                rightCamera.SetRotation((float)(scn[0].header.startGrid[0].Rotation.X / 4096f * Math.PI * 2), (float)(scn[0].header.startGrid[0].Rotation.Y / 4096 * Math.PI * 2));
                leftCamera.SetRotation((float)(scn[0].header.startGrid[0].Rotation.X / 4096f * Math.PI * 2), (float)(scn[0].header.startGrid[0].Rotation.Y / 4096 * Math.PI * 2));
                skycamera.SetRotation((float)(scn[0].header.startGrid[0].Rotation.X / 4096f * Math.PI * 2), (float)(scn[0].header.startGrid[0].Rotation.Y / 4096 * Math.PI * 2));

                UpdateCameras(new GameTime());

                GameConsole.Write(scn[0].header.startGrid[0].Rotation.ToString());
            }
        }

        protected override void UnloadContent()
        {
            scn.Clear();
            textures.Clear();
            instanced.Clear();
            instmodels.Clear();
            instTris.Clear();
        }

        public bool updatemouse = false;
        public static bool InMenu = false;
        public static bool HideInvisible = true;
        public static bool HideWater = false;
        public static bool RenderEnabled = true;
        public static bool ControlsEnabled = true;
        public static bool IsDrawing = false;

        public bool lodEnabled = false;

        GamePadState oldstate = GamePad.GetState(activeGamePad);
        GamePadState newstate = GamePad.GetState(activeGamePad);

        KeyboardState oldkb = new KeyboardState();
        KeyboardState newkb = new KeyboardState();


        public float rotation = 0;

        protected override void Update(GameTime gameTime)
        {
            //rotation += 0.01f;
            //camera.SetRotation(rotation, 0);

            if (loading == null)
                LoadGame();

            newms = Mouse.GetState();

            //x += 0.01f ;
            //if (x > Math.PI * 2)
            //    x = 0;
            //camera.SetRotation(x, y);
            //Console.WriteLine(x);

            if (IsActive)
            {
                newstate = GamePad.GetState(activeGamePad);
                newkb = Keyboard.GetState();

                foreach (Kart k in karts)
                    k.Update(gameTime);

                if (newstate.Buttons.Start == ButtonState.Pressed && newstate.Buttons.Back == ButtonState.Pressed)
                    Exit();

                if (settings.StereoPair)
                {
                    if (newstate.IsButtonDown(Buttons.RightShoulder))
                        settings.StereoPairSeparation += 5;

                    if (newstate.IsButtonDown(Buttons.LeftShoulder))
                        settings.StereoPairSeparation -= 5;

                    if (settings.StereoPairSeparation < 0) settings.StereoPairSeparation = 0;

                    if (newstate.IsButtonDown(Buttons.RightShoulder) && newstate.IsButtonDown(Buttons.LeftShoulder))
                        settings.StereoPairSeparation = 130;
                }

                if ((newkb.IsKeyDown(Keys.Enter) && newkb.IsKeyDown(Keys.RightAlt)) && !(oldkb.IsKeyDown(Keys.Enter) && newkb.IsKeyDown(Keys.RightAlt)))
                {
                    settings.Windowed = !settings.Windowed;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) settings.FieldOfView--;
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) settings.FieldOfView++;

                if ((newstate.Buttons.Start == ButtonState.Pressed && oldstate.Buttons.Start != newstate.Buttons.Start) ||
                    (newkb.IsKeyDown(Keys.Escape) && newkb.IsKeyDown(Keys.Escape) != oldkb.IsKeyDown(Keys.Escape)))
                {
                    InMenu = !InMenu;
                }

                if (InMenu)
                {
                    menu.Update(oldstate, newstate, oldkb, newkb);

                    //currentflag = menu.items.Find(x => x.Title == "current flag: {0}").rangeval;

                    if (menu.Exec)
                    {
                        switch (menu.SelectedItem.Action)
                        {
                            case "close":
                                InMenu = false;
                                break;
                            case "load":
                                LoadGame();
                                InMenu = false;
                                break;
                            case "loadbig":
                                LoadLevelsFromBig(menu.SelectedItem.Value);//, 0, 2); 
                                break;
                            case "loadbigadv":
                                LoadLevelsFromBig(200, 203, 206, 209, 212);
                                break;
                            case "tod_day":
                                TimeOfDay = new Vector3(2f);
                                UpdateEffects();
                                break;
                            case "tod_evening":
                                TimeOfDay = new Vector3(1.7f, 1.4f, 0.7f);
                                UpdateEffects();
                                break;
                            case "tod_night":
                                TimeOfDay = new Vector3(0.5f, 0.7f, 1.7f);
                                UpdateEffects();
                                break;
                            case "link":
                                menu.SetMenu(font);
                                break;
                            case "toggle":
                                switch (menu.SelectedItem.Param)
                                {
                                    case "inst": settings.Models = !settings.Models; break;
                                    case "paths": settings.BotsPath = !settings.BotsPath; break;
                                    case "lod": settings.UseLowLod = !settings.UseLowLod; break;
                                    case "antialias": settings.AntiAlias = !settings.AntiAlias; break;
                                    case "invis": HideInvisible = !HideInvisible; break;
                                    case "water": HideWater = !HideWater; break;
                                    case "console": settings.ShowConsole = !settings.ShowConsole; break;
                                    case "campos": settings.ShowCamPos = !settings.ShowCamPos; break;
                                    case "visbox": settings.VisData = !settings.VisData; break;
                                    case "visboxleaf": settings.VisDataLeaves = !settings.VisDataLeaves; break;
                                    case "filter": Samplers.EnableFiltering = !Samplers.EnableFiltering; Samplers.Refresh(); break;
                                    case "wire": Samplers.EnableWireframe = !Samplers.EnableWireframe; break;
                                    case "genmips": settings.GenerateMips = !settings.GenerateMips; break;
                                    case "window": settings.Windowed = !settings.Windowed; break;
                                    case "vcolor": settings.VertexLighting = !settings.VertexLighting; break;
                                    case "stereo": settings.StereoPair = !settings.StereoPair; break;
                                    case "sky": settings.Sky = !settings.Sky; break;
                                    case "vsync": settings.VerticalSync = !settings.VerticalSync; break;
                                    default: GameConsole.Write("unimplemented toggle: " + menu.SelectedItem.Param); break;
                                }
                                break;

                            case "exit":
                                Exit();
                                break;
                        }

                        menu.Exec = !menu.Exec;
                    }

                    if (newstate.Buttons.B == ButtonState.Pressed && newstate.Buttons.B != oldstate.Buttons.B)
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
                    foreach (MGLevel mg in MeshHigh)
                        mg.Update(gameTime);

                    if (ControlsEnabled)
                    {
                        UpdateCameras(gameTime);
                    }
                }

                foreach (InstancedModel im in instanced)
                    im.Update(gameTime);

                foreach (InstancedModel im in paths)
                    im.Update(gameTime);

                oldstate = newstate;
                oldkb = newkb;

                oldms = newms;
            }

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
                            camera.speedScale += 0.1f;
                        }

                        if (newms.ScrollWheelValue < oldms.ScrollWheelValue)
                        {
                            camera.speedScale -= 0.1f;
                        }


                        if (camera.speedScale < 0.1f)
                            camera.speedScale = 0.1f;

                        if (camera.speedScale > 5)
                            camera.speedScale = 5;
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



            skycamera.Update(gameTime, updatemouse, false, newms, oldms);
            camera.Update(gameTime, updatemouse, true, newms, oldms);

            rightCamera.Position = camera.Position + Vector3.Transform(Vector3.Left * settings.StereoPairSeparation / 100f, Matrix.CreateFromYawPitchRoll(camera.leftRightRot, camera._upDownRot, 0));
            rightCamera.rotationSpeed = camera.rotationSpeed;
            rightCamera.Target = camera.Target;
            rightCamera.Update(gameTime, updatemouse, true, newms, oldms);

            leftCamera.Position = camera.Position + Vector3.Transform(Vector3.Right * settings.StereoPairSeparation / 100f, Matrix.CreateFromYawPitchRoll(camera.leftRightRot, camera._upDownRot, 0));
            leftCamera.rotationSpeed = camera.rotationSpeed;
            leftCamera.Target = camera.Target;
            leftCamera.Update(gameTime, updatemouse, true, newms, oldms);


        }

        private void UpdateProjectionMatrices()
        {
            camera.UpdateProjectionMatrix();
            rightCamera.UpdateProjectionMatrix();
            leftCamera.UpdateProjectionMatrix();
            skycamera.UpdateProjectionMatrix();
        }

        //public static bool twoSided = false;

        private void DrawLevel(FirstPersonCamera cam = null)
        {
            if (RenderEnabled)
            {
                //if (loading != null && gameLoaded)
                //{
                //if we have a sky and sky is enabled
                if (sky != null && settings.Sky)
                {
                    effect.View = skycamera.ViewMatrix;
                    effect.Projection = skycamera.ProjectionMatrix;

                    effect.DiffuseColor /= 2;
                    sky.DrawSky(graphics, effect, null);
                    effect.DiffuseColor *= 2;

                    alphaTestEffect.DiffuseColor = effect.DiffuseColor;

                    //clear z buffer
                    GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);
                }

                effect.View = (cam != null ? cam.ViewMatrix : camera.ViewMatrix);
                effect.Projection = (cam != null ? cam.ProjectionMatrix : camera.ProjectionMatrix);

                alphaTestEffect.View = effect.View;
                alphaTestEffect.Projection = effect.Projection;

                foreach (var v in external)
                    v.Draw(graphics, instanceEffect, null, (cam != null ? cam : camera));

                if (settings.Models || settings.BotsPath)
                {
                    Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);

                    if (settings.Models)
                    {
                        foreach (var v in instanced)
                            v.Draw(graphics, instanceEffect, null, (cam != null ? cam : camera));

                        //render karts
                        foreach (Kart k in karts)
                            k.Draw(graphics, instanceEffect, null, (cam != null ? cam : camera));
                    }

                    if (settings.BotsPath)
                    {
                        foreach (var v in paths)
                            v.Draw(graphics, instanceEffect, null, (cam != null ? cam : camera));
                    }

                    Samplers.SetToDevice(graphics, EngineRasterizer.Default);
                }

                Samplers.SetToDevice(graphics, EngineRasterizer.Default);

                //render depending on lod
                foreach (MGLevel qb in (settings.UseLowLod ? MeshLow : MeshHigh))
                    qb.Draw(graphics, effect, alphaTestEffect);


                if (settings.VisData)
                {
                    //GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);

                    //texture enabled makes visdata invisible
                    effect.TextureEnabled = false;

                    foreach (var x in bbox)
                    {
                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, x, 0, x.Length / 2);
                        }
                    }

                    if (settings.VisDataLeaves)
                        foreach (var key in bbox2.Keys)
                            foreach (var x in bbox2[key])
                            {
                                foreach (var pass in effect.CurrentTechnique.Passes)
                                {
                                    pass.Apply();
                                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, x, 0, x.Length / 2);
                                }
                            }
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

        private void LoadLevelsFromBig(params int[] absId)
        {
            if (big == null)
            {
                if (File.Exists("bigfile.big"))
                {
                    big = new BigFileReader(File.OpenRead("bigfile.big"));
                }
                else
                {
                    return;
                }
            }

            List<string> files = new List<string>();

            foreach (int i in absId)
            {
                big.FileCursor = i;

                if (Path.GetExtension(big.GetFilename()) != ".vrm")
                    return;

                Helpers.WriteToFile(Path.Combine(Meta.BasePath, big.GetFilename()), big.ReadFile());

                big.NextFile();

                if (Path.GetExtension(big.GetFilename()) != ".lev")
                    return;

                Helpers.WriteToFile(Path.Combine(Meta.BasePath, big.GetFilename()), big.ReadFile());

                files.Add(Path.Combine(Meta.BasePath, big.GetFilename()));
            }

            LoadStuff(files.ToArray());
        }

        protected override void Draw(GameTime gameTime)
        {
            //remember we're busy drawing stuff
            IsDrawing = true;

            GraphicsDevice.Clear(backColor);

            //graphics.GraphicsDevice.Viewport = vpFull;
            //DrawLevel();

            if (settings.StereoPair)
            {
                graphics.GraphicsDevice.Viewport = vpLeft;
                UpdateProjectionMatrices();
                DrawLevel(leftCamera);

                graphics.GraphicsDevice.Viewport = vpRight;
                UpdateProjectionMatrices();
                DrawLevel(rightCamera);
            }
            else
            {
                DrawLevel();
            }

            graphics.GraphicsDevice.Viewport = vpFull;
            UpdateProjectionMatrices();

            if (InMenu)
                menu.Draw(GraphicsDevice, spriteBatch, font, tint);


            spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

            if (InMenu)
            {
                spriteBatch.Draw(
                    textures["logo"],
                    new Vector2((graphics.GraphicsDevice.Viewport.Width / 2), 50 * graphics.GraphicsDevice.Viewport.Height / 1080f),
                    new Rectangle(0, 0, textures["logo"].Width, textures["logo"].Height),
                    Color.White,
                    0,
                    new Vector2(textures["logo"].Width / 2, 0),
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

            if (!gameLoaded)
                spriteBatch.DrawString(font, "LOADING...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString("LOADING...").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            if (scn.Count == 0 && gameLoaded)
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
                spriteBatch.DrawString(font, String.Format("FOV {0}", camera.ViewAngle.ToString("0.##")), new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(String.Format("FOV {0}", camera.ViewAngle.ToString("0.##"))).X - 20, 20), Color.Yellow);

            if (GamePad.GetState(0).Triggers.Left > 0 || GamePad.GetState(0).Triggers.Right > 0)
                spriteBatch.DrawString(
                    font,
                    $"Speed scale: {camera.speedScale.ToString("0.##")}",
                    new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString($"Speed scale: {camera.speedScale.ToString("0.##")}").X - 20, 20),
                    Color.Yellow);

            if (settings.ShowCamPos)
                spriteBatch.DrawString(font, $"({camera.Position.X.ToString("0.00")}, {camera.Position.Y.ToString("0.00")}, {camera.Position.Z.ToString("0.00")})", new Vector2(20, 20), Color.Yellow,
                    0,
                    Vector2.Zero,
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None,
                    0.5f);


            //spriteBatch.DrawString(font, String.Format("sp: {0}\r\nac:{1}", karts[0].Speed, karts[0].Accel), new Vector2(20, 20), Color.Yellow);

            if (settings.ShowConsole)
                GameConsole.Draw(graphics.GraphicsDevice, spriteBatch);

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
            alphalist.Clear();
            paths.Clear();
            textures.Clear();
            scn.Clear();
            MeshHigh.Clear();
            MeshLow.Clear();
            instanced.Clear();
            bbox.Clear();
            bbox2.Clear();

            sky = null;
        }

    }
}
