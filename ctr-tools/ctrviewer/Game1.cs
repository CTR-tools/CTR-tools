using CTRFramework;
using CTRFramework.Big;
using CTRFramework.Shared;
using CTRFramework.Sound;
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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static Dictionary<string, TriList> instTris = new System.Collections.Generic.Dictionary<string, TriList>();
        public static Dictionary<string, QuadList> instmodels = new System.Collections.Generic.Dictionary<string, QuadList>();

        List<InstancedModel> instanced = new List<InstancedModel>();
        List<InstancedModel> paths = new List<InstancedModel>();

        List<Kart> karts = new List<Kart>();


        List<VertexPositionColorTexture[]> bbox = new List<VertexPositionColorTexture[]>();

        Menu menu;

        //effects
        BasicEffect effect;
        BasicEffect instanceEffect;

        //cameras
        FirstPersonCamera camera;
        FirstPersonCamera lowcamera;
        FirstPersonCamera skycamera;

        //ctr scenes
        List<Scene> scn = new List<Scene>();

        //hi and low scenes converted to monogame
        List<MGLevel> levels = new List<MGLevel>();
        List<MGLevel> quads_low = new List<MGLevel>();

        //sky
        MGLevel sky;
        Color backColor = Color.Blue;


        public static PlayerIndex activeGamePad = PlayerIndex.One;


        //meh
        public static int currentflag = 1;

        public static string version = Meta.GetVersionInfo();


        public Game1()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            graphics.HardwareModeSwitch = false;
        }

        public void GoFullScreen()
        {
            graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        public void GoWindowed()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }



        protected override void Initialize()
        {
            graphics.PreferMultiSampling = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            graphics.ApplyChanges();
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;


            Samplers.Refresh();

            GoWindowed();

            IsMouseVisible = false;

            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;
            effect.DiffuseColor = new Vector3(2f, 2f, 2f);

            instanceEffect = new BasicEffect(graphics.GraphicsDevice);
            instanceEffect.VertexColorEnabled = true;
            instanceEffect.TextureEnabled = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new FirstPersonCamera(this);
            lowcamera = new FirstPersonCamera(this);
            skycamera = new FirstPersonCamera(this);

            DisableLodCamera();

            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                GamePadState state = GamePad.GetState(i);
                if (state.IsConnected)
                {
                    activeGamePad = i;
                    break;
                }
            }

            Samplers.InitRasterizers();

            base.Initialize();
        }


        private void EnableLodCamera()
        {
            lodEnabled = true;
            /*
            camera.NearClip = 1f;
            camera.FarClip = 10000f;
            lowcamera.NearClip = 9000f;
            lowcamera.FarClip = 50000f;
            */
            lowcamera.NearClip = 1f;
            lowcamera.FarClip = 100000f;
            camera.NearClip = 1f;
            camera.FarClip = 2f;

            camera.Update(null);
            lowcamera.Update(null);
        }

        private void DisableLodCamera()
        {
            lodEnabled = false;
            camera.NearClip = 1f;
            camera.FarClip = 100000f;
            lowcamera.NearClip = 1f;
            lowcamera.FarClip = 2f;
            camera.Update(null);
            lowcamera.Update(null);
        }



        // int currentCameraPosIndex = 0;

        Texture2D tint;

        protected override void LoadContent()
        {
            textures.Add("test", Content.Load<Texture2D>("test"));
            textures.Add("flag", Content.Load<Texture2D>("flag"));
            textures.Add("logo", Content.Load<Texture2D>("logo"));

            effect.Texture = textures["test"];
            effect.TextureEnabled = true;

            font = Content.Load<SpriteFont>("File");

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.Black });

            menu = new Menu(font);
            //graphics.GraphicsDevice.Viewport.Height / 2));

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
            QuadList modl = new QuadList();

            List<VertexPositionColorTexture> vptc = new List<VertexPositionColorTexture>();

            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, -10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(0, 0, 0), MGConverter.Blend(Color.Black, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            modl.PushQuad(vptc);

            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, 10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(0, 0, 0), MGConverter.Blend(Color.Black, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            modl.PushQuad(vptc);

            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, -10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(10, 50, 10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, -10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
            vptc.Add(new VertexPositionColorTexture(new Vector3(-10, 50, 10), MGConverter.Blend(Color.LightGray, c), new Vector2(0, 0)));
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


        private void LoadTextures(MGLevel qb)
        {


            foreach (string s in qb.textureList)
            {
                string path = String.Format("levels\\tex\\{0}.png", s);
                string path_new = String.Format("levels\\newtex\\{0}.png", s);

                if (File.Exists(path_new))
                    path = path_new;

                if (File.Exists(path))
                {
                    if (!textures.ContainsKey(s))
                    {
                        Texture2D t = Texture2D.FromStream(graphics.GraphicsDevice, File.OpenRead(path));
                        textures.Add(s, t);
                    }
                }
                else Console.WriteLine("Missing texture: " + s);
            }
        }


        private void LoadLevel(string[] lev)
        {
            if (lev == null)
                lev = new string[] { };

            paths.Clear();

            textures.Clear();
            textures.Add("test", Content.Load<Texture2D>("test"));
            textures.Add("flag", Content.Load<Texture2D>("flag"));
            textures.Add("logo", Content.Load<Texture2D>("logo"));

            scn.Clear();
            levels.Clear();
            quads_low.Clear();
            instanced.Clear();

            bbox.Clear();

            sky = null;

            GC.Collect();

            if (File.Exists("karts.lev"))
            {
                Scene karts = Scene.FromFile("karts.lev");

                foreach (DynamicModel m in karts.dynamics)
                {
                    if (!instTris.ContainsKey(m.Name) && m.Name == "selectkart")
                    {
                        List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

                        foreach (var x in m.headers[0].verts)
                            li.Add(MGConverter.ToVptc(x, new Vector2b(0, 0)));

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

            RenderEnabled = false;

            //wait for the end of frame
            while (IsDrawing) { };

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("LoadLevel()");

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
                Console.WriteLine("no files");
                return;
            }

            foreach (string s in files)
            {
                scn.Add(Scene.FromFile(s));
            }


            Console.WriteLine("scenes parsed at: " + sw.Elapsed.TotalSeconds);


            foreach (Scene s in scn)
            {
                levels.Add(new MGLevel(s, Detail.Med));
                quads_low.Add(new MGLevel(s, Detail.Low));
            }

            Console.WriteLine("converted scenes to monogame render at: " + sw.Elapsed.TotalSeconds);

            //force 1st scene sky and back color
            if (scn.Count > 0)
            {
                backColor = MGConverter.ToColor(scn[0].header.backColor);
                if (scn[0].skybox != null)
                    sky = new MGLevel(scn[0].skybox);
            }

            foreach (Scene s in scn)
            {
                if (s.unkadv != null)
                {
                    foreach (PosAng pa in s.unkadv.smth)
                        instanced.Add(new InstancedModel("limecone", new Vector3(pa.Position.X, pa.Position.Y, pa.Position.Z), Vector3.Zero, 3));
                }

                if (s.header.ptru2 != 0)
                {
                    foreach (Vector3s v in s.posu2)
                    {
                        instanced.Add(new InstancedModel("goldcone", new Vector3(v.X, v.Y, v.Z), Vector3.Zero, 3));
                    }
                }

                if (s.header.ptrTrialData != 0)
                {
                    foreach (PosAng v in s.posu1)
                    {
                        instanced.Add(new InstancedModel("browncone", new Vector3(v.Position.X, v.Position.Y, v.Position.Z), Vector3.Zero, 30));
                    }
                }
            }


            foreach (Scene s in scn)
                foreach (DynamicModel m in s.dynamics)
                {
                    if (!instTris.ContainsKey(m.Name))
                    {
                        List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

                        foreach (var x in m.headers[0].verts)
                            li.Add(MGConverter.ToVptc(x, new Vector2b(0, 0)));

                        TriList t = new TriList();
                        t.textureEnabled = false;
                        t.textureName = "test";
                        t.scrollingEnabled = false;
                        t.PushTri(li);
                        t.Seal();

                        instTris.Add(m.Name, t);
                    }

                }

            karts.Add(new Kart("selectkart", MGConverter.ToVector3(scn[0].header.startGrid[0].Position), Vector3.Left, 0.5f));


            Console.WriteLine("extracted dynamics at: " + sw.Elapsed.TotalSeconds);

            foreach (Scene s in scn)
            {
                foreach (PosAng pa in s.header.startGrid)
                    instanced.Add(new InstancedModel("purplecone", new Vector3(pa.Position.X, pa.Position.Y, pa.Position.Z), Vector3.Zero, 3));

                foreach (PickupHeader ph in s.pickups)
                    instanced.Add(new InstancedModel(
                        ph.ModelName,
                        new Vector3(ph.Position.X, ph.Position.Y, ph.Position.Z),
                        new Vector3((float)(ph.Angle.X / 4094f * Math.PI * 2), (float)(ph.Angle.Y / 4094f * Math.PI * 2), (float)(ph.Angle.Z / 4094f * Math.PI * 2)),
                        1));

                foreach (PosAng n in s.restartPts)
                    paths.Add(new InstancedModel("cyancone", new Vector3(n.Position.X, n.Position.Y, n.Position.Z), Vector3.Zero, 3));

                if (s.nav.paths.Count == 3)
                {
                    foreach (NavFrame n in s.nav.paths[0].frames)
                        paths.Add(new InstancedModel("greencone", new Vector3(n.position.X, n.position.Y, n.position.Z), Vector3.Zero, 3));
                    foreach (NavFrame n in s.nav.paths[1].frames)
                        paths.Add(new InstancedModel("yellowcone", new Vector3(n.position.X, n.position.Y, n.position.Z), Vector3.Zero, 3));
                    foreach (NavFrame n in s.nav.paths[2].frames)
                        paths.Add(new InstancedModel("redcone", new Vector3(n.position.X, n.position.Y, n.position.Z), Vector3.Zero, 3));
                }
            }



            foreach (Scene s in scn)
                s.ExportTexturesAll(Path.Combine(Meta.BasePath, "levels\\tex"));


            Console.WriteLine("textures extracted at: " + sw.Elapsed.TotalSeconds);

            //files = Directory.GetFiles("tex", "*.png");

            foreach (MGLevel q in levels) LoadTextures(q);
            foreach (MGLevel q in quads_low) LoadTextures(q);

            foreach (Scene s in scn)
            {
                foreach (var b in s.visdata)
                {
                    bbox.Add(MGConverter.ToLineList(b.bbox));
                }
            }

            sw.Stop();

            Console.WriteLine("textures loaded. level done: " + sw.Elapsed.TotalSeconds);

            RenderEnabled = true;
        }

        public void ResetCamera()
        {
            if (scn.Count > 0)
            {
                camera.Position = MGConverter.ToVector3(scn[0].header.startGrid[0].Position);
                lowcamera.Position = camera.Position;

                camera.SetRotation((float)(scn[0].header.startGrid[0].Angle.Y / 1024 * Math.PI * 2), scn[0].header.startGrid[0].Angle.X / 1024);
                lowcamera.SetRotation((float)(scn[0].header.startGrid[0].Angle.Y / 1024 * Math.PI * 2), scn[0].header.startGrid[0].Angle.X / 1024);
                skycamera.SetRotation((float)(scn[0].header.startGrid[0].Angle.Y / 1024 * Math.PI * 2), scn[0].header.startGrid[0].Angle.X / 1024);

                UpdateCameras(new GameTime());

                Console.WriteLine(scn[0].header.startGrid[0].Angle.ToString());
            }
        }

        protected override void UnloadContent()
        {
        }

        public bool renderVisBoxes = false;
        public bool updatemouse = false;
        public bool InMenu = false;
        public static bool HideInvisible = true;
        public static bool RenderEnabled = true;
        public static bool ControlsEnabled = true;
        public static bool IsDrawing = false;
        public bool lodEnabled = false;
        public bool show_inst = false;
        public bool show_paths = false;
        public bool lock_fps = true;

        GamePadState oldstate = GamePad.GetState(activeGamePad);
        GamePadState newstate = GamePad.GetState(activeGamePad);

        KeyboardState oldkb = new KeyboardState();
        KeyboardState newkb = new KeyboardState();

        protected override void Update(GameTime gameTime)
        {
            if (loading == null)
                LoadGame();

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

                if (Keyboard.GetState().IsKeyDown(Keys.RightAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (graphics.IsFullScreen) GoWindowed(); else GoFullScreen();
                }


                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
                {
                    float x = camera.ViewAngle;
                    x--;
                    if (x < 20) x = 20;

                    camera.ViewAngle = x;
                    lowcamera.ViewAngle = x;
                    skycamera.ViewAngle = x;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                {
                    float x = camera.ViewAngle;
                    x++;
                    if (x > 150) x = 150;

                    camera.ViewAngle = x;
                    lowcamera.ViewAngle = x;
                    skycamera.ViewAngle = x;
                }

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
                                LoadLevelFromBig(menu.SelectedItem.Value, 0, 2);
                                break;
                            case "loadbigadv":
                                LoadLevelFromBig(menu.SelectedItem.Value, 0, 3);
                                break;
                            case "link":
                                menu.SetMenu(font);
                                break;
                            case "toggle":
                                switch (menu.SelectedItem.Param)
                                {
                                    case "inst": show_inst = !show_inst; break;
                                    case "paths": show_paths = !show_paths; break;
                                    case "lod": lodEnabled = !lodEnabled; if (lodEnabled) EnableLodCamera(); else DisableLodCamera(); break;
                                    case "antialias": graphics.PreferMultiSampling = !graphics.PreferMultiSampling; break;
                                    case "invis": HideInvisible = !HideInvisible; break;
                                    case "visbox": renderVisBoxes = !renderVisBoxes; break;
                                    case "filter": Samplers.EnableBilinear = !Samplers.EnableBilinear; Samplers.Refresh(); break;
                                    case "wire": Samplers.EnableWireframe = !Samplers.EnableWireframe; break;
                                    case "window": if (graphics.IsFullScreen) GoWindowed(); else GoFullScreen(); break;
                                    case "lockfps":
                                        lock_fps = !lock_fps;
                                        graphics.SynchronizeWithVerticalRetrace = lock_fps;
                                        IsFixedTimeStep = lock_fps;
                                        graphics.ApplyChanges();
                                        break;
                                    default: Console.WriteLine("unimplemented toggle: " + menu.SelectedItem.Param); break;
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
                            Console.WriteLine(m.Action + " " + m.Title);
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
                    foreach (MGLevel mg in levels)
                        mg.Update(gameTime);

                    if (ControlsEnabled)
                        UpdateCameras(gameTime);
                }

                oldstate = newstate;
                oldkb = newkb;

            }

            base.Update(gameTime);
        }

        MouseState oldms = new MouseState();
        MouseState newms = new MouseState();

        private void UpdateCameras(GameTime gameTime)
        {
            oldms = newms;
            newms = Mouse.GetState();

            if (IsActive && newms.X >= 0 && newms.Y >= 0 && newms.LeftButton == ButtonState.Pressed)
            {
                IsMouseVisible = false;
                updatemouse = true;
                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            }
            else
            {
                IsMouseVisible = true;
                updatemouse = false;
            }

            skycamera.Update(gameTime, updatemouse, false, newms, oldms);
            camera.Update(gameTime, updatemouse, true, newms, oldms);
            lowcamera.Copy(gameTime, camera);

            newms = Mouse.GetState();
        }

        //public static bool twoSided = false;

        private void DrawLevel()
        {
            if (RenderEnabled)
            {
                //if (loading != null && gameLoaded)
                //{
                //if we have a sky
                if (sky != null)
                {
                    effect.View = skycamera.ViewMatrix;
                    effect.Projection = skycamera.ProjectionMatrix;

                    effect.DiffuseColor = new Vector3(1, 1, 1);
                    sky.RenderSky(graphics, effect);
                    effect.DiffuseColor = new Vector3(2.0f, 2.0f, 2.0f);
                }

                //clear z buffer
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);

                //render depending on lod
                if (lodEnabled)
                {
                    effect.View = lowcamera.ViewMatrix;
                    effect.Projection = lowcamera.ProjectionMatrix;

                    if (show_inst)
                    {
                        Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);
                        foreach (var v in instanced)
                            v.Render(graphics, instanceEffect, lowcamera);
                    }

                    if (show_paths)
                    {
                        Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);
                        foreach (var v in paths)
                            v.Render(graphics, instanceEffect, lowcamera);
                    }

                    Samplers.SetToDevice(graphics, EngineRasterizer.Default);

                    foreach (MGLevel qb in quads_low)
                        qb.Render(graphics, effect);

                    foreach (Kart k in karts)
                        k.Render(graphics, instanceEffect, lowcamera);

                }
                else
                {
                    if (show_inst)
                    {
                        Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);
                        foreach (var v in instanced)
                            v.Render(graphics, instanceEffect, camera);
                    }

                    if (show_paths)
                    {
                        Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);
                        foreach (var v in paths)
                            v.Render(graphics, instanceEffect, camera);
                    }

                    Samplers.SetToDevice(graphics, EngineRasterizer.Default);

                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;

                    foreach (MGLevel qb in levels)
                        qb.Render(graphics, effect);

                    foreach (Kart k in karts)
                        k.Render(graphics, instanceEffect, camera);

                }

                if (renderVisBoxes)
                {
                    //GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);

                    foreach (var x in bbox)
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

        private void LoadLevelFromBig(int levelId = 0, int mode = 0, int files = 2)
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

            if (levelId == -1 && files == 3)
            {
                string[] levels = new string[5];

                for (int i = 0; i < 5; i++)
                {
                    big.FileCursor = 200 + i * 3;

                    Directory.CreateDirectory(Path.Combine(Meta.BasePath, Directory.GetParent(big.GetFilename()).FullName));
                    File.WriteAllBytes(Path.Combine(Meta.BasePath, big.GetFilename()), big.ReadFile());

                    big.NextFile();

                    levels[i] = Path.Combine(Meta.BasePath, big.GetFilename());
                    File.WriteAllBytes(levels[i], big.ReadFile());
                }

                LoadStuff(levels);
            }
            else
            {
                big.FileCursor = (files == 3 ? 200 + levelId * 3 : levelId * 8) + mode * files;

                Directory.CreateDirectory(Path.Combine(Meta.BasePath, Directory.GetParent(big.GetFilename()).FullName));

                File.WriteAllBytes(Path.Combine(Meta.BasePath, big.GetFilename()), big.ReadFile());

                big.NextFile();
                File.WriteAllBytes(Path.Combine(Meta.BasePath, big.GetFilename()), big.ReadFile());

                LoadStuff(new string[] { Path.Combine(Meta.BasePath, big.GetFilename()) });
            }

            if (howl == null)
            {
                if (File.Exists("kart.hwl"))
                {
                    howl = Howl.FromFile("kart.hwl");
                }
                else
                {
                    return;
                }
            }

            //howl.ExportAllSamples();
        }



        protected override void Draw(GameTime gameTime)
        {
            IsDrawing = true;

            GraphicsDevice.Clear(backColor);

            DrawLevel();

            if (InMenu)
                menu.Render(GraphicsDevice, spriteBatch, font, tint);


            spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

            if (InMenu)
            {
                spriteBatch.Draw(textures["logo"], new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - textures["logo"].Width / 2, 50), Color.White);

                spriteBatch.DrawString(
                    font,
                    version,
                    new Vector2(((graphics.PreferredBackBufferWidth - font.MeasureString(version).X * graphics.GraphicsDevice.Viewport.Height / 1080f) / 2), graphics.PreferredBackBufferHeight - 60),
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
                spriteBatch.DrawString(font, "No levels loaded.\r\nPut LEV/VRM files in levels folder.\r\n...or put BIGFILE.BIG in root folder\r\nand use load level menu.".ToString(), new Vector2(20, 60), Color.Yellow);

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) || Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                spriteBatch.DrawString(font, String.Format("FOV {0}", camera.ViewAngle.ToString("0.##")), new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(String.Format("FOV {0}", camera.ViewAngle.ToString("0.##"))).X - 20, 20), Color.Yellow);

            //spriteBatch.DrawString(font, IsActive ? "Active" : "Not active", new Vector2(20, 20), Color.Yellow);

            //spriteBatch.DrawString(font, String.Format("sp: {0}\r\nac:{1}", karts[0].Speed, karts[0].Accel), new Vector2(20, 20), Color.Yellow);

            spriteBatch.End();


            base.Draw(gameTime);

            IsDrawing = false;
        }


    }
}
