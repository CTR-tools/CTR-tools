using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

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

    public class AnimatedVertexBuffer
    {
        public string Name = "default";
        public bool Sealed = false;

        public int totalFrames = 0;
        public int frameSize = 0;

        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        public VertexPositionColorTexture[] verts_sealed;

        public void PushTri(List<VertexPositionColorTexture> v)
        {
            verts.AddRange(v);
        }

        public void Seal()
        {
            if (!Sealed)
            {
                verts_sealed = verts.ToArray();
                verts.Clear();
            }
        }
    }

    public class AnimatedVertexBufferPlayer
    {
        public AnimatedVertexBuffer buffer;

        public bool loopEnabled = true;
        public bool IsPlaying = false;

        public int curFrame = 0;

        public float frameDuration = 0.5f;
        public float curAnimTime = 0;

        public float interpDistance = 0;

        public void Reset()
        {
            curAnimTime = 0;
            curFrame = 0;
            interpDistance = 0;
        }

        public void FrameAdvance(GameTime gameTime)
        {
            if (!IsPlaying) return;

            curAnimTime += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

            while (curAnimTime > frameDuration)
            {
                curFrame++;
                curAnimTime -= frameDuration;
            }

            //keep track of how much we advanced between the frames
            interpDistance = curAnimTime / frameDuration;

            if (curFrame >= buffer.totalFrames)
            {
                curAnimTime = 0;

                if (!loopEnabled)
                {
                    IsPlaying = false;
                    return;

                    //we dont update curFrame, so it can stay at the last frame.
                }

                curFrame = 0;
            }
        }
    }

    public class TexturedIndexBuffer
    {
        public bool Sealed { get; private set; } = false;

        //todo: refactor to a material class, so can just swap material on the fly
        
        public string textureName = "";

        public Texture2D Texture { get; private set; }
        public Texture2D AltTexture { get; private set; }

        public bool IsAlpha = false;


        public List<short> indices = new List<short>();
        public short[] indices_sealed { get; private set; }

        public int numFaces { get; private set; } = 0;

        public TexturedIndexBuffer Clone()
        {
            return new TexturedIndexBuffer()
            {
                textureName = this.textureName,
                Sealed = this.Sealed,
                Texture = this.Texture,
                AltTexture = this.AltTexture,
                indices_sealed = this.indices_sealed
            };
        }

        public void Seal()
        {
            Texture = ContentVault.GetTexture(textureName, false);
            AltTexture = ContentVault.GetTexture(textureName, EngineSettings.Instance.UseTextureReplacements);

            if (ContentVault.alphalist.Contains(textureName)) IsAlpha = true;

            indices_sealed = indices.ToArray();
            numFaces = indices_sealed.Length / 3;

            indices.Clear();

            Sealed = true;
        }

        /// <summary>
        /// Adds a triangle to the list.
        /// </summary>
        public void PushTri(int x, int y, int z)
        {
            if (Sealed)
            {
                GameConsole.Write("Attempeted to push tri to a sealed trilist!");
                return;
            }

            indices.Add((short)x);
            indices.Add((short)y);
            indices.Add((short)z);
        }
    }

    //a super class to render entire textured model
    public class TriList : IRenderable
    {
        public TriListType type = TriListType.Basic;
        public BlendState blendState = BlendState.Opaque;

        public bool Sealed = false;
        public List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
        private VertexPositionColorTexture[] verts_sealed;


        public AnimatedVertexBuffer anim = null;
        public List<AnimatedVertexBuffer> animsList = new List<AnimatedVertexBuffer>();

        public List<Color> beginColor = new List<Color>();
        public List<Color> endColor = new List<Color>();
        public Dictionary<int, List<Color>> animatedColors = new Dictionary<int, List<Color>>();
        public bool vColAnimEnabled = false;
        public bool textureEnabled = true;
        public bool ScrollingEnabled = false;
        public bool CullingEnabled = true;

        public List<TexturedIndexBuffer> indexBuffers = new List<TexturedIndexBuffer>();

        public int numVerts { get; private set; } = 0;

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

        //used in MGLevel building
        public TriList(List<VertexPositionColorTexture> v, bool te = false, string name = "test")
        {
            verts.AddRange(v);
            textureEnabled = te;

            var buf = new TexturedIndexBuffer() { textureName = name };
            buf.indices = GenerateIndices(verts.Count);
        }

        //todo: used for skybox only for whatever reason, check why
        public TriList(TriList t)
        {
            verts.AddRange(t.verts);
            textureEnabled = t.textureEnabled;

            foreach (var buf in t.indexBuffers)
            {
                indexBuffers.Add(buf.Clone());
            }
        }

        public TexturedIndexBuffer GetIndexBuffer(string texName)
        {
            // ptr intex buffer
            TexturedIndexBuffer buf = null;

            //lookup for index buffer by texture name
            foreach (var list in indexBuffers)
            {
                if (list.textureName == texName)
                {
                    buf = list;
                    break;
                }
            }

            //if missing, add a new one
            if (buf == null)
            {
                buf = new TexturedIndexBuffer() { textureName = texName };
                indexBuffers.Add(buf);
            }

            return buf;
        }

        /// <summary>
        /// Seal method intended to lock up the trilist, so we can just throw the baked data into the graphics pipeline.
        /// The intended usage is to populate all the required data of the object and then seal it.
        /// </summary>
        public void Seal()
        {
            if (Sealed)
            {
                GameConsole.Write("Attempted to seal a sealed trilist.");
                return;
            }

            //seal all index buffers
            foreach (var mat in indexBuffers)
                mat.Seal();

            //seal vertex buffer
            verts_sealed = verts.ToArray();
            numVerts = verts_sealed.Length;
            verts.Clear();


            foreach (var anim in animsList)
                anim.Seal();

            if (anim == null && animsList.Count > 0)
                anim = animsList[0];

            //we're good to go
            Sealed = true;
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
                GameConsole.Write("Attempted to push quad to a sealed list.");
                return;
            }

            if (lv == null) return;

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

        public List<short> GenerateIndices(int numVerts)
        {
            var indices = new List<short>();

            for (int i = 0; i < numVerts; i++)
                indices.Add((short)i);

            return indices;
        }

        /*
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
        */

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {

            //maybe im dumb, make sure matrices are fine
            if (alpha != null)
            {
                alpha.World = effect.World;
                alpha.View = effect.View;
                alpha.Projection = effect.Projection;
            }


            #region [param validation]
            if (indexBuffers is null || verts_sealed is null)
            {
                GameConsole.Write($"Uninitialized trilist!");
                return;
            }

            if (anim == null)
            {
                if (indexBuffers.Count == 0 || verts_sealed.Length == 0)
                {
                    GameConsole.Write("Empty static trilist!");
                    return;
                }
            }
            else
            {
                if (anim.verts_sealed.Length == 0 || indexBuffers.Count == 0)
                {
                    GameConsole.Write("Empty anim trilist!");
                    return;
                }
            }
            #endregion

            if (!textureEnabled)
                effect.DiffuseColor /= 2;

            graphics.GraphicsDevice.BlendState = blendState;
            if (type == TriListType.Water)
                graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //check for global texture setting and use trilist value if true
            effect.TextureEnabled = EngineSettings.Instance.DrawTextures ? textureEnabled : false;

            if (!CullingEnabled || !EngineSettings.Instance.BackFaceCulling)
            {
                Samplers.SetToDevice(graphics, EngineRasterizer.DoubleSided);
            }
            else
            {
                Samplers.SetToDevice(graphics, EngineRasterizer.Default);
            }

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


            //actual object rendering here

            Effect fx = effect;

            foreach (var mat in indexBuffers)
            {
                //set current texture
                if (EngineSettings.Instance.DrawTextures)
                {
                    effect.Texture = EngineSettings.Instance.UseTextureReplacements && mat.AltTexture != null ? mat.AltTexture : mat.Texture;

                    if (alpha != null)
                        alpha.Texture = effect.Texture;
                }

                //maybe transparent?
                if (alpha != null && mat.IsAlpha)
                    fx = alpha;
                else
                    fx = effect;

                //anim legth fail check. should be animplayer dependent instead
                if (anim != null)
                    if (anim.verts_sealed.Length < anim.frameSize * (int)Game1.frame + 1)
                        continue;

                foreach (var pass in fx.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        anim == null ? verts_sealed : anim.verts_sealed, anim == null ? 0 : anim.frameSize * (int)Game1.frame, anim == null ? numVerts : anim.frameSize,
                        mat.indices_sealed, 0, mat.numFaces,
                        VertexPositionColorTexture.VertexDeclaration
                    );
                }
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

                    foreach (var mat in indexBuffers)
                    {
                        graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            anim == null ? verts_sealed : anim.verts_sealed, anim == null ? 0 : anim.frameSize * (int)Game1.frame, anim == null ? numVerts : anim.frameSize,
                            mat.indices_sealed, 0, mat.numFaces,
                            VertexPositionColorTexture.VertexDeclaration
                        );
                    }
                }

                if (alpha != null)
                    foreach (var pass in alpha.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        foreach (var mat in indexBuffers)
                        {
                            graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                anim == null ? verts_sealed : anim.verts_sealed, anim == null ? 0 : anim.frameSize * (int)Game1.frame, anim == null ? numVerts : anim.frameSize,
                                mat.indices_sealed, 0, mat.numFaces,
                                VertexPositionColorTexture.VertexDeclaration
                            );
                        }
                    }
            }

            if (!textureEnabled)
                effect.DiffuseColor *= 2;

            Samplers.SetToDevice(graphics, EngineRasterizer.Default);
        }
    }
}