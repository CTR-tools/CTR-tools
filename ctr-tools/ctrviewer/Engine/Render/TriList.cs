using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer
{
    public class TriList
    {
        public bool Sealed = false;
        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        private VertexPositionColorTexture[] verts_sealed;
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
            verts_sealed = verts.ToArray();
            Sealed = true;
        }

        public void PushTri(List<VertexPositionColorTexture> lv)
        {
            if (Sealed)
            {
                Console.WriteLine("Trying to update sealed list.");
                return;
            }

            if (lv != null)
                verts.AddRange(lv);
        }

        public void Update(GameTime gameTime)
        {
            if (scrollingEnabled)
            {
                for (int i = 0; i < verts.Count; i++)
                    verts_sealed[i].TextureCoordinate.Y -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f * 3; //this will potentially overflow
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
            textureEnabled = t.textureEnabled;
            textureName = t.textureName;
            GenerateIndices();
        }

        public List<short> GenerateIndices()
        {
            List<short> x = new List<short>();

            for (int i = 0; i < verts.Count; i++)
                x.Add((short)i);

            return x;
        }


        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
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

                    Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);

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

                    if (Samplers.EnableWireframe)
                    {
                        effect.TextureEnabled = false;

                        Samplers.SetToDevice(graphics, EngineRasterizer.Wireframe);

                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                verts_sealed, 0, verts_sealed.Length,
                                indices, 0, indices.Length / 3,
                                VertexPositionColorTexture.VertexDeclaration
                            );
                        }

                        Samplers.SetToDevice(graphics, EngineRasterizer.Default);
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
