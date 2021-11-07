using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ctrviewer.Engine.Testing
{
    public class KartPhysics
    {
        public static float MaxAcceleration = 0.01f;
        public static float MaxSpeed = 0.25f;
        public static float MaxTurningStep = 0.025f;

        public static float Friction = 0.001f;
        public static float BrakeFriction = 0.01f;

        public static float TargetFps = 60;
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

        public float GetDelta(GameTime gameTime, float value)
        {
            return value * (float)gameTime.ElapsedGameTime.TotalMilliseconds * KartPhysics.TargetFps / 1000;
        }

        public void Update(GameTime gameTime)
        {
            GamePadState gs = GamePad.GetState(Game1.activeGamePad);
            KeyboardState ks = Keyboard.GetState();

            //move up/down

            if (ks.IsKeyDown(Keys.PageUp) || gs.Buttons.LeftShoulder == ButtonState.Pressed)
                Position.Y += GetDelta(gameTime, 0.1f);

            if (ks.IsKeyDown(Keys.PageDown) || gs.Buttons.RightShoulder == ButtonState.Pressed)
                Position.Y += GetDelta(gameTime, -0.1f);

            //turning

            if (ks.IsKeyDown(Keys.A) || gs.DPad.Left == ButtonState.Pressed)
                Rotation.X += GetDelta(gameTime, KartPhysics.MaxTurningStep * (Speed / KartPhysics.MaxSpeed) * (ks.IsKeyDown(Keys.S) ? 2 : 1));

            if (ks.IsKeyDown(Keys.D) || gs.DPad.Right == ButtonState.Pressed)
                Rotation.X -= GetDelta(gameTime, KartPhysics.MaxTurningStep * (Speed / KartPhysics.MaxSpeed) * (ks.IsKeyDown(Keys.S) ? 2 : 1));

            //udate accel value

            if (gs.IsButtonDown(Buttons.A) || ks.IsKeyDown(Keys.W) || gs.Buttons.A == ButtonState.Pressed)
            {
                Accel = KartPhysics.MaxAcceleration;
            }
            else
            {
                Accel = 0;
            }

            //update speed value

            Speed += GetDelta(gameTime, Accel - (ks.IsKeyDown(Keys.S) ? KartPhysics.BrakeFriction : KartPhysics.Friction));

            if (Speed > KartPhysics.MaxSpeed)
                Speed = KartPhysics.MaxSpeed;

            if (Speed < 0)
                Speed = 0;

            //move forward

            Position += Vector3.Transform(Vector3.Backward  * GetDelta(gameTime, Speed), Matrix.CreateRotationY(Rotation.X));
        }
    }
}