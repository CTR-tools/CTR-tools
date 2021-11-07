using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using CTRFramework;
using CTRFramework.Shared;

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

        public static float GravityStep = 0.01f;
        public static float MaxGravity = 1f;
    }

    class Kart : InstancedModel
    { 
        public float Speed = 0;
        public float Accel = 0;
        public float Gravity = 0;

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

        public void Collide(List<QuadBlock> quads)
        {
            foreach (var quad in quads)
            {
                if (quad.quadFlags.HasFlag(QuadFlags.Ground) || quad.quadFlags.HasFlag(QuadFlags.Wall))
                if (
                    (quad.bb.numericMin.X <= Position.X) &&
                    (quad.bb.numericMin.Y-1 <= Position.Y) &&
                    (quad.bb.numericMin.Z <= Position.Z) &&
                    (quad.bb.numericMax.X >= Position.X) &&
                    (quad.bb.numericMax.Y+2 >= Position.Y) &&
                    (quad.bb.numericMax.Z >= Position.Z)
                    )
                {
                    GameConsole.Write($"collide with quad bb: {quad.bb} at {Position}");
                        if (Position.Y <= quad.bb.numericMax.Y)
                        {
                            Position.Y = quad.bb.numericMax.Y;
                            Gravity = 0;
                            return;
                        }
                }
            }
        }

        public void Update(GameTime gameTime, List<QuadBlock> quads)
        {
            GamePadState gs = GamePad.GetState(Game1.activeGamePad);
            KeyboardState ks = Keyboard.GetState();

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


            //apply gravity

            if (!ks.IsKeyDown(Keys.PageUp) && !ks.IsKeyDown(Keys.PageDown))
            {
                Gravity += GetDelta(gameTime, KartPhysics.GravityStep);

                if (Gravity > KartPhysics.MaxGravity)
                    Gravity = KartPhysics.MaxGravity;

                Position.Y -= GetDelta(gameTime, Gravity);

                Collide(quads);
            }

            //move up/down

            if (ks.IsKeyDown(Keys.PageUp) || gs.Buttons.LeftShoulder == ButtonState.Pressed)
                Position.Y += GetDelta(gameTime, 0.5f);

            if (ks.IsKeyDown(Keys.PageDown) || gs.Buttons.RightShoulder == ButtonState.Pressed)
                Position.Y += GetDelta(gameTime, -0.5f);
        }
    }
}