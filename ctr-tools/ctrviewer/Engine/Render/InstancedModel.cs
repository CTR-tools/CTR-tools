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
        public string ModelName;

        public InstancedModel()
        {
        }
        public InstancedModel(string name, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            ModelName = name;
            Scale = scale;
        }

        private static List<string> rotated = new List<string>() { "c", "t", "r", "fruit" };

        public void Update(GameTime gameTime)
        {
            if (rotated.Contains(ModelName))
                Rotation += new Vector3(2f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f, 0, 0);
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha, FirstPersonCamera camera)
        {
            effect.World = Matrix.CreateScale(Scale) * Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * Matrix.CreateTranslation(Position);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            if (ContentVault.Models.ContainsKey(ModelName))
            {
                ContentVault.Models[ModelName].Draw(graphics, effect, alpha);
            }
            else
            {
                Console.WriteLine(ModelName + " not loaded");
            }
        }
    }
}
