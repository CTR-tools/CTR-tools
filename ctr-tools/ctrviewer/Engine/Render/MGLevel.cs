﻿using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class MGLevel : IRenderable
    {
        public TriList wire = new TriList();

        public Dictionary<string, TriList> Trilists = new Dictionary<string, TriList>();

        public Dictionary<string, TriList> flagq = new Dictionary<string, TriList>();

        public BoundingBox boundingBox = new BoundingBox();

        public MGLevel()
        {
        }

        public List<string> textureList
        {
            get
            {
                List<string> list = new List<string>();

                foreach (var n in Trilists)
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                return list;
            }
        }

        public MGLevel(CtrSkyBox sb)
        {
            var skybox = new TriList();

            skybox.textureEnabled = false;
            skybox.ScrollingEnabled = false;

            //we only need 1 buffer for entire scene
            skybox.indexBuffers.Add(new TexturedIndexBuffer() { textureName = "default" });

            int x = 0;

            foreach (var face in sb.Faces)
                for (int i = 0; i < face.Count; i++)
                {
                    var tri = new List<VertexPositionColorTexture>();

                    tri.Add(DataConverter.ToVptc(sb.Vertices[(int)face[i].X], System.Numerics.Vector2.Zero));
                    tri.Add(DataConverter.ToVptc(sb.Vertices[(int)face[i].Y], System.Numerics.Vector2.Zero));
                    tri.Add(DataConverter.ToVptc(sb.Vertices[(int)face[i].Z], System.Numerics.Vector2.Zero));

                    skybox.PushTri(tri);

                    skybox.indexBuffers[0].PushTri(x * 3 + 0, x * 3 + 1, x * 3 + 2);

                    x++;
                }

            skybox.Seal();
            skybox.type = TriListType.Basic;

            Trilists.Add("test", skybox);

            wire = new TriList(skybox);
            wire.SetColor(Color.Red);
        }

        public void Push(Dictionary<string, TriList> dict, string name, List<VertexPositionColorTexture> monolist, TriListType type = TriListType.Basic, BlendState blendState = null, string custTex = "")
        {
            if (blendState is null)
                blendState = BlendState.Opaque;

            if (!dict.ContainsKey(name))
            {
                var ql = new TriList(new List<VertexPositionColorTexture>() { }, true, (custTex != "" ? custTex : name));
                ql.blendState = blendState;
                ql.type = type;
                dict.Add(name, ql);
            }


            var buf = dict[name].GetIndexBuffer(custTex);

            var numVerts = dict[name].verts.Count;

            buf.PushTri(numVerts + 0, numVerts + 1, numVerts + 2);
            buf.PushTri(numVerts + 3, numVerts + 4, numVerts + 5);

            dict[name].PushQuad(monolist);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Animated)
                    ql.Value.Update(gameTime);

            //foreach (var ql in animatedq)
            //    ql.Value.Update(gameTime);
        }

        /*
        public void DrawTest(GraphicsDeviceManager graphics, Effect effect)
        {
            foreach (var ql in Trilists)
                ql.Value.DrawTest(graphics, ContentVault.GetShader("tutorial"));
        }*/


        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            //foreach (var ql in normalq)
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Basic)
                    ql.Value.Draw(graphics, effect, alpha);

            Samplers.SetToDevice(graphics, EngineSampler.Animated);

            //foreach (var ql in animatedq)
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Animated)
                    ql.Value.Draw(graphics, effect, alpha);

            Samplers.SetToDevice(graphics, EngineSampler.Default);


            if (flagq.ContainsKey(((QuadFlags)Game1.currentflag).ToString()))
                flagq[((QuadFlags)Game1.currentflag).ToString()].Draw(graphics, effect, alpha);



            effect.Alpha = 0.5f;

            if (EngineSettings.Instance.ShowWater)
            {
                foreach (var ql in Trilists)
                    if (ql.Value.type == TriListType.Water)
                        ql.Value.Draw(graphics, effect, null);
            }

            if (EngineSettings.Instance.ShowInvisible)
            {
                if (flagq.ContainsKey("invis"))
                    flagq["invis"].Draw(graphics, effect, null);
            }

            effect.Alpha = 1f;


            //foreach (var ql in alphaq)
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Alpha)
                    ql.Value.Draw(graphics, effect, alpha);
        }

        public void DrawSky(GraphicsDeviceManager graphics, FirstPersonCamera camera, BasicEffect effect, AlphaTestEffect alpha)
        {
            effect.Projection = camera.ProjectionMatrix;
            effect.View = camera.ViewMatrix;

            if (alpha != null)
            {
                alpha.Projection = camera.ProjectionMatrix;
                alpha.View = camera.ViewMatrix;
            }

            foreach (var ql in Trilists)
                ql.Value.Draw(graphics, effect, alpha);

            //clear z buffer to make sure skybox is behind everything
            graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Green, 1, 0);
        }

        public void Seal()
        {
            foreach (var ql in Trilists)
                ql.Value.Seal();

            foreach (var ql in flagq)
                ql.Value.Seal();
        }
    }
}