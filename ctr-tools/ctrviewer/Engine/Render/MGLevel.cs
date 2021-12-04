using CTRFramework;
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

        public MGLevel(SkyBox sb)
        {
            TriList skybox = new TriList();

            skybox.textureEnabled = false;
            skybox.ScrollingEnabled = false;

            for (int i = 0; i < sb.Faces.Count; i++)
            {
                List<VertexPositionColorTexture> tri = new List<VertexPositionColorTexture>();
                tri.Add(DataConverter.ToVptc(sb.Vertices[(int)sb.Faces[i].X], System.Numerics.Vector2.Zero));
                tri.Add(DataConverter.ToVptc(sb.Vertices[(int)sb.Faces[i].Y], System.Numerics.Vector2.Zero));
                tri.Add(DataConverter.ToVptc(sb.Vertices[(int)sb.Faces[i].Z], System.Numerics.Vector2.Zero));

                skybox.PushTri(tri);
            }

            skybox.Seal();
            skybox.type = TriListType.Basic;

            Trilists.Add("test", skybox);

            wire = new TriList(skybox);
            wire.SetColor(Color.Red);
        }

        public void Push(Dictionary<string, TriList> dict, string name, List<VertexPositionColorTexture> monolist, TriListType type = TriListType.Basic, BlendState blendState = null, string custTex = "")
        {
            if (blendState == null)
                blendState = BlendState.Opaque;

            if (!dict.ContainsKey(name))
            {
                TriList ql = new TriList(new List<VertexPositionColorTexture>() { }, true, (custTex != "" ? custTex : name));
                ql.blendState = blendState;
                ql.type = type;
                dict.Add(name, ql);
            }

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

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            Samplers.SetToDevice(graphics, EngineSampler.Default);

            //foreach (var ql in normalq)
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Basic)
                    ql.Value.Draw(graphics, effect, null);

            Samplers.SetToDevice(graphics, EngineSampler.Animated);

            //foreach (var ql in animatedq)
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Animated)
                    ql.Value.Draw(graphics, effect, null);

            Samplers.SetToDevice(graphics, EngineSampler.Default);


            if (flagq.ContainsKey(((QuadFlags)(1 << Game1.currentflag)).ToString()))
                flagq[((QuadFlags)(1 << Game1.currentflag)).ToString()].Draw(graphics, effect, alpha);



            effect.Alpha = 0.5f;

            if (EngineSettings.Instance.ShowWater)
            {
                //foreach (var ql in waterq)
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

        public void DrawSky(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            foreach (var ql in Trilists)
                if (ql.Value.type == TriListType.Basic)
                    ql.Value.Draw(graphics, effect, alpha);
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