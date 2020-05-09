using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ctrviewer
{
    class Kart
    {
        public Vector3 Position;
        public Vector3 Angle;
        public float Scale;
        public string ModelName = "bluecone";

        public Kart()
        {
        }
        public Kart(Vector3 pos, Vector3 ang)
        {
            Position = pos;
            Angle = ang;
        }

        public void Update()
        {
            /// Position += new Vector3(0, 0, 0.001f);
        }


        public void Render(GraphicsDeviceManager graphics, BasicEffect effect, FirstPersonCamera camera)
        {
            effect.World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            if (Game1.instmodels.ContainsKey(ModelName))
            {
                Game1.instmodels[ModelName].Render(graphics, effect);
            }
            else if (Game1.instTris.ContainsKey(ModelName))
            {
                Game1.instTris[ModelName].Render(graphics, effect);
            }
            else
            {
                Console.WriteLine(ModelName + " not loaded");
            }
        }
    }
}
