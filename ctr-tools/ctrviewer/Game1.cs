using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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

        List<Scene> scn = new List<Scene>();
        Menu menu;

        BasicEffect effect;
        FirstPersonCamera camera;
        FirstPersonCamera lowcamera;
        FirstPersonCamera skycamera;

        //List<VertexPositionColor> verts = new List<VertexPositionColor>();
        List<MGQuadBlock> quads = new List<MGQuadBlock>();
        List<MGQuadBlock> quads_low = new List<MGQuadBlock>();
        MGQuadBlock sky;

        Color backColor = Color.Blue;

        private short flag = 0;
        public short Flag
        {
            get
            {
                return flag;
            }
            set
            {
                flag = value;
                if (flag > 7) flag = 0;
                if (flag < 0) flag = 7;
            }
        }


        public Game1()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            GoWindowed();

            graphics.PreferMultiSampling = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;


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

        SamplerState ss;

        protected override void Initialize()
        {
            ss = new SamplerState();

            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            graphics.ApplyChanges();

            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new FirstPersonCamera(this);
            lowcamera = new FirstPersonCamera(this);
            skycamera = new FirstPersonCamera(this);

            camera.NearClip = 0.1f;
            camera.FarClip = 3000;
            lowcamera.NearClip = 2000;
            lowcamera.FarClip = 70000;

            base.Initialize();
        }


        int currentCameraPosIndex = 0;

        Texture2D tint;

        protected override void LoadContent()
        {
            //textures.Add("test", Content.Load<Texture2D>("test"));
            //effect.Texture = textures["test"];
            effect.TextureEnabled = false;

            font = Content.Load<SpriteFont>("File");

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData(new Color[] { Color.Black });

            menu = new Menu(font);
            //graphics.GraphicsDevice.Viewport.Height / 2));
        }

        bool gameLoaded = false;

        private void LoadStuff()
        {
            gameLoaded = false;

            LoadLevel((TerrainFlags)(1 << flag));
            ResetCamera();

            gameLoaded = true;
        }


        public Microsoft.Xna.Framework.Vector3 toMGv2(CTRFramework.Shared.Vector3s v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }



        private void LoadLevel(TerrainFlags qf)
        {
            scn.Clear();
            quads.Clear();

            string[] files = new string[] { };

            if (Directory.Exists(@"levels\"))
                files = Directory.GetFiles(@"levels\", "*.lev");

            foreach (string s in files)
                scn.Add(new Scene(s, "obj"));

            foreach (Scene s in scn)
            {
                quads.Add(new MGQuadBlock(s, Detail.Med));
            }

            foreach (Scene s in scn)
            {
                quads_low.Add(new MGQuadBlock(s, Detail.Low));
            }

            // quads.Add(new MGQuadBlock(s, i++, qf, hide_invis));

            if (scn.Count > 0)
            {
                backColor.R = scn[0].header.backColor.X;
                backColor.G = scn[0].header.backColor.Y;
                backColor.B = scn[0].header.backColor.Z;

                if (scn[0].skybox != null)
                    sky = new MGQuadBlock(scn[0].skybox);
            }


            foreach (Scene s in scn)
            {
                //s.ExportTextures(@".\tex\");

                foreach (var x in s.ctrvram.textures)
                {
                    if (!textures.ContainsKey(x.Key))
                        textures.Add(x.Key, Game1.GetTexture(GraphicsDevice, x.Value));
                }

            }

            // effect.Texture = textures["test"];

        }

        public void ResetCamera()
        {
            if (scn.Count > 0)
            {
                camera.Position = new Vector3(
                    scn[0].header.startGrid[0].Position.X,
                    scn[0].header.startGrid[0].Position.Y,
                    scn[0].header.startGrid[0].Position.Z
                    );

                lowcamera.Position = camera.Position;
            }
        }

        protected override void UnloadContent()
        {
        }

        public bool usemouse = false;
        public bool wire = true;
        public bool inmenu = false;
        public bool hide_invis = true;
        public bool filter = true;

        GamePadState oldstate = GamePad.GetState(PlayerIndex.One);
        GamePadState newstate = GamePad.GetState(PlayerIndex.One);


        protected override void Update(GameTime gameTime)
        {
            newstate = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.RightAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (graphics.IsFullScreen) GoWindowed(); else GoFullScreen();
            }


            if (newstate.Buttons.Start == ButtonState.Pressed && oldstate.Buttons.Start != newstate.Buttons.Start)
            {
                inmenu = !inmenu;
            }

            if (inmenu)
            {
                menu.Update(oldstate, newstate);

                if (menu.Exec)
                {
                    switch (menu.SelectedItem.Action)
                    {
                        case "load": LoadGame(); ResetCamera(); inmenu = false; break;
                        case "flag":
                            switch (menu.SelectedItem.Param)
                            {
                                case "next": Flag++; break;
                                case "prev": Flag--; break;
                            }
                            LoadLevel((TerrainFlags)(1 << flag));
                            break;

                        case "toggle":
                            switch (menu.SelectedItem.Param)
                            {

                                case "antialias":
                                    graphics.PreferMultiSampling = !graphics.PreferMultiSampling;
                                    break;

                                case "invis": hide_invis = !hide_invis; LoadLevel((TerrainFlags)(1 << Flag)); break;
                                case "mouse": usemouse = !usemouse; break;
                                case "filter": filter = !filter; break;
                                case "wire": wire = !wire; break;
                                case "window":
                                    if (graphics.IsFullScreen) GoWindowed(); else GoFullScreen();
                                    break;

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
                camera.Update(gameTime, usemouse, true);
                skycamera.Update(gameTime, usemouse, false);
                lowcamera.Update(gameTime, usemouse, true);
                lowcamera.Position = camera.Position;
            }

            oldstate = newstate;


            base.Update(gameTime);
        }



        private SamplerState GetCurrentSampler()
        {
            ss = new SamplerState();
            ss.FilterMode = TextureFilterMode.Default;

            ss.Filter = filter ? TextureFilter.Anisotropic : TextureFilter.Point;
            ss.MaxAnisotropy = 16;
            //ss.MaxMipLevel = 8;
            //ss.MipMapLevelOfDetailBias = -1.5f;

            return ss;
        }

        private void DrawLevel()
        {
            graphics.GraphicsDevice.SamplerStates[0] = GetCurrentSampler();
            graphics.ApplyChanges();

            if (loading != null && gameLoaded)
            {
                if (sky != null)
                {
                    effect.View = skycamera.ViewMatrix;
                    effect.Projection = skycamera.ProjectionMatrix;

                    sky.Render(graphics, effect);

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

        protected override void Draw(GameTime gameTime)
        {
            // graphics.BeginDraw();

            GraphicsDevice.Clear(backColor);

            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            DrawLevel();

            if (inmenu)
            {
                menu.Render(GraphicsDevice, spriteBatch, font, tint);
            }

            spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

            spriteBatch.DrawString(font, (1 << flag).ToString("X4") + ": " + ((TerrainFlags)(1 << flag)).ToString(), new Vector2(20, 20), Color.Yellow);

            if (!gameLoaded)
                spriteBatch.DrawString(font, "LOADING...", new Vector2(graphics.PreferredBackBufferWidth / 2 - (font.MeasureString("LOADING...").X / 2), graphics.PreferredBackBufferHeight / 2), Color.Yellow);

            if (scn.Count == 0)
                spriteBatch.DrawString(font, "No levels loaded. Put LEV files in levels folder.".ToString(), new Vector2(20, 60), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);

            // graphics.EndDraw();
        }


        //magic
        public static Texture2D GetTexture(GraphicsDevice gd, System.Drawing.Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(gd, bmp.Width, bmp.Height);

            // lock bitmap
            System.Drawing.Imaging.BitmapData origdata =
                bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            //uint* byteData = (uint*)origdata.Scan0;
            /*
            // Switch bgra -> rgba
            for (int i = 0; i < imgData.Length; i++)
            {
                byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
            }
            */
            // copy data
            System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

            //byteData = null;

            // unlock bitmap
            bmp.UnlockBits(origdata);

            texture.SetData(imgData);

            return texture;
        }

    }
}
