using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine.Render
{
    public interface IRenderable
    {
        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha);
    }
}