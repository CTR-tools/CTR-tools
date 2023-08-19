using Assimp;
using CTRFramework;
using ctrviewer.Engine.Input;
using ctrviewer.Engine.Testing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public partial class FirstPersonCamera : Camera
    {
        public float leftRightRot = 0;
        private float _upDownRot = 0;

        public float upDownRot
        {
            get => _upDownRot;
            set
            {
                if ((value < MathHelper.Pi / 2) && (value > -MathHelper.Pi / 2))
                {
                    _upDownRot = value;
                }
            }
        }

        public float rotationSpeed = 0.1f;
        public float translationSpeed = 2500f;

        public float speedScale = 1.0f;
        public float mouseScale = 1.0f;
        public float gamePadScale = 0.75f;

        Microsoft.Xna.Framework.Vector3 slowdown = new Microsoft.Xna.Framework.Vector3(0, 0, 0);

        Microsoft.Xna.Framework.Vector3 cameraFinalTarget = Microsoft.Xna.Framework.Vector3.Zero;

        public FirstPersonCamera(Game game) : base(game)
        {
        }

        public void SetRotation(float x, float y)
        {
            leftRightRot = x;
            upDownRot = y;
        }

        public void Update(GameTime gameTime, bool usemouse, bool move)
        {
            base.Update(gameTime);

            double amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            //handle keyboard rotations
            var rotationAmount = (float)(rotationSpeed * amount * 20);

            if (KeyboardHandler.IsDown(Keys.Left))
                leftRightRot += rotationAmount;

            if (KeyboardHandler.IsDown(Keys.Right))
                leftRightRot -= rotationAmount;

            if (KeyboardHandler.IsDown(Keys.Up))
                upDownRot += rotationAmount;

            if (KeyboardHandler.IsDown(Keys.Down))
                upDownRot -= rotationAmount;

            //handle mouse rotation
            if (usemouse)
            {
                leftRightRot -= MouseHandler.DeltaX * mouseScale / 1000f;
                upDownRot -= MouseHandler.DeltaY * mouseScale / 1000f;

                UpdateViewMatrix();
            }

            Microsoft.Xna.Framework.Vector2 gamepadrot = GamePadHandler.RightStick * (float)(amount * 4 * gamePadScale);

            leftRightRot -= gamepadrot.X;
            upDownRot += gamepadrot.Y;


            //handle movement

            Microsoft.Xna.Framework.Vector3 moveVector = new Microsoft.Xna.Framework.Vector3(0, 0, 0);

            if (move)
            {
                if (KeyboardHandler.IsDown(Keys.W) || GamePadHandler.IsDown(Buttons.DPadUp))
                    moveVector += Microsoft.Xna.Framework.Vector3.Forward;
                if (KeyboardHandler.IsDown(Keys.S) || GamePadHandler.IsDown(Buttons.DPadDown))
                    moveVector += Microsoft.Xna.Framework.Vector3.Backward;
                if (KeyboardHandler.IsDown(Keys.D) || GamePadHandler.IsDown(Buttons.DPadRight))
                    moveVector += Microsoft.Xna.Framework.Vector3.Right;
                if (KeyboardHandler.IsDown(Keys.A) || GamePadHandler.IsDown(Buttons.DPadLeft))
                    moveVector += Microsoft.Xna.Framework.Vector3.Left;

                //compensate diagonal movement
                if (
                    KeyboardHandler.AreAllDown(Keys.W, Keys.A) ||
                    KeyboardHandler.AreAllDown(Keys.W, Keys.D) ||
                    KeyboardHandler.AreAllDown(Keys.S, Keys.A) ||
                    KeyboardHandler.AreAllDown(Keys.S, Keys.D)
                    )
                    moveVector *= 0.7071f;


                if (KeyboardHandler.IsDown(Keys.Q))
                    moveVector += Microsoft.Xna.Framework.Vector3.Up;
                if (KeyboardHandler.IsDown(Keys.Z))
                    moveVector += Microsoft.Xna.Framework.Vector3.Down;

                moveVector *= 33f;
                moveVector *= 0.0001f;

                if (Math.Abs(moveVector.X) > Math.Abs(slowdown.X))
                    slowdown.X = moveVector.X;

                if (Math.Abs(moveVector.Y) > Math.Abs(slowdown.Y))
                    slowdown.Y = moveVector.Y;

                if (Math.Abs(moveVector.Z) > Math.Abs(slowdown.Z))
                    slowdown.Z = moveVector.Z;

                moveVector += slowdown;

                slowdown *= 0.75f;


                moveVector += new Microsoft.Xna.Framework.Vector3(GamePadHandler.State.ThumbSticks.Left.X / 100f, 0, -GamePadHandler.State.ThumbSticks.Left.Y / 100f);

                if (KeyboardHandler.IsDown(Keys.LeftShift) || GamePadHandler.IsDown(Buttons.A))
                    moveVector *= 2;

                speedScale -= (float)(GamePadHandler.State.Triggers.Left / 20f * amount);
                speedScale += (float)(GamePadHandler.State.Triggers.Right / 20f * amount);

                if (speedScale < 0.1f)
                    speedScale = 0.1f;

                if (speedScale > 5)
                    speedScale = 5;

                moveVector *= speedScale * speedScale;
                //moveVector *= (1 + padState.Triggers.Right * 3);
            }

            AddToCameraPosition(moveVector * (float)amount);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            AddToCameraPosition(new Microsoft.Xna.Framework.Vector3(0, 0, 0));
        }

        private void AddToCameraPosition(Microsoft.Xna.Framework.Vector3 vectorToAdd)
        {
            Microsoft.Xna.Framework.Matrix cameraRotation = Microsoft.Xna.Framework.Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);
            Microsoft.Xna.Framework.Vector3 rotatedVector = Microsoft.Xna.Framework.Vector3.Transform(vectorToAdd, cameraRotation);
            Position += translationSpeed * rotatedVector;
            Target += translationSpeed * rotatedVector;

            UpdateViewMatrix();
        }

        public void UpdateViewMatrix(float x = 0, float y = 0, float z = 0)
        {
            Microsoft.Xna.Framework.Matrix cameraRotation = Microsoft.Xna.Framework.Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);

            Microsoft.Xna.Framework.Vector3 cameraOriginalTarget = new Microsoft.Xna.Framework.Vector3(0, 0, -1);
            Microsoft.Xna.Framework.Vector3 cameraOriginalUpVector = new Microsoft.Xna.Framework.Vector3(0, 1, 0);

            Microsoft.Xna.Framework.Vector3 cameraRotatedTarget = Microsoft.Xna.Framework.Vector3.Transform(cameraOriginalTarget, cameraRotation);
            cameraFinalTarget = Position + cameraRotatedTarget;

            Microsoft.Xna.Framework.Vector3 cameraRotatedUpVector = Microsoft.Xna.Framework.Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            ViewMatrix = Microsoft.Xna.Framework.Matrix.CreateLookAt(Position, cameraFinalTarget, cameraRotatedUpVector);
        }


        public Microsoft.Xna.Framework.Matrix GetYawPitchRollMatrix()
        {
            return Microsoft.Xna.Framework.Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);
        }

        public void Copy(GameTime gameTime, FirstPersonCamera c)
        {
            leftRightRot = c.leftRightRot;
            upDownRot = c.upDownRot;
            Position = c.Position;
            Target = c.Target;
            Update(gameTime);
        }


        public QuadBlock FindLookAt(List<QuadBlock> quads)
        {
            QuadBlock result = null;

            var distance = 0f;
            var result_distance = 999999f;

            var ray = new SharpDX.Ray(
                new SharpDX.Vector3(
                    Position.X,
                    Position.Y,
                    Position.Z
                ),
                new SharpDX.Vector3(
                    cameraFinalTarget.X,
                    cameraFinalTarget.Y,
                    cameraFinalTarget.Z
                ));

            ray.Direction.X += ray.Position.X;
            ray.Direction.Y += ray.Position.Y;
            ray.Direction.Z += ray.Position.Z;
            ray.Direction.Normalize();

            GameConsole.Write(ray.ToString());

            var box = new SharpDX.BoundingBox();

            foreach (var quad in quads)
            {
                box = new SharpDX.BoundingBox(
                    new SharpDX.Vector3(
                        quad.bbox.Min.X,
                         quad.bbox.Min.Y,
                          quad.bbox.Min.Z
                        ),
                    new SharpDX.Vector3(
                        quad.bbox.Max.X,
                         quad.bbox.Max.Y,
                          quad.bbox.Max.Z
                        )
                    );

                if (SharpDX.Collision.RayIntersectsBox(ref ray, ref box, out distance))
                {
                    if (result_distance > distance)
                    {
                        result = quad;
                        result_distance = distance;
                    }
                }
            }

            return result;
        }
    }
}