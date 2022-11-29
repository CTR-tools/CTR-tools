using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.XInput;

namespace ctrviewer.Engine.Render
{
    public class InstancedModel
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public AnimationPlayer anim;

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
                {
                    anim = AnimationPlayer.Create("rotate_left");
                    anim.Speed = 0.5f;
                    anim.Run();
                }
        }

        public void Update(GameTime gameTime)
        {
            anim?.Advance(gameTime);
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha, FirstPersonCamera camera)
        {
            if (model is null)
            {
                GameConsole.Write($"missing model {ModelName}");
                return;
            }

            var scale = Scale;
            var pos = Position;
            var rot = Rotation;

            if (anim is not null)
            {
                scale *= anim.State.Scale;
                pos += anim.State.Position;
                rot += anim.State.Rotation;
            }

            effect.World = 
                Matrix.CreateScale(scale) * 
                Matrix.CreateFromYawPitchRoll(rot.X, rot.Y, rot.Z) * 
                Matrix.CreateTranslation(pos);

            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            //alpha.World = effect.World;
            //alpha.View = effect.View;
            //alpha.Projection = effect.Projection;

            model.Draw(graphics, effect, null);
        }
    }
}