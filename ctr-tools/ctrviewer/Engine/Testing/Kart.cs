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

    enum Power
    {
        Gravity,
        EngineForward,
        EngineBackward,
        Jump,
        SteerLeft,
        SteerRight,
        DriftLeft,
        DriftRight
    }

    class Kart : InstancedModel
    {
        Dictionary<Power, Vector3> Powers = new Dictionary<Power, Vector3>();

        public float Speed = 0;
        public float Accel = 0;
        public float Gravity = 0;

        public Kart(Vector3 pos, Vector3 rot) : base("crash", pos, rot, Vector3.One)
        {
            Powers.Add(Power.Gravity, Vector3.Down * KartPhysics.MaxGravity);
            Powers.Add(Power.EngineBackward, Vector3.Zero);
            Powers.Add(Power.EngineForward, Vector3.Zero);
            Powers.Add(Power.SteerLeft, Vector3.Zero);
            Powers.Add(Power.SteerRight, Vector3.Zero);
            Powers.Add(Power.DriftLeft, Vector3.Zero);
            Powers.Add(Power.DriftRight, Vector3.Zero);
            Powers.Add(Power.Jump, Vector3.Zero);
        }

        public float GetDelta(GameTime gameTime, float value)
        {
            return value * (float)gameTime.ElapsedGameTime.TotalMilliseconds * KartPhysics.TargetFps / 1000;
        }

        public void Collide(List<Scene> scenes)
        {
            foreach (var scene in scenes)
                foreach (var quad in scene.quads)
                    if (quad.quadFlags.HasFlag(QuadFlags.Ground) || quad.quadFlags.HasFlag(QuadFlags.Wall))
                        if (
                            (quad.bb.numericMin.X <= Position.X) &&
                            (quad.bb.numericMin.Y - Gravity * 2 <= Position.Y) &&
                            (quad.bb.numericMin.Z <= Position.Z) &&
                            (quad.bb.numericMax.X >= Position.X) &&
                            (quad.bb.numericMax.Y + 1 + Gravity >= Position.Y) &&
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

        public void Update(GameTime gameTime, List<Scene> scenes)
        {
            GamePadState gs = GamePad.GetState(Game1.activeGamePad);

            //turning

            if (KeyboardHandler.IsAnyDown(Keys.A, Keys.Left) || gs.DPad.Left == ButtonState.Pressed)
                Rotation.X += GetDelta(gameTime, KartPhysics.MaxTurningStep * (Speed / KartPhysics.MaxSpeed) * (KeyboardHandler.IsAnyDown(Keys.S, Keys.Down) ? 2 : 1));

            if (KeyboardHandler.IsAnyDown(Keys.D, Keys.Right) || gs.DPad.Right == ButtonState.Pressed)
                Rotation.X -= GetDelta(gameTime, KartPhysics.MaxTurningStep * (Speed / KartPhysics.MaxSpeed) * (KeyboardHandler.IsAnyDown(Keys.S, Keys.Down) ? 2 : 1));

            //udate accel value

            if (gs.IsButtonDown(Buttons.A) || KeyboardHandler.IsAnyDown(Keys.W, Keys.Up) || gs.Buttons.A == ButtonState.Pressed)
            {
                Accel = KartPhysics.MaxAcceleration;
            }
            else
            {
                Accel = 0;
            }

            //update speed value

            Speed += GetDelta(gameTime, Accel - (KeyboardHandler.IsAnyDown(Keys.S, Keys.Down) ? KartPhysics.BrakeFriction : KartPhysics.Friction));

            if (Speed > KartPhysics.MaxSpeed)
                Speed = KartPhysics.MaxSpeed;

            if (Speed < 0)
                Speed = 0;

            //move forward

            Position += Vector3.Transform(Vector3.Backward  * GetDelta(gameTime, Speed), Matrix.CreateRotationY(Rotation.X));


            //apply gravity

            if (!KeyboardHandler.IsDown(Keys.PageUp) && !KeyboardHandler.IsDown(Keys.PageDown))
            {
                Gravity += GetDelta(gameTime, KartPhysics.GravityStep);

                if (Gravity > KartPhysics.MaxGravity)
                    Gravity = KartPhysics.MaxGravity;

                Position.Y -= GetDelta(gameTime, Gravity);

                Collide(scenes);

                if (Gravity == 0 & KeyboardHandler.IsDown(Keys.Space))
                {
                    Gravity = 0;
                    Position.Y += GetDelta(gameTime, 2f);
                }
            }

            //move up/down

            if (KeyboardHandler.IsDown(Keys.PageUp) || gs.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                Gravity = 0;
                Position.Y += GetDelta(gameTime, 0.5f);
            }

            if (KeyboardHandler.IsDown(Keys.PageDown) || gs.Buttons.RightShoulder == ButtonState.Pressed)
            {
                Gravity = 0;
                Position.Y += GetDelta(gameTime, -0.5f);
            }
        }
    }
}