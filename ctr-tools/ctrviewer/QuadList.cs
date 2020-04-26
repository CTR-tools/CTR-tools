using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer
{
    public class QuadList
    {
        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        public bool textureEnabled;
        public bool scrollingEnabled;
        public string textureName = "";
        private short[] indices;

        public int numVerts
        {
            get { return verts.Count; }
        }

        public int numQuads
        {
            get { return indices.Length / 6; }
        }

        public void SetColor(Color c)
        {
            for (int i = 0; i < numVerts; i++)
            {
                VertexPositionColorTexture v = verts[i];
                v.Color = c;
                verts[i] = v;
            }
        }

        public QuadList()
        {
        }

        public void Seal()
        {
            indices = GenerateIndices().ToArray();
        }

        public void PushQuad(List<VertexPositionColorTexture> lv)
        {
            if (lv != null)
                verts.AddRange(lv);
        }

        public void Update()
        {
            if (scrollingEnabled)
                Scroll();
        }

        public void Scroll()
        {
            for (int i = 0; i < verts.Count; i++)
            {
                VertexPositionColorTexture v = verts[i];
                v.TextureCoordinate = new Vector2(v.TextureCoordinate.X, v.TextureCoordinate.Y - 0.05f);
                verts[i] = v;
            }
        }

        public QuadList(List<VertexPositionColorTexture> v, bool te, string name = "")
        {
            verts.AddRange(v);
            textureEnabled = te;
            textureName = name;
        }

        public QuadList(QuadList t)
        {
            verts.AddRange(t.verts);
            textureEnabled = t.textureEnabled;
            textureName = t.textureName;
            GenerateIndices();
        }

        short[] pattern = new short[] { 0, 1, 2, 2, 1, 3 };

        public List<short> GenerateIndices()
        {
            List<short> x = new List<short>();

            for (int i = 0; i < verts.Count / 4; i++)
            {
                foreach (short s in pattern)
                    x.Add((short)(i * 4 + s));
            }

            return x;
        }

        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            if (indices != null && verts != null)
            {
                effect.TextureEnabled = textureEnabled;

                if (Game1.textures.ContainsKey(textureName))
                {
                    effect.Texture = Game1.textures[textureName];
                }
                else
                {
                    //Console.WriteLine("missing texture: " + textureName);
                    effect.Texture = Game1.textures["test"];
                }

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

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
}