using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine.Render
{
    public class InstancedModel
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public SimpleAnimation anim = new SimpleAnimation();

        private string _modelName;
        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; model = ContentVault.GetModel(_modelName.ToLower()); }
        }

        private TriListCollection model;

        private bool ShouldRotate = false;

        private static string[] rotated = new string[] { "c", "t", "r", "fruit", "crystal" };

        public InstancedModel()
        {
        }

        public InstancedModel(string name, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            ModelName = name;
            Scale = scale;

            foreach (var entry in rotated)
                if (entry == ModelName)
                    ShouldRotate = true;
        }

        public void Update(GameTime gameTime)
        {
            if (anim != null)
                anim.Update(gameTime);

            if (ShouldRotate)
            {
                // Rotation += new Vector3(2f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f, 0, 0);
                //  if (Rotation.X >= Math.PI * 2)
                //      Rotation -= new Vector3((float)(Math.PI * 2), 0, 0);
            }
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha, FirstPersonCamera camera)
        {
            if (model == null)
            {
                GameConsole.Write($"missing model {ModelName}");
                return;
            }

            effect.World = Matrix.CreateScale(Scale * anim.State.Scale) * Matrix.CreateFromYawPitchRoll(
                Rotation.X + anim.State.Rotation.X,
                Rotation.Y + anim.State.Rotation.Y,
                Rotation.Z + anim.State.Rotation.Z
                ) * Matrix.CreateTranslation(Position + anim.State.Position);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            //alpha.World = effect.World;
            //alpha.View = effect.View;
            //alpha.Projection = effect.Projection;

            model.Draw(graphics, effect, null);
        }
    }
}