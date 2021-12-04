using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ctrviewer.Engine.Testing
{
    public class QuadList : IRenderable
    {
        public bool Sealed = false;
        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        private VertexPositionColorTexture[] verts_sealed;
        public bool textureEnabled;
        public bool scrollingEnabled;
        public bool CullingEnabled = true;
        public string textureName = "";
        private short[] indices;

        public int numVerts
        {
            get => verts.Count;
        }

        public int numQuads
        {
            get => indices.Length / 6;
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
            verts_sealed = verts.ToArray();
            Sealed = true;
        }

        public void PushQuad(List<VertexPositionColorTexture> lv)
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

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            if (indices != null && verts != null)
            {
                if (verts.Count > 0)
                {
                    effect.TextureEnabled = textureEnabled;

                    if (textureEnabled)
                        if (ContentVault.Textures.ContainsKey(textureName))
                        {
                            effect.Texture = ContentVault.Textures[textureName];
                            if (alpha != null)
                                alpha.Texture = effect.Texture;
                        }
                        else
                        {
                            //Console.WriteLine("missing texture: " + textureName);
                            effect.Texture = ContentVault.Textures["test"];
                            if (alpha != null)
                                alpha.Texture = effect.Texture;
                        }

                    if (!CullingEnabled || !EngineSettings.Instance.BackFaceCulling)
                        Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);

                    foreach (var pass in (alpha != null ? alpha.CurrentTechnique.Passes : effect.CurrentTechnique.Passes))
                    {
                        pass.Apply();

                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            verts_sealed, 0, verts_sealed.Length,
                            indices, 0, indices.Length / 3,
                            VertexPositionColorTexture.VertexDeclaration
                        );
                    }

                    if (EngineSettings.Instance.DrawWireframe)
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
                    Console.WriteLine("Empty QuadList!");
                }
            }
        }
    }
}