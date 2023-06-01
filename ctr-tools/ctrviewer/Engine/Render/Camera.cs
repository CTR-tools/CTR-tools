using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata.Ecma335;

namespace ctrviewer.Engine.Render
{
    public partial class Camera : GameComponent
    {
        public Vector3 Target;
        public Vector3 Position;
        public Matrix ViewMatrix { get; protected set; }
        public Matrix ProjectionMatrix { get; protected set; }

        public float AspectRatio = 4f / 3f;

        public void SetAspectRatio(float aspectRatio)
        {
            AspectRatio = aspectRatio / (EngineSettings.Instance.StereoPair ? 2f : 1f);
        }

        public void SetAspectRatio(float width, float height)
        {
            AspectRatio = width / height / (EngineSettings.Instance.StereoPair ? 2f : 1f);
        }

        public float NearClip = 0.1f;
        public float FarClip = 1000.0f;
        public float ViewAngle = 80.0f;

        public Camera(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;

            base.Initialize();
        }

        public void UpdateProjectionMatrix()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(ViewAngle), AspectRatio, NearClip, FarClip);
        }

        public override void Update(GameTime gameTime)
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            //UpdateProjectionMatrix();

            base.Update(gameTime);
        }
    }
}