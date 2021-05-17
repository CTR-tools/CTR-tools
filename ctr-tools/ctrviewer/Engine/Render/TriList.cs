using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class TriList : IRenderable
    {
        public bool Sealed = false;
        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        private VertexPositionColorTexture[] verts_sealed;
        public List<int> curColor = new List<int>();
        public Dictionary<int, List<Color>> animatedColors = new Dictionary<int, List<Color>>();
        public bool vColAnimEnabled = true;
        public bool textureEnabled = true;
        public bool ScrollingEnabled = false;
        public bool CullingEnabled = true;
        public string textureName = "";
        private short[] indices;

        public int numVerts
        {
            get => verts.Count;
        }

        public int numQuads
        {
            get => indices.Length / 3;
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
            for (int i = 0; i < numVerts; i++)
            {
                animatedColors.Add(i, new List<Color>());

                for (int j = 0; j < 127; j++)
                    animatedColors[i].Add(new Color(j*2,j*2,j*2));

                for (int j = 0; j < 127; j++)
                    animatedColors[i].Add(new Color(255-j*2, 255-j*2, 255-j*2));

                curColor.Add(0);
            }
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

        public void PushQuad(List<VertexPositionColorTexture> lv)
        {
            if (Sealed)
            {
                Console.WriteLine("Trying to update sealed list.");
                return;
            }

            if (lv != null)
                verts.AddRange(new List<VertexPositionColorTexture>() { lv[0], lv[1], lv[2], lv[2], lv[1], lv[3] } );
        }

        public void Update(GameTime gameTime)
        {
            if (ScrollingEnabled)
            {
                for (int i = 0; i < numVerts; i++)
                {
                    verts_sealed[i].TextureCoordinate.Y -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f * 3; //this will potentially overflow
                }
            }

            if (vColAnimEnabled)
            {
                for (int i = 0; i < numVerts; i++)
                {
                    verts_sealed[i].Color = animatedColors[i][curColor[i]];

                    curColor[i]++;
                    if (curColor[i] >= animatedColors[i].Count)
                        curColor[i] = 0;
                }
            }
        }

        public TriList(List<VertexPositionColorTexture> v, bool te, string name = "")
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
            List<short> indices = new List<short>();

            for (int i = 0; i < numVerts; i++)
                indices.Add((short)i);

            return indices;
        }


        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            if (indices != null && verts != null)
            {
                if (verts.Count > 0)
                {
                    effect.TextureEnabled = textureEnabled;

                    if (textureEnabled)
                    {
                        effect.Texture = ContentVault.Textures["test"];

                        if (ContentVault.Textures.ContainsKey(textureName))
                            effect.Texture = ContentVault.Textures[textureName];
                    }

                    if (!CullingEnabled)
                        Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                verts_sealed, 0, numVerts,
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
                                verts_sealed, 0, numVerts,
                                indices, 0, indices.Length / 3,
                                VertexPositionColorTexture.VertexDeclaration
                            );
                        }
                    }

                    Samplers.SetToDevice(graphics, EngineRasterizer.Default);

                }
                else
                {
                    Console.WriteLine("Empty Trilist!");
                }
            }
        }
    }
}
