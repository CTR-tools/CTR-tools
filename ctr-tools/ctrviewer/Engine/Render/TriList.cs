﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public enum TriListType
    {
        Basic,
        Water,
        Animated,
        Alpha,
        Flag
    }

    public class TriList : IRenderable
    {
        public TriListType type = TriListType.Basic;
        public BlendState blendState = BlendState.Opaque;

        public bool Sealed = false;
        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        private VertexPositionColorTexture[] verts_sealed;
        public List<Color> beginColor = new List<Color>();
        public List<Color> endColor = new List<Color>();
        public Dictionary<int, List<Color>> animatedColors = new Dictionary<int, List<Color>>();
        public bool vColAnimEnabled = false;
        public bool textureEnabled = true;
        public bool ScrollingEnabled = false;
        public bool CullingEnabled = true;
        public string textureName = "";
        private Texture2D texture;
        private Texture2D replacement;
        private short[] indices;

        public int numVerts => verts.Count;

        public int numFaces => indices.Length / 3;

        public void SetColor(Color c)
        {
            for (int i = 0; i < numVerts; i++)
            {
                var v = verts[i];
                v.Color = c;
                verts[i] = v;
            }
        }

        public void Move(Vector3 move)
        {
            for (int i = 0; i < numVerts; i++)
            {
                var v = verts[i];
                v.Position += move;
                verts[i] = v;
            }
        }

        public TriList()
        {
        }

        public TriList(List<VertexPositionColorTexture> v, bool te = false, string name = "")
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
            indices = GenerateIndices();
        }


        public void Seal()
        {
            indices = GenerateIndices();
            verts_sealed = verts.ToArray();
            Sealed = true;

            texture = ContentVault.GetTexture(textureName, false);
            replacement = ContentVault.GetTexture(textureName, EngineSettings.Instance.UseTextureReplacements);
            if (replacement is null)
                replacement = texture;

            /*
            for (int i = 0; i < numVerts; i++)
            {
                beginColor.Add(verts[i].Color);
                endColor.Add(Color.White);
            }
            */
        }

        public void PushTri(List<VertexPositionColorTexture> lv, bool fixOrder = false)
        {
            if (Sealed)
            {
                GameConsole.Write("Trying to update sealed list.");
                return;
            }

            if (lv != null)
            {
                if (fixOrder)
                {
                    var x = lv[2];
                    lv[2] = lv[1];
                    lv[1] = x;
                }

                verts.AddRange(lv);
            }
        }

        public void PushQuad(List<VertexPositionColorTexture> lv, bool fixOrder = false)
        {
            if (Sealed)
            {
                GameConsole.Write("Trying to update sealed list.");
                return;
            }

            if (lv != null)
            {
                for (int i = 0; i < lv.Count / 4; i++)
                    if (!fixOrder)
                    {
                        verts.AddRange(new List<VertexPositionColorTexture>() { lv[0 + i * 4], lv[1 + i * 4], lv[2 + i * 4], lv[2 + i * 4], lv[1 + i * 4], lv[3 + i * 4] });
                    }
                    else
                    {
                        verts.AddRange(new List<VertexPositionColorTexture>() { lv[0 + i * 4], lv[2 + i * 4], lv[1 + i * 4], lv[2 + i * 4], lv[0 + i * 4], lv[3 + i * 4] });
                    }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!vColAnimEnabled && !ScrollingEnabled)
                return;

            if (ScrollingEnabled)
            {
                for (int i = 0; i < numVerts; i++)
                {
                    verts_sealed[i].TextureCoordinate.Y -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f * 3; //this will potentially overflow
                }
            }

            /*
            if (vColAnimEnabled)
            {
                for (int i = 0; i < numVerts; i++)
                {
                    verts_sealed[i].Color = Color.Lerp(beginColor[i], endColor[i], forward ? lerp : 1 - lerp);
                }
            }

            lerp += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;
            if (lerp > 1)
            {
                lerp -= 1;
                forward = !forward;
            }
            */
        }

        public short[] GenerateIndices()
        {
            var indices = new short[numVerts];

            for (int i = 0; i < numVerts; i++)
                indices[i] = (short)i;

            return indices;
        }

        public void DrawTest(GraphicsDeviceManager graphics, Effect effect)
        {

            if (effect == null)
            {
                GameConsole.Write("missing shader");
            }

            else
            {
                if (textureEnabled)
                {

                    effect.Parameters["VertexColorEnabled"]?.SetValue(1);
                    effect.Parameters["bDiffuseMapEnabled"]?.SetValue(1);
                    effect.Parameters["DiffuseMap"]?.SetValue(texture);
                    effect.Parameters["bNormalMapEnabled"]?.SetValue(1);
                    effect.Parameters["NormalMap"]?.SetValue(ContentVault.GetTexture("normalmap", false));
                }

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        verts_sealed, 0, numVerts,
                        indices, 0, numFaces,
                        VertexPositionColorTexture.VertexDeclaration
                    );
                }
            }
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            if (indices is null || verts is null)
            {
                GameConsole.Write("Bad trilist.");
                return;
            }

            if (indices.Length == 0 || verts.Count == 0)
            {
                GameConsole.Write("Empty trilist.");
                return;
            }

            effect.VertexColorEnabled = EngineSettings.Instance.VertexLighting;

            if (!textureEnabled)
                effect.DiffuseColor /= 2;

            graphics.GraphicsDevice.BlendState = blendState;
            if (type == TriListType.Water)
                graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            effect.TextureEnabled = EngineSettings.Instance.Textures ? textureEnabled : false;

            if (textureEnabled)
                if (ContentVault.Textures.ContainsKey(textureName))
                {
                    //effect.Texture = ContentVault.GetTexture(textureName, EngineSettings.Instance.UseTextureReplacements);
                    effect.Texture = EngineSettings.Instance.UseTextureReplacements ? replacement : texture;
                    if (alpha != null)
                        alpha.Texture = effect.Texture;
                }
                else
                {
                    //Console.WriteLine("missing texture: " + textureName);
                    effect.Texture = ContentVault.GetTexture("test", false);
                    if (alpha != null)
                        alpha.Texture = effect.Texture;
                }

            if (!CullingEnabled || !EngineSettings.Instance.BackFaceCulling)
                Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);

            if (type == TriListType.Water || type == TriListType.Flag)
            {
                effect.Alpha = 0.5f;
                if (alpha != null)
                    alpha.Alpha = 0.5f;
            }

            /*
            if (blendState == BlendState.AlphaBlend)
            {
                effect.Alpha = 1f;
                if (alpha != null)
                    alpha.Alpha = 1f;
            }*/


            Effect tempeffect = effect;

            if (alpha != null && EngineSettings.Instance.Textures)
                tempeffect = alpha;

            foreach (var pass in tempeffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    verts_sealed, 0, numVerts,
                    indices, 0, numFaces,
                    VertexPositionColorTexture.VertexDeclaration
                );
            }

            if (type == TriListType.Water || type == TriListType.Flag)
            {
                effect.Alpha = 1f;
                if (alpha != null)
                    alpha.Alpha = 1f;
            }

            /*
            if (blendState == BlendState.AlphaBlend)
            {
                effect.Alpha = 1f;
                if (alpha != null)
                    alpha.Alpha = 1f;
            }
            */

            if (EngineSettings.Instance.DrawWireframe)
            {
                effect.TextureEnabled = false;

                Samplers.SetToDevice(graphics, EngineRasterizer.Wireframe);

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        verts_sealed, 0, numVerts,
                        indices, 0, numFaces,
                        VertexPositionColorTexture.VertexDeclaration
                    );
                }
            }

            if (!textureEnabled)
                effect.DiffuseColor *= 2;

            Samplers.SetToDevice(graphics, EngineRasterizer.Default);
        }
    }
}