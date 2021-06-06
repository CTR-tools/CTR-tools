using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine.Render
{
    public class WireBox : IRenderable
    {
        VertexPositionColor[] Vertices;

        public WireBox(Vector3 min, Vector3 max, Color color, float scale = 1.0f)
        {
            Vertices = GenerateVerts(min * scale, max * scale, color);
        }

        public VertexPositionColor[] GenerateVerts(Vector3 min, Vector3 max, Color color)
        {
            return new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(min.X, min.Y, min.Z), color),
                new VertexPositionColor(new Vector3(max.X, min.Y, min.Z), color),
                new VertexPositionColor(new Vector3(max.X, min.Y, min.Z), color),
                new VertexPositionColor(new Vector3(max.X, max.Y, min.Z), color),

                new VertexPositionColor(new Vector3(max.X, max.Y, min.Z), color),
                new VertexPositionColor(new Vector3(min.X, max.Y, min.Z), color),
                new VertexPositionColor(new Vector3(min.X, max.Y, min.Z), color),
                new VertexPositionColor(new Vector3(min.X, min.Y, min.Z), color),

                new VertexPositionColor(new Vector3(min.X, min.Y, max.Z), color),
                new VertexPositionColor(new Vector3(max.X, min.Y, max.Z), color),
                new VertexPositionColor(new Vector3(max.X, min.Y, max.Z), color),
                new VertexPositionColor(new Vector3(max.X, max.Y, max.Z), color),

                new VertexPositionColor(new Vector3(max.X, max.Y, max.Z), color),
                new VertexPositionColor(new Vector3(min.X, max.Y, max.Z), color),
                new VertexPositionColor(new Vector3(min.X, max.Y, max.Z), color),
                new VertexPositionColor(new Vector3(min.X, min.Y, max.Z), color),


                new VertexPositionColor(new Vector3(max.X, min.Y, min.Z), color),
                new VertexPositionColor(new Vector3(max.X, min.Y, max.Z), color),
                new VertexPositionColor(new Vector3(max.X, max.Y, min.Z), color),
                new VertexPositionColor(new Vector3(max.X, max.Y, max.Z), color),

                new VertexPositionColor(new Vector3(min.X, max.Y, min.Z), color),
                new VertexPositionColor(new Vector3(min.X, max.Y, max.Z), color),
                new VertexPositionColor(new Vector3(min.X, min.Y, min.Z), color),
                new VertexPositionColor(new Vector3(min.X, min.Y, max.Z), color)
            };
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alphaEffect = null)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, Vertices, 0, Vertices.Length / 2);
            }
        }
    }
}