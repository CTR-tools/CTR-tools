using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer
{
    public class TriList
    {
        public VertexPositionColorTexture[] verts;
        public short[] indices;
        public bool textureEnabled;
        public string textureName;

        public TriList()
        {
        }
        public TriList(VertexPositionColorTexture[] v, short[] i, bool te, string name = "")
        {
            verts = v;
            indices = i;
            textureEnabled = te;
            textureName = name;
        }

        public TriList(TriList t)
        {
            verts = (VertexPositionColorTexture[])t.verts.Clone();
            indices = (short[])t.indices.Clone();
            textureEnabled = t.textureEnabled;
            textureName = t.textureName;
        }

        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            effect.TextureEnabled = textureEnabled;
            //effect.Texture = null;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        verts, 0, verts.Length,
                        indices, 0, indices.Length / 3,
                        VertexPositionColorTexture.VertexDeclaration
                );
            }
        }

    }
}