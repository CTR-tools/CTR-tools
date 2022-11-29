using CTRFramework;
using ctrviewer.Engine.Input;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Testing
{
    public class KartPhysics
    {
        public static float MaxAcceleration = 0.01f;
        public static float MaxSpeed = 0.25f;
        public static float MaxTurningStep = 0.025f;
        public static float MaxDriftingStep = 0.05f;
        public static float MaxJumpStep = 0.001f;

        public static float Friction = 0.001f;
        public static float BrakeFriction = 0.01f;

        public static float TargetFps = 60;

        public static float GravityStep = 0.01f;//0.01f;
        public static float MaxGravity = 0.5f;//1f;
    }

    enum PowerType
    {
        Gravity,
        Engine,
        EngineBackward,
        Jump,
        SteerLeft,
        SteerRight,
        DriftLeft,
        DriftRight,
        Terrain,
        GodZ
    }

    class Power
    {
        public bool Enabled = true;
        public bool IsInertial = false;

        public Vector3 Direction = Vector3.Zero;

        private float _value = 0;
        public float Value
        {
            get
            {
                if (_value > MaxValue)
                    _value = MaxValue;

                return _value;
            }

            set
            {
                _value = value;
            }
        }

        public float MaxValue = 0;
        public float FadeValue = 0;

        public void Boost(float value)
        {
            if (Enabled)
                Value += value;
        }

        public void Update(GameTime gameTime)
        {
            if (_value != 0 && IsInertial)
            {
                if (_value > 0)
                {
                    _value -= FadeValue;
                    if (_value < 0)
                        _value = 0;
                }

                if (_value < 0)
                {
                    _value += FadeValue;
                    if (_value > 0)
                        _value = 0;
                }
            }
        }
    }

    public class Kart : InstancedModel
    {
        public AnimationPlayer path;

        Dictionary<PowerType, Power> Powers = new Dictionary<PowerType, Power>();

        Vector3 oldPosition;

        public float Speed = 0;
        public float Accel = 0;
        public float Gravity = 0;
        public Kart(Vector3 pos, Vector3 rot) : base("crash", pos, rot, Vector3.One * 0.06f)
        {
            Powers.Add(PowerType.Gravity, new Power() { MaxValue = KartPhysics.MaxGravity, Direction = Vector3.Down });
            Powers.Add(PowerType.Terrain, new Power() { MaxValue = 99999f, Direction = Vector3.Zero });
            Powers.Add(PowerType.Engine, new Power() { MaxValue = KartPhysics.MaxSpeed, Direction = Vector3.Forward, IsInertial = true, FadeValue = KartPhysics.Friction });
            //Powers.Add(PowerType.SteerLeft, new Power() { MaxValue = KartPhysics.MaxTurningStep, Direction = Vector3.Left });
            //Powers.Add(PowerType.SteerRight, new Power() { MaxValue = KartPhysics.MaxTurningStep, Direction = Vector3.Right });
            //Powers.Add(PowerType.DriftLeft, new Power() { MaxValue = KartPhysics.MaxDriftingStep, Direction = Vector3.Left });
            //Powers.Add(PowerType.DriftRight, new Power() { MaxValue = KartPhysics.MaxDriftingStep, Direction = Vector3.Right });
            //Powers.Add(PowerType.Jump, new Power() { MaxValue = KartPhysics.MaxJumpStep, Direction = Vector3.Up });
            Powers.Add(PowerType.GodZ, new Power() { MaxValue = 1f, Direction = Vector3.Up });
        }

        public float GetDelta(GameTime gameTime, float value)
        {
            return value * KartPhysics.TargetFps / 1000f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public Vector3 GetResultingPower(GameTime gameTime)
        {
            Vector3 result = Vector3.Zero;

            foreach (var power in Powers.Values)
                if (power.Enabled)
                    result += power.Direction * GetDelta(gameTime, power.Value);

            return result;
        }

        public Vector3 GetResultingDirection(GameTime gameTime)
        {
            return Vector3.Normalize(GetResultingPower(gameTime));
        }

        public void Collide(List<CtrScene> scenes, GameTime gameTime)
        {
            Powers[PowerType.Terrain].Enabled = false;

            foreach (var scene in scenes)
                foreach (var quad in scene.quads)
                    if (!quad.visDataFlags.HasFlag(VisDataFlags.Water) && !quad.visDataFlags.HasFlag(VisDataFlags.Hidden))
                        if (
                            (quad.bbox.numericMin.X - 1 <= Position.X) &&
                            (quad.bbox.numericMin.Y - 1 <= Position.Y) &&
                            (quad.bbox.numericMin.Z - 1 <= Position.Z) &&
                            (quad.bbox.numericMax.X + 1 >= Position.X) &&
                            (quad.bbox.numericMax.Y + 1 >= Position.Y) &&
                            (quad.bbox.numericMax.Z + 1 >= Position.Z)
                            )
                        {
                            GameConsole.Write($"collide with quad bb: {quad.bbox} at {Position}");

                            for (int i = 0; i < 4; i++)
                            {
                                List<Vertex> vertices = quad.GetVertexListq(scene.verts, i);

                                Vector3 p0 = DataConverter.ToVector3(vertices[0].Position);
                                Vector3 p1 = DataConverter.ToVector3(vertices[1].Position);
                                Vector3 p2 = DataConverter.ToVector3(vertices[2].Position);
                                Vector3 p3 = DataConverter.ToVector3(vertices[3].Position);

                                Vector3 dir = GetResultingDirection(gameTime);
                                Ray oldray = new Ray(Position, dir);
                                Ray newray = new Ray(Position + GetResultingPower(gameTime), dir);

                                if (!TestCollision(oldray, newray, p0, p2, p1))
                                    TestCollision(oldray, newray, p3, p2, p1);
                            }
                        }

        }

        public bool TestCollision(Ray oldray, Ray newray, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float oldcoll = Collision.IntersectRayTriangle(oldray, p1, p2, p3);
            float newcoll = Collision.IntersectRayTriangle(newray, p1, p2, p3);

            float dist = Vector3.Distance(oldray.Position, newray.Position);

            Vector3 fn = Vector3.Normalize(Vector3.Cross(p1 - p3, p1 - p2));

            if (!Single.IsNaN(oldcoll) && !Single.IsNaN(newcoll))
                return false;

            if (
                (!Single.IsNaN(oldcoll) && Single.IsNaN(newcoll))
               //(newcoll > 0 && oldcoll < 0) ||
               //(newcoll < 0 && oldcoll > 0)
               )
            {
                GameConsole.Write("BANG");

                GameConsole.Write($" face normal: {fn} vector up: {Vector3.Up}");

                Powers[PowerType.Gravity].Value = 0;

                Powers[PowerType.Terrain].Direction = Vector3.Up;  //oldray.Direction;
                Powers[PowerType.Terrain].Value = dist;//- dist + oldcoll;

                Powers[PowerType.Terrain].Enabled = true;

                return true;
            }

            return false;
        }

        float limitMax = Int16.MaxValue / 100f;
        float limitMin = Int16.MinValue / 100f;

        public void Update(GameTime gameTime, List<CtrScene> scenes)
        {
            if (path != null)
            {
                path.Advance(gameTime);
                Position = path.State.Position;
                Rotation = path.State.Rotation;
                return;
            }

            oldPosition = Position;

            foreach (var power in Powers)
                power.Value.Update(gameTime);

            Powers[PowerType.Gravity].Boost(GetDelta(gameTime, KartPhysics.GravityStep));

            //turning

            if (KeyboardHandler.IsAnyDown(Keys.A, Keys.Left) || GamePadHandler.IsDown(Buttons.DPadLeft))
                Rotation.X += GetDelta(gameTime, KartPhysics.MaxTurningStep * (KeyboardHandler.IsAnyDown(Keys.S, Keys.Down) ? 2 : 1));

            if (KeyboardHandler.IsAnyDown(Keys.D, Keys.Right) || GamePadHandler.IsDown(Buttons.DPadRight))
                Rotation.X -= GetDelta(gameTime, KartPhysics.MaxTurningStep * (KeyboardHandler.IsAnyDown(Keys.S, Keys.Down) ? 2 : 1));

            Rotation.X -= GetDelta(gameTime, KartPhysics.MaxTurningStep * GamePadHandler.State.ThumbSticks.Left.X);

            Powers[PowerType.Engine].Direction = Vector3.Transform(Vector3.Backward, Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z));


            //go forward

            if (KeyboardHandler.IsDown(Keys.W) || GamePadHandler.IsDown(Buttons.A))
            {
                Powers[PowerType.Engine].Boost(GetDelta(gameTime, KartPhysics.MaxAcceleration));
            }


            //move up/down

            if (KeyboardHandler.IsDown(Keys.PageUp) || GamePadHandler.IsDown(Buttons.DPadUp))
            {
                Powers[PowerType.Gravity].Value = 0;
                Powers[PowerType.Gravity].Enabled = false;
                Powers[PowerType.GodZ].Value = GetDelta(gameTime, 0.5f);
                Powers[PowerType.GodZ].Enabled = true;
            }
            else if (KeyboardHandler.IsDown(Keys.PageDown) || GamePadHandler.IsDown(Buttons.DPadDown))
            {
                Powers[PowerType.Gravity].Value = 0;
                Powers[PowerType.Gravity].Enabled = false;
                Powers[PowerType.GodZ].Value = GetDelta(gameTime, -0.5f);
                Powers[PowerType.GodZ].Enabled = true;
            }
            else
            {
                Collide(scenes, gameTime);
            }

            var result = GetResultingPower(gameTime);

            Speed = result.Length();

            Position += result;

            Powers[PowerType.Gravity].Enabled = true;
            Powers[PowerType.GodZ].Enabled = false;

            if (Position.X > limitMax) Position.X = limitMin;
            if (Position.Y > limitMax) Position.Y = limitMin;
            if (Position.Z > limitMax) Position.Z = limitMin;
            if (Position.X < limitMin) Position.X = limitMax;
            if (Position.Y < limitMin) Position.Y = limitMax;
            if (Position.Z < limitMin) Position.Z = limitMax;
        }
    }
}