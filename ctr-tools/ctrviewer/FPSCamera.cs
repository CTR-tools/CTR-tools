using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ctrviewer
{
    public partial class FirstPersonCamera : Camera
    {
        #region Локальные свойства

        private float leftRightRot = 0;       // Угол поворота по оси Y
        private float _upDownRot = 0;
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
        private MouseState originalMouseState;
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

        #region Изменение объекта

        #region Цикл обновления
        public void Update(GameTime gameTime, bool usemouse, bool move)
        {
            base.Update(gameTime);

            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            if (usemouse)
            {
                #region Изменение направления цели камера при помощи мыши
                MouseState currentMouseState = Mouse.GetState();
                if (currentMouseState != originalMouseState)
                {
                    float xDifference = currentMouseState.X - originalMouseState.X;
                    float yDifference = currentMouseState.Y - originalMouseState.Y;

                    leftRightRot -= rotationSpeed * xDifference * amount;
                    //System.Diagnostics.Debug.WriteLine("leftRightRot=" + leftRightRot);

                    upDownRot -= rotationSpeed * yDifference * amount;
                    //System.Diagnostics.Debug.WriteLine("upDownRot=" + upDownRot);

                    Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
                    UpdateViewMatrix();
                    originalMouseState = Mouse.GetState();
                }
            }
            #endregion

            leftRightRot -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X / 20.0f;
            upDownRot += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y / 20.0f;

            #region Изменение пространственного положения камеры при помощи клавиатуры
            Vector3 moveVector = new Vector3(0, 0, 0);

            if (move)
            {

                KeyboardState keyState = Keyboard.GetState();
                GamePadState padState = GamePad.GetState(PlayerIndex.One);

                if (keyState.IsKeyDown(Keys.W) || padState.DPad.Up == ButtonState.Pressed)
                    moveVector += new Vector3(0, 0, -1);
                if (keyState.IsKeyDown(Keys.S) || padState.DPad.Down == ButtonState.Pressed)
                    moveVector += new Vector3(0, 0, 1);
                if (keyState.IsKeyDown(Keys.D) || padState.DPad.Right == ButtonState.Pressed)
                    moveVector += new Vector3(1, 0, 0);
                if (keyState.IsKeyDown(Keys.A) || padState.DPad.Left == ButtonState.Pressed)
                    moveVector += new Vector3(-1, 0, 0);

                if (keyState.IsKeyDown(Keys.Q))
                    moveVector += new Vector3(0, 1, 0);
                if (keyState.IsKeyDown(Keys.Z))
                    moveVector += new Vector3(0, -1, 0);

                moveVector += new Vector3(padState.ThumbSticks.Left.X, 0, -padState.ThumbSticks.Left.Y);

                // if (keyState.IsKeyDown(Keys.LeftShift) || padState.Buttons.A == ButtonState.Pressed)
                //     moveVector *= 2;

                moveVector *= (1 + padState.Triggers.Right * 3);


                if (keyState.IsKeyDown(Keys.Left))
                    leftRightRot += rotationSpeed;
                if (keyState.IsKeyDown(Keys.Right))
                    leftRightRot -= rotationSpeed;
                if (keyState.IsKeyDown(Keys.Up))
                    upDownRot += rotationSpeed;
                if (keyState.IsKeyDown(Keys.Down))
                    upDownRot -= rotationSpeed;
                #endregion


            }

            AddToCameraPosition(moveVector * amount);

        }
        #endregion

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
        private void UpdateViewMatrix()
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
    }

}