using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class TriListCollection : List<TriList>, IRenderable
    {
        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            foreach (var entry in this)
                entry.Draw(graphics, effect, alpha);
        }
    }

    public class AnimatedTriListCollection : List<TriListCollection>, IRenderable
    {
        public int TotalFrames => this.Count;
        public int CurrentFrame = 0;
        public float frameDuration = 0.01f;

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            this[CurrentFrame].Draw(graphics, effect, alpha);
        }


        public void Update(GameTime gameTime)
        {
            CurrentFrame++;

            if (CurrentFrame >= TotalFrames)
                CurrentFrame = 0;
        }
    }
}