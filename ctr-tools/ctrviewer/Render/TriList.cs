using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace ctrviewer
{
    public class TriList
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
            get { return indices.Length / 3; }
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


        public TriList()
        {
        }
        public void Seal()
        {
            indices = GenerateIndices().ToArray();
        }

        public void PushTri(List<VertexPositionColorTexture> lv)
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

        public TriList(VertexPositionColorTexture[] v, short[] i, bool te, string name = "")
        {
            verts.AddRange(v);
            textureEnabled = te;
            textureName = name;
        }

        public TriList(TriList t)
        {
            verts.AddRange(t.verts);
            indices = (short[])t.indices.Clone();
            textureEnabled = t.textureEnabled;
            textureName = t.textureName;
        }

        public List<short> GenerateIndices()
        {
            List<short> x = new List<short>();

            for (int i = 0; i < verts.Count; i++)
                x.Add((short)i);

            return x;
        }


        public void Render(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            if (indices != null && verts != null)
            {
                if (verts.Count > 0)
                {
                    effect.TextureEnabled = textureEnabled;

                    if (textureEnabled)
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
                else
                {
                    Console.WriteLine("Empty Trilist!");
                }
            }
        }
    }

}
