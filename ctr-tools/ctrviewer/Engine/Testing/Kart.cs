using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ctrviewer.Engine.Testing
{
    class Kart : InstancedModel
    {
        public Vector3 Position;
        public Vector3 Angle;
        public float Scale;
        public string ModelName = "bluecone";

        public float Speed = 0;
        public float Accel = 0;

        public static float MaxAccel = 1;
        public static float MaxSpeed = 20;

        public Kart()
        {
        }

        public Kart(Vector3 pos, Vector3 ang)
        {
            Position = pos;
            Angle = ang;
        }

        public Kart(string mn, Vector3 pos, Vector3 ang, float scale)
        {
            ModelName = mn;
            Position = pos;
            Angle = ang;
            Scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            GamePadState gs = GamePad.GetState(Game1.activeGamePad);

            if (gs.IsButtonDown(Buttons.A))
            {
                if (Accel != MaxAccel)
                {
                    Accel += 0.01f;

                    if (Accel > MaxAccel)
                        Accel = MaxAccel;
                }

                Speed += Accel;
            }
            else
            {
                Accel = 0;

                Speed -= 0.1f;
            }

            if (Speed > MaxSpeed)
                Speed = MaxSpeed;

            if (Speed < 0)
                Speed = 0;

            Position += Vector3.Left * Speed;

            //Position += new Vector3(-1f, 0, 0);

            if (Speed == 0)
                Accel = 0;
        }


        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha, FirstPersonCamera camera)
        {
            effect.World = Matrix.CreateScale(Scale) * Matrix.CreateFromYawPitchRoll(Angle.X, Angle.Y, Angle.Z) * Matrix.CreateTranslation(Position);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            if (ContentVault.Models.ContainsKey(ModelName))
            {
                ContentVault.Models[ModelName].Draw(graphics, effect, alpha);
            }
            else if (ContentVault.Tris.ContainsKey(ModelName))
            {
                ContentVault.Tris[ModelName].Draw(graphics, effect, alpha);
            }
            else
            {
                Console.WriteLine(ModelName + " not loaded");
            }
        }
    }
}
