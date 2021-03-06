﻿using Microsoft.Xna.Framework;

namespace ctrviewer.Engine.Render
{
    public partial class Camera : GameComponent
    {
        #region Свойства
        public Vector3 Target;                 // Координаты цели
        public Vector3 Position;                // Позиция 
        public Matrix ViewMatrix { get; protected set; }     // Видовая матрица
        public Matrix ProjectionMatrix { get; protected set; }  // Матрица проекции
        public float AspectRatio
        {
            get
            {
                return Game.GraphicsDevice.Viewport.AspectRatio;
            }
        }
        public float NearClip = 0.1f;
        public float FarClip = 1000.0f;
        public float ViewAngle = 80.0f;
        #endregion

        #region Раздел инициализации: конструктор, инициализация
        #region Конструктор
        public Camera(Game game)
            : base(game)
        {
        }
        #endregion

        #region Инициализация
        public override void Initialize()
        {
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;

            base.Initialize();
        }
        #endregion
        #endregion

        public void UpdateProjectionMatrix()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(ViewAngle), AspectRatio, NearClip, FarClip);
        }

        #region Обновление состояния объекта
        public override void Update(GameTime gameTime)
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
            UpdateProjectionMatrix();

            base.Update(gameTime);
        }

        #endregion
    }

}