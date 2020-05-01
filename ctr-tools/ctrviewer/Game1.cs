using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using CTRFramework.Shared;

namespace ctrviewer
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        List<Scene> scn = new List<Scene>();
        Menu menu;

        BasicEffect effect;
        BasicEffect instanceEffect;
        FirstPersonCamera camera;
        FirstPersonCamera lowcamera;
        FirstPersonCamera skycamera;


        List<MGQuadBlock> quads = new List<MGQuadBlock>();
        List<MGQuadBlock> quads_low = new List<MGQuadBlock>();
        MGQuadBlock sky;

        Color backColor = Color.Blue;

        public static SamplerState ss;
        public static PlayerIndex activeGamePad = PlayerIndex.One;


        public static int currentflag = 1;

        public Game1()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferMultiSampling = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;

            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            /*
                = new BlendState()
            {
                ColorSourceBlend = Blend.SourceColor,
                ColorDestinationBlend = Blend.DestinationColor,
                ColorBlendFunction = BlendFunction.Add
            };
            */

            GoWindowed();

            IsMouseVisible = false;
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
            ss = new SamplerState();

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

        private void LoadStuff()
        {
            gameLoaded = false;

            LoadLevel((TerrainFlags)(0));
            ResetCamera();

            gameLoaded = true;
        }


        private void LoadTextures(MGQuadBlock qb)
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
                        textures.Add(s, Texture2D.FromStream(graphics.GraphicsDevice, File.OpenRead(path)));
                }
                else Console.WriteLine("Missing texture: " + s);
            }
        }

        private void LoadLevel(TerrainFlags qf)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            scn.Clear();
            quads.Clear();
            quads_low.Clear();

            string[] files = new string[] { };

            if (Directory.Exists(@"levels\"))
                files = Directory.GetFiles(@"levels\", "*.lev");

            foreach (string s in files)
                scn.Add(new Scene(s, "obj"));

            Console.WriteLine("scenes loaded: " + sw.Elapsed.TotalSeconds);

            foreach (Scene s in scn)
                quads.Add(new MGQuadBlock(s, Detail.Med));

            foreach (Scene s in scn)
                quads_low.Add(new MGQuadBlock(s, Detail.Low));

            Console.WriteLine("scenes converted to mg: " + sw.Elapsed.TotalSeconds);

            
            if (scn.Count > 0)
            {
                backColor = MGConverter.ToColor(scn[0].header.backColor) ;
                if (scn[0].skybox != null)
                    sky = new MGQuadBlock(scn[0].skybox);
            }

            foreach (Scene s in scn)
                foreach (PickupHeader ph in s.pickups)
                    instanced.Add(new InstancedModel("purplecone", new Vector3(ph.Position.X, ph.Position.Y, ph.Position.Z), 3));
            
            foreach (Scene s in scn)
            {
                foreach (PosAng n in s.restartPts)
                    instanced.Add(new InstancedModel("cyancone", new Vector3(n.Position.X, n.Position.Y, n.Position.Z), 3));
            }


            foreach (Scene s in scn)
            {
                if (s.nav.paths.Count == 3)
                {
                    foreach (NavFrame n in s.nav.paths[0].frames)
                        instanced.Add(new InstancedModel("greencone", new Vector3(n.position.X, n.position.Y, n.position.Z), 3));
                    foreach (NavFrame n in s.nav.paths[1].frames)
                        instanced.Add(new InstancedModel("yellowcone", new Vector3(n.position.X, n.position.Y, n.position.Z), 3));
                    foreach (NavFrame n in s.nav.paths[2].frames)
                        instanced.Add(new InstancedModel("redcone", new Vector3(n.position.X, n.position.Y, n.position.Z), 3));
                }
            }
            





            scn[0].ExportTextures("levels\\tex");

            //files = Directory.GetFiles("tex", "*.png");

            foreach (MGQuadBlock q in quads) LoadTextures(q);
            foreach (MGQuadBlock q in quads_low) LoadTextures(q);

            sw.Stop();
            Console.WriteLine("loading done: " + sw.Elapsed.TotalSeconds);
        }

        public void ResetCamera()
        {
            if (scn.Count > 0)
            {
                camera.Position = MGConverter.ToVector3(scn[0].header.startGrid[0].Position);
                lowcamera.Position = camera.Position;

                camera.SetRotation((float)(scn[0].header.startGrid[0].Angle.Y / 1024 * Math.PI * 2), scn[0].header.startGrid[0].Angle.X / 1024);
                lowcamera.SetRotation((float)(scn[0].header.startGrid[0].Angle.Y / 1024 * Math.PI * 2), scn[0].header.startGrid[0].Angle.X / 1024);

                Console.WriteLine(scn[0].header.startGrid[0].Angle.ToString());
            }
        }

        protected override void UnloadContent()
        {
        }

        public bool usemouse = false;
        public bool wire = false;
        public bool inmenu = false;
        public bool hide_invis = true;
        public static bool filter = true;
        public static bool clamp = true;
        public bool lodEnabled = false;
        public bool show_inst = false;

        GamePadState oldstate = GamePad.GetState(activeGamePad);
        GamePadState newstate = GamePad.GetState(activeGamePad);

        protected override void Update(GameTime gameTime)
        {

            //x += 0.01f ;
            //if (x > Math.PI * 2)
            //    x = 0;
            //camera.SetRotation(x, y);
            //Console.WriteLine(x);

            newstate = GamePad.GetState(activeGamePad);

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
                lowcamera.ViewAngle=x ;
                skycamera.ViewAngle=x;
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




            if (newstate.Buttons.Start == ButtonState.Pressed && oldstate.Buttons.Start != newstate.Buttons.Start || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                inmenu = !inmenu;
            }

            if (inmenu)
            {
                menu.Update(oldstate, newstate);

                //currentflag = menu.items.Find(x => x.Title == "current flag: {0}").rangeval;

                if (menu.Exec)
                {
                    switch (menu.SelectedItem.Action)
                    {
                        case "close":
                            inmenu = false;
                            break;
                        case "load": 
                            LoadGame(); 
                            ResetCamera(); 
                            inmenu = false; 
                            break;
                        case "toggle":
                            switch (menu.SelectedItem.Param)
                            {
                                case "inst": show_inst = !show_inst; break;
                                case "lod": lodEnabled = !lodEnabled; if (lodEnabled) EnableLodCamera(); else DisableLodCamera(); break;
                                case "antialias": graphics.PreferMultiSampling = !graphics.PreferMultiSampling; break;
                                //case "invis": hide_invis = !hide_invis; LoadLevel((TerrainFlags)(1 << Flag)); break;
                                case "mouse": usemouse = !usemouse; break;
                                case "filter": filter = !filter; break;
                                case "wire": wire = !wire; break;
                                case "window": if (graphics.IsFullScreen) GoWindowed(); else GoFullScreen(); break;
                            }
                            break;

                        case "exit":
                            Exit();
                            break;
                    }

                    menu.Exec = !menu.Exec;
                }

                if (newstate.Buttons.B == ButtonState.Pressed)
                {
                    inmenu = !inmenu;
                }
            }
            else
            {
                foreach(MGQuadBlock mg in quads)
                {
                    mg.Update();
                }
                UpdateCameras(gameTime);
            }

            oldstate = newstate;


            base.Update(gameTime);
        }

        MouseState oldms;
        MouseState newms;

        private void UpdateCameras(GameTime gameTime)
        {
            newms = Mouse.GetState();
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            skycamera.Update(gameTime, usemouse, false, newms, oldms);

            camera.Update(gameTime, usemouse, true, newms, oldms);
            lowcamera.Copy(gameTime, camera);


            oldms = Mouse.GetState();

        }

        public static bool twoSided = false;

        public static void UpdateSamplerState(GraphicsDeviceManager g)
        {
            //ss = SamplerState.PointClamp;
            ss = new SamplerState();
            ss.FilterMode = TextureFilterMode.Default;

            ss.Filter = filter ? TextureFilter.Anisotropic : TextureFilter.Point;
            ss.MaxAnisotropy = 16;

            if (clamp)
            {
                ss.AddressU = TextureAddressMode.Clamp;
                ss.AddressV = TextureAddressMode.Clamp;
            }
            else
            {
                //only use wrap on V, cause it only scrolls on V.
                //this helps to avoid seams on animated quads.
                ss.AddressU = TextureAddressMode.Clamp;
                ss.AddressV = TextureAddressMode.Wrap;
            }

            ss.MaxMipLevel = 8;
            ss.MipMapLevelOfDetailBias = 0;

            g.GraphicsDevice.SamplerStates[0] = ss;
        }

        private void DrawLevel()
        {
            UpdateSamplerState(graphics);
            graphics.ApplyChanges();

            if (loading != null && gameLoaded)
            {
                if (sky != null)
                {
                    effect.View = skycamera.ViewMatrix;
                    effect.Projection = skycamera.ProjectionMatrix;

                    effect.DiffuseColor = new Vector3(1, 1, 1);
                    sky.RenderSky(graphics, effect);
                    effect.DiffuseColor = new Vector3(2.0f, 2.0f, 2.0f);

                    if (wire)
                    {
                        WireframeMode(graphics.GraphicsDevice, true);
                        sky.RenderWire(graphics, effect);
                        WireframeMode(graphics.GraphicsDevice, false);
                    }
                }


                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);


                effect.View = lowcamera.ViewMatrix;
                effect.Projection = lowcamera.ProjectionMatrix;

                foreach (MGQuadBlock qb in quads_low)
                    qb.Render(graphics, effect);

                if (wire)
                {
                    WireframeMode(graphics.GraphicsDevice, true);

                    foreach (MGQuadBlock qb in quads_low)
                        qb.RenderWire(graphics, effect);

                    WireframeMode(graphics.GraphicsDevice, false);
                }


                if (show_inst)
                    foreach (var v in instanced)
                        v.Render(graphics, instanceEffect, lowcamera);

                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);



                effect.View = camera.ViewMatrix;
                effect.Projection = camera.ProjectionMatrix;


                foreach (MGQuadBlock qb in quads)
                    qb.Render(graphics, effect);

                if (wire)
                {
                    WireframeMode(graphics.GraphicsDevice, true);

                    foreach (MGQuadBlock qb in quads)
                        qb.RenderWire(graphics, effect);

                    WireframeMode(graphics.GraphicsDevice, false);
                }

                if (show_inst)
                    foreach (var v in instanced)
                        v.Render(graphics, instanceEffect, camera);

            }
            else
            {
                if (loading == null)
                {
                    LoadGame();
                }
            }
        }


        Task loading;

        private void LoadGame()
        {
            loading = Task.Run(() => LoadStuff());
            //loading.Wait();
        }


        RasterizerState rasterizerState;



        public void WireframeMode(GraphicsDevice gd, bool toggle)
        {
            rasterizerState = new RasterizerState();
            rasterizerState.FillMode = (toggle ? FillMode.WireFrame : FillMode.Solid);
            rasterizerState.CullMode = (toggle ? CullMode.None : CullMode.CullCounterClockwiseFace);

            if (gd.RasterizerState != rasterizerState)
                gd.RasterizerState = rasterizerState;
        }

        public static Dictionary<string, QuadList> instmodels = new System.Collections.Generic.Dictionary<string, QuadList>();
        List<InstancedModel> instanced = new List<InstancedModel>();

        protected override void Draw(GameTime gameTime)
        {
            // graphics.BeginDraw();

            GraphicsDevice.Clear(backColor);

            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            DrawLevel();


          //  WireframeMode(GraphicsDevice, false);

            if (inmenu)
            {
                menu.Render(GraphicsDevice, spriteBatch, font, tint);
            }

            spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

            if (inmenu)
            {
                spriteBatch.DrawString(
                    font, 
                    Meta.GetVersion(), 
                    new Vector2(((graphics.PreferredBackBufferWidth - font.MeasureString(Meta.GetVersion()).X * graphics.GraphicsDevice.Viewport.Height / 1080f) / 2), graphics.PreferredBackBufferHeight - 60),
                    Color.Aquamarine,
                    0, 
                    new Vector2(0, 0),
                    graphics.GraphicsDevice.Viewport.Height / 1080f,
                    SpriteEffects.None, 
                     0.5f
                    );
            }

            //spriteBatch.DrawString(font, (1 << flag).ToString("X4") + ": " + ((TerrainFlags)(1 << flag)).ToString(), new Vector2(20, 20), Color.Yellow);

            if (!gameLoaded)
                spriteBatch.DrawString(font, "LOADING...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString("LOADING...").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            if (scn.Count == 0)
                spriteBatch.DrawString(font, "No levels loaded. Put LEV files in levels folder.".ToString(), new Vector2(20, 60), Color.Yellow);

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) || Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                spriteBatch.DrawString(font, String.Format("FOV {0}", camera.ViewAngle.ToString("0.##")), new Vector2(graphics.PreferredBackBufferWidth - font.MeasureString(String.Format("FOV {0}", camera.ViewAngle.ToString("0.##"))).X - 20, 20), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);

            // graphics.EndDraw();
        }


    }
}
