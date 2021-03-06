using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ctrviewer.Engine.Render
{
    public partial class FirstPersonCamera : Camera
    {
        #region Локальные свойства


        public void SetRotation(float x, float y)
        {
            leftRightRot = x;
            upDownRot = y;
        }

        public float leftRightRot = 0;       // Угол поворота по оси Y
        public float _upDownRot = 0;
        private float upDownRot           // Угол поворота по оси X
        {
            get { return _upDownRot; }
            set
            {
                if ((value < MathHelper.Pi / 2) && (value > -MathHelper.Pi / 2))
                {
                    _upDownRot = value;
                }
            }
        }
        //private MouseState originalMouseState;
        #endregion

        #region Глобальные свойства
        public float rotationSpeed = 0.1f;     // Скорость угла поворота
        public float translationSpeed = 2500f;    // Скорость перемещения
        #endregion

        #region Конструктор
        public FirstPersonCamera(Game game)
            : base(game)
        {

        }
        #endregion

        public float speedScale = 1.0f;
        public float mouseScale = 1.0f;

        Vector3 slowdown = new Vector3(0, 0, 0);

        #region Изменение объекта

        #region Цикл обновления
        public void Update(GameTime gameTime, bool usemouse, bool move, MouseState newms, MouseState oldms)
        {
            base.Update(gameTime);

            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            //float amount = 0.01f;

            if (usemouse && Game.IsActive)
            {
                #region Изменение направления цели камера при помощи мыши

                if (oldms != newms)
                {
                    float xDifference = (newms.X - oldms.X) / 1000f;
                    float yDifference = (newms.Y - oldms.Y) / 1000f;

                    leftRightRot -= xDifference * mouseScale;//rotationSpeed * xDifference * amount;
                    //System.Diagnostics.Debug.WriteLine("leftRightRot=" + leftRightRot);

                    upDownRot -= yDifference * mouseScale;// rotationSpeed * yDifference* amount;
                    //System.Diagnostics.Debug.WriteLine("upDownRot=" + upDownRot);
                    UpdateViewMatrix();
                }
            }
            #endregion

            leftRightRot -= GamePad.GetState(Game1.activeGamePad).ThumbSticks.Right.X * amount * 4;
            upDownRot += GamePad.GetState(Game1.activeGamePad).ThumbSticks.Right.Y * amount * 4;

            #region Изменение пространственного положения камеры при помощи клавиатуры
            Vector3 moveVector = new Vector3(0, 0, 0);

            if (move && Game.IsActive)
            {

                KeyboardState keyState = Keyboard.GetState();
                GamePadState padState = GamePad.GetState(Game1.activeGamePad);


                if (keyState.IsKeyDown(Keys.W) || padState.DPad.Up == ButtonState.Pressed)
                    moveVector += new Vector3(0, 0, -1) * 100;
                if (keyState.IsKeyDown(Keys.S) || padState.DPad.Down == ButtonState.Pressed)
                    moveVector += new Vector3(0, 0, 1) * 100;
                if (keyState.IsKeyDown(Keys.D) || padState.DPad.Right == ButtonState.Pressed)
                    moveVector += new Vector3(1, 0, 0) * 100;
                if (keyState.IsKeyDown(Keys.A) || padState.DPad.Left == ButtonState.Pressed)
                    moveVector += new Vector3(-1, 0, 0) * 100;


                if (keyState.IsKeyDown(Keys.W) && keyState.IsKeyDown(Keys.A))
                    moveVector += new Vector3(0.25f, 0, 0.25f) * 100;

                if (keyState.IsKeyDown(Keys.W) && keyState.IsKeyDown(Keys.D))
                    moveVector += new Vector3(-0.25f, 0, 0.25f) * 100;

                if (keyState.IsKeyDown(Keys.S) && keyState.IsKeyDown(Keys.A))
                    moveVector += new Vector3(0.25f, 0, -0.25f) * 100;

                if (keyState.IsKeyDown(Keys.S) && keyState.IsKeyDown(Keys.D))
                    moveVector += new Vector3(-0.25f, 0, -0.25f) * 100;


                if (keyState.IsKeyDown(Keys.Q))
                    moveVector += new Vector3(0, 1, 0) * 100;
                if (keyState.IsKeyDown(Keys.Z))
                    moveVector += new Vector3(0, -1, 0) * 100;

                moveVector *= 0.3f;
                moveVector *= 0.0001f;

                if (Math.Abs(moveVector.X) > Math.Abs(slowdown.X))
                    slowdown.X = moveVector.X;

                if (Math.Abs(moveVector.Y) > Math.Abs(slowdown.Y))
                    slowdown.Y = moveVector.Y;

                if (Math.Abs(moveVector.Z) > Math.Abs(slowdown.Z))
                    slowdown.Z = moveVector.Z;

                moveVector += slowdown;

                slowdown *= 0.75f;


                moveVector += new Vector3(padState.ThumbSticks.Left.X / 100f, 0, -padState.ThumbSticks.Left.Y / 100f);

                // if (keyState.IsKeyDown(Keys.LeftShift) || padState.Buttons.A == ButtonState.Pressed)
                //     moveVector *= 2;


                speedScale -= padState.Triggers.Left / 20f;
                speedScale += padState.Triggers.Right / 20f;

                if (speedScale < 0.1f)
                    speedScale = 0.1f;

                if (speedScale > 5)
                    speedScale = 5;

                moveVector *= speedScale * speedScale;
                //moveVector *= (1 + padState.Triggers.Right * 3);


                if (keyState.IsKeyDown(Keys.Left))
                    leftRightRot += rotationSpeed * amount * 20;
                if (keyState.IsKeyDown(Keys.Right))
                    leftRightRot -= rotationSpeed * amount * 20;
                if (keyState.IsKeyDown(Keys.Up))
                    upDownRot += rotationSpeed * amount * 20;
                if (keyState.IsKeyDown(Keys.Down))
                    upDownRot -= rotationSpeed * amount * 20;
                #endregion


            }

            AddToCameraPosition(moveVector * amount);

        }
        #endregion

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            AddToCameraPosition(new Vector3(0, 0, 0));
        }

        #region Изменение положения камеры и направления смотра
        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            Position += translationSpeed * (rotatedVector);
            Target += translationSpeed * (rotatedVector);
            UpdateViewMatrix();
        }
        #endregion

        #region Обновление матрицы вида
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
        #endregion
        #endregion


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