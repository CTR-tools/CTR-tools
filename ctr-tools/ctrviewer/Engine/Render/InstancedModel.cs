using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class InstancedModel
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        private string _modelName;
        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; model = ContentVault.GetModel(_modelName.ToLower()); }
        }

        private TriListCollection model;

        private bool ShouldRotate = false;

        private static List<string> rotated = new List<string>() { "c", "t", "r", "fruit", "crystal" };

        public InstancedModel()
        {
        }

        public InstancedModel(string name, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            ModelName = name;
            Scale = scale;

            if (rotated.Contains(ModelName))
                ShouldRotate = true;
        }

        public void Update(GameTime gameTime)
        {
            if (ShouldRotate)
            {
                Rotation += new Vector3(2f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f, 0, 0);
                if (Rotation.X >= Math.PI * 2)
                    Rotation -= new Vector3((float)(Math.PI * 2), 0, 0);
            }
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha, FirstPersonCamera camera)
        {
            if (model == null)
            {
                GameConsole.Write($"missing model {ModelName}");
                return;
            }

            effect.World = Matrix.CreateScale(Scale) * Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * Matrix.CreateTranslation(Position);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            //alpha.World = effect.World;
            //alpha.View = effect.View;
            //alpha.Projection = effect.Projection;

            model.Draw(graphics, effect, null);
        }
    }
}
