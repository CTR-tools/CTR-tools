using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace viewer
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        List<Scene> scn = new List<Scene>();
        Menu menu;

        BasicEffect effect;
        FirstPersonCamera camera;

        //List<VertexPositionColor> verts = new List<VertexPositionColor>();
        List<MGQuadBlock> quads = new List<MGQuadBlock>();

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

            GoFullScreen();

            graphics.PreferMultiSampling = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = false;
        }

        public void GoFullScreen()
        {
            graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            graphics.IsFullScreen = true;
        }
        public void GoWindowed()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            graphics.ApplyChanges();

            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;

            camera = new FirstPersonCamera(this);

            base.Initialize();
        }


        int currentCameraPosIndex = 0;

        Texture2D tint;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("File");

            tint = new Texture2D(GraphicsDevice, 1, 1);
            tint.SetData( new Color[] { Color.Black } );

            menu = new Menu(font);
                //graphics.GraphicsDevice.Viewport.Height / 2));

            LoadLevel((TerrainFlags) (1 << flag));
            ResetCamera();
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
                scn.Add(new Scene(s, "obj", null));

            int i = 0;
            foreach (Scene s in scn)
                quads.Add(new MGQuadBlock(s, i++, qf, hide_invis));

            if (scn.Count > 0)
            {
                backColor.R = scn[0].header.backColor.X;
                backColor.G = scn[0].header.backColor.Y;
                backColor.B = scn[0].header.backColor.Z;
            }

        }

        public void ResetCamera()
        {
            if (scn.Count > 0)
                camera.Position = new Vector3(
                    scn[0].header.startGrid[0].Position.X,
                    scn[0].header.startGrid[0].Position.Y,
                    scn[0].header.startGrid[0].Position.Z
                    );
        }

        protected override void UnloadContent()
        {
        }

        public bool usemouse = false;
        public bool wire = true;
        public bool inmenu = false;
        public bool hide_invis = true;

        GamePadState oldstate = GamePad.GetState(PlayerIndex.One);
        GamePadState newstate = GamePad.GetState(PlayerIndex.One);


        protected override void Update(GameTime gameTime)
        {
            newstate = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
                        case "load": LoadLevel((TerrainFlags)(1 << flag)); ResetCamera(); inmenu = false;  break;
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
                                /*
                                case "antialias":
                                    if (graphics.GraphicsProfile != GraphicsProfile.HiDef)
                                    {
                                        graphics.GraphicsProfile = GraphicsProfile.HiDef;
                                    }
                                    else
                                    {
                                        graphics.GraphicsProfile = GraphicsProfile.Reach;
                                    }
                                    graphics.ApplyChanges();
                                    break;*/

                                case "invis": hide_invis = !hide_invis; LoadLevel((TerrainFlags)(1 << Flag)); break;
                                case "mouse": usemouse = !usemouse; break;
                                case "wire": wire = !wire; break;
                                case "window":
                                    if (graphics.IsFullScreen) GoWindowed(); else GoFullScreen();
                                    graphics.ApplyChanges();
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
                camera.Update(gameTime, usemouse);
            }

            oldstate = newstate;


            base.Update(gameTime);
        }


        private void DrawLevel()
        {
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

        RasterizerState rasterizerState;

        public void WireframeMode(GraphicsDevice gd, bool toggle)
        {
            rasterizerState = new RasterizerState();
            rasterizerState.FillMode = (toggle ? FillMode.WireFrame : FillMode.Solid);
            rasterizerState.CullMode = (toggle ? CullMode.None : CullMode.CullCounterClockwiseFace);

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

            spriteBatch.DrawString(font, (1 << flag).ToString("X4") + ": " + ((TerrainFlags)(1<<flag)).ToString(), new Vector2(20, 20), Color.Yellow);
            if (scn.Count == 0)
                spriteBatch.DrawString(font, "No levels loaded. Put LEV files in levels folder.".ToString(), new Vector2(20, 60), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);

           // graphics.EndDraw();
        }
    }
}
