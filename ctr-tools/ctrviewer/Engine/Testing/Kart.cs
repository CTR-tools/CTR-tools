using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ctrviewer.Engine.Testing
{
    public class Physics
    {
        public static float MaxAcceleration = 0.01f;
        public static float MaxSpeed = 0.25f;
        public static float MaxTurningStep = 0.025f;

        public static float Friction = 0.001f;
        public static float BrakeFriction = 0.01f;
    }

    class Kart : InstancedModel
    { 
        public float Speed = 0;
        public float Accel = 0;

        public Kart()
        {
        }

        public Kart(Vector3 pos, Vector3 rot)
        {
            ModelName = "selectkart";
            Position = pos;
            Rotation = rot;
        }

        public void Update(GameTime gameTime)
        {
            GamePadState gs = GamePad.GetState(Game1.activeGamePad);
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Up))
                Position.Y += 0.1f;

            if (ks.IsKeyDown(Keys.Down))
                Position.Y += -0.1f;

            if (ks.IsKeyDown(Keys.A))
                Rotation.X += Physics.MaxTurningStep * (Speed / Physics.MaxSpeed) * (ks.IsKeyDown(Keys.S) ? 2 : 1);

            if (ks.IsKeyDown(Keys.D))
                Rotation.X -= Physics.MaxTurningStep * (Speed / Physics.MaxSpeed) * (ks.IsKeyDown(Keys.S) ? 2 : 1);
            

            if (gs.IsButtonDown(Buttons.A) || ks.IsKeyDown(Keys.W))
            {
                Accel = Physics.MaxAcceleration;
            }
            else
            {
                Accel = 0;
            }

            Speed += Accel - (ks.IsKeyDown(Keys.S) ? Physics.BrakeFriction : Physics.Friction);

            if (Speed > Physics.MaxSpeed)
                Speed = Physics.MaxSpeed;

            if (Speed < 0)
                Speed = 0;

            Position += Vector3.Transform(Vector3.Backward  * Speed, Matrix.CreateRotationY(Rotation.X));
        }
    }
}
