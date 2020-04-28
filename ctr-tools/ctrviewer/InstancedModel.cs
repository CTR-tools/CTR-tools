using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer
{
    class InstancedModel
    {
        public Vector3 Position;
        public float Scale;
        public string ModelName;

        public InstancedModel()
        {
        }
        public InstancedModel(string name, Vector3 pos, float scale)
        {
            Position = pos;
            ModelName = name;
            Scale = scale;
        }
        public void Render(GraphicsDeviceManager graphics, BasicEffect effect, FirstPersonCamera camera)
        {
            effect.World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            Game1.instmodels[ModelName].Render(graphics, effect);
        }
    }
}
