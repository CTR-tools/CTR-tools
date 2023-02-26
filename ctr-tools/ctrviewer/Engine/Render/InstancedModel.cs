using CTRFramework.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;

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
            set { _modelName = value; model = ContentVault.GetModel(_modelName); }
        }

        private TriListCollection model;

        private bool ShouldRotate = false;

        private static List<string> rotated = new List<string>() { "c", "t", "r", "fruit", "crystal" };

        public InstancedModel()
        {
        }

        public InstancedModel(string name, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            ModelName = name;

            Position = pos;
            Rotation = rot;
            Scale = scale;

            if (rotated.Contains(ModelName))
                anim = AnimationPlayer.Create("fullspin_left", true, 0.5f);
        }

        public void Update(GameTime gameTime)
        {
            if (anim is not null)
            {
                anim.Advance(gameTime);
                anim.Animate(this);
            }
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha, FirstPersonCamera camera)
        {
            if (model is null)
            {
                GameConsole.Write($"missing model {ModelName}");
                return;
            }

            effect.World = 
                Matrix.CreateScale(Scale) * 
                Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * 
                Matrix.CreateTranslation(Position);

            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            //alpha.World = effect.World;
            //alpha.View = effect.View;
            //alpha.Projection = effect.Projection;

            model.Draw(graphics, effect, null);
        }
    }
}