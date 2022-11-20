using ctrviewer.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

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

        Vector3 slowdown = new Vector3(0, 0, 0);

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

            if (usemouse && Game.IsActive)
            {
                leftRightRot -= MouseHandler.DeltaX * mouseScale / 1000f;
                upDownRot -= MouseHandler.DeltaY * mouseScale / 1000f;

                UpdateViewMatrix();
            }


            leftRightRot -= (float)(GamePadHandler.State.ThumbSticks.Right.X * amount * 4 * gamePadScale);
            upDownRot += (float)(GamePadHandler.State.ThumbSticks.Right.Y * amount * 4 * gamePadScale);


            Vector3 moveVector = new Vector3(0, 0, 0);

            if (move && Game.IsActive)
            {
                if (KeyboardHandler.IsDown(Keys.W) || GamePadHandler.IsPressed(Buttons.DPadUp))
                    moveVector += Vector3.Forward;
                if (KeyboardHandler.IsDown(Keys.S) || GamePadHandler.IsPressed(Buttons.DPadDown))
                    moveVector += Vector3.Backward;
                if (KeyboardHandler.IsDown(Keys.D) || GamePadHandler.IsPressed(Buttons.DPadRight))
                    moveVector += Vector3.Right;
                if (KeyboardHandler.IsDown(Keys.A) || GamePadHandler.IsPressed(Buttons.DPadLeft))
                    moveVector += Vector3.Left;

                //compensate diagonal movement
                if (
                    KeyboardHandler.AreAllDown(Keys.W, Keys.A) ||
                    KeyboardHandler.AreAllDown(Keys.W, Keys.D) ||
                    KeyboardHandler.AreAllDown(Keys.S, Keys.A) ||
                    KeyboardHandler.AreAllDown(Keys.S, Keys.D)
                    )
                    moveVector *= 0.7071f;


                if (KeyboardHandler.IsDown(Keys.Q))
                    moveVector += Vector3.Up;
                if (KeyboardHandler.IsDown(Keys.Z))
                    moveVector += Vector3.Down;

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


                moveVector += new Vector3(GamePadHandler.State.ThumbSticks.Left.X / 100f, 0, -GamePadHandler.State.ThumbSticks.Left.Y / 100f);

                if (KeyboardHandler.IsDown(Keys.LeftShift) || GamePadHandler.State.Buttons.A == ButtonState.Pressed)
                    moveVector *= 2;

                speedScale -= (float)(GamePadHandler.State.Triggers.Left / 20f * amount);
                speedScale += (float)(GamePadHandler.State.Triggers.Right / 20f * amount);

                if (speedScale < 0.1f)
                    speedScale = 0.1f;

                if (speedScale > 5)
                    speedScale = 5;

                moveVector *= speedScale * speedScale;
                //moveVector *= (1 + padState.Triggers.Right * 3);

                var rotationAmount = (float)(rotationSpeed * amount * 20);

                if (KeyboardHandler.IsDown(Keys.Left))
                    leftRightRot += rotationAmount;

                if (KeyboardHandler.IsDown(Keys.Right))
                    leftRightRot -= rotationAmount;

                if (KeyboardHandler.IsDown(Keys.Up))
                    upDownRot += rotationAmount;

                if (KeyboardHandler.IsDown(Keys.Down))
                    upDownRot -= rotationAmount;

            }

            AddToCameraPosition(moveVector * (float)amount);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            AddToCameraPosition(new Vector3(0, 0, 0));
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            Position += translationSpeed * rotatedVector;
            Target += translationSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        public void UpdateViewMatrix(float x = 0, float y = 0, float z = 0)
        {
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = Position + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            ViewMatrix = Matrix.CreateLookAt(Position, cameraFinalTarget, cameraRotatedUpVector);
        }


        public Matrix GetYawPitchRollMatrix()
        {
            return Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);
        }

        public void Copy(GameTime gameTime, FirstPersonCamera c)
        {
            leftRightRot = c.leftRightRot;
            upDownRot = c.upDownRot;
            Position = c.Position;
            Target = c.Target;
            Update(gameTime);
        }
    }
}