using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CTRFramework;
using CTRFramework.Shared;

namespace ctrviewer
{
    public class QuadList
    {
        public List<VertexPositionColorTexture> verts;
        public bool textureEnabled;
        public bool scrollingEnabled;
        public string textureName;

        public QuadList()
        {

        }

        public void PushQuad(List<VertexPositionColorTexture> lv)
        {
            verts.AddRange(lv);
        }

        public void Update()
        {
            if (scrollingEnabled) Scroll();
        }

        public void Scroll()
        {
            for (int i = 0; i < verts.Count; i++)
            {
                VertexPositionColorTexture v = verts[i];
                v.TextureCoordinate = new Vector2(v.TextureCoordinate.X + 0.1f, v.TextureCoordinate.Y);
                verts[i] = v;
            }
        }

        public QuadList(List<VertexPositionColorTexture> v, bool te, string name = "")
        {
            verts = v;
            textureEnabled = te;
            textureName = name;
        }

        public QuadList(QuadList t)
        {
            verts.AddRange(t.verts);
            textureEnabled = t.textureEnabled;
            textureName = t.textureName;
        }

        public List<short> GenerateIndices()
        {
            List<short> x = new List<short>();

            List<short> pattern = new List<short> { 0, 1, 2, 2, 1, 3 };
            short stride = 4;

            for (int i = 0; i < verts.Count / stride; i++)
                foreach (short s in pattern) x.Add((short)(i * stride + s));

            return x;
        }

        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            effect.TextureEnabled = textureEnabled;
            //effect.Texture = null;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                short[] indices = GenerateIndices().ToArray();

                graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        verts.ToArray(), 0, verts.Count,
                        indices, 0, indices.Length / 3,
                        VertexPositionColorTexture.VertexDeclaration
                );
            }
        }

    }
}