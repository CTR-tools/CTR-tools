using CTRFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine.Render
{
    public class MGLevel : IRenderable
    {
        public TriList wire = new TriList();

        public Dictionary<string, TriList> normalq = new Dictionary<string, TriList>();
        public Dictionary<string, TriList> waterq = new Dictionary<string, TriList>();
        public Dictionary<string, TriList> alphaq = new Dictionary<string, TriList>();
        public Dictionary<string, TriList> animatedq = new Dictionary<string, TriList>();

        public TriList wireq = new TriList();

        public Dictionary<string, TriList> flagq = new Dictionary<string, TriList>();

        public MGLevel()
        {
        }

        public List<string> textureList
        {
            get
            {
                List<string> list = new List<string>();

                foreach (var n in normalq)
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                foreach (var n in alphaq)
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                foreach (var n in animatedq)
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                foreach (var n in waterq)
                    if (!list.Contains(n.Key))
                        list.Add(n.Key);

                return list;
            }
        }

        public MGLevel(SkyBox sb)
        {
            TriList normal = new TriList();

            normal.textureEnabled = false;
            normal.ScrollingEnabled = false;

            for (int i = 0; i < sb.faces.Count; i++)
            {
                List<VertexPositionColorTexture> tri = new List<VertexPositionColorTexture>();
                tri.Add(DataConverter.ToVptc(sb.verts[(int)sb.faces[i].X], new CTRFramework.Shared.Vector2b(0, 0)));
                tri.Add(DataConverter.ToVptc(sb.verts[(int)sb.faces[i].Y], new CTRFramework.Shared.Vector2b(0, 0)));
                tri.Add(DataConverter.ToVptc(sb.verts[(int)sb.faces[i].Z], new CTRFramework.Shared.Vector2b(0, 0)));

                normal.PushTri(tri);
            }

            normal.Seal();

            normalq.Add("test", normal);

            wire = new TriList(normal);
            wire.SetColor(Color.Red);
        }

        public void Push(Dictionary<string, TriList> dict, string name, List<VertexPositionColorTexture> monolist, string custTex = "")
        {
            if (!dict.ContainsKey(name))
            {
                TriList ql = new TriList(new List<VertexPositionColorTexture>() { }, true, (custTex != "" ? custTex : name));
                dict.Add(name, ql);
            }

            dict[name].PushQuad(monolist);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var ql in animatedq)
                ql.Value.Update(gameTime);
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            Samplers.SetToDevice(graphics, EngineSampler.Default);

            foreach (var ql in normalq)
                ql.Value.Draw(graphics, effect, null);

            Samplers.SetToDevice(graphics, EngineSampler.Animated);

            foreach (var ql in animatedq)
                ql.Value.Draw(graphics, effect, null);

            Samplers.SetToDevice(graphics, EngineSampler.Default);

            graphics.GraphicsDevice.BlendState = BlendState.Additive;

            if (flagq.ContainsKey(((QuadFlags)(1 << Game1.currentflag)).ToString()))
                flagq[((QuadFlags)(1 << Game1.currentflag)).ToString()].Draw(graphics, effect, alpha);

            graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            foreach (var ql in alphaq)
                ql.Value.Draw(graphics, effect, alpha);

            effect.Alpha = 0.5f;

            if (!Game1.HideWater)
            {
                foreach (var ql in waterq)
                    ql.Value.Draw(graphics, effect, null);
            }

            if (!Game1.HideInvisible)
            {
                if (flagq.ContainsKey("invis"))
                    flagq["invis"].Draw(graphics, effect, null);
            }

            effect.Alpha = 1f;
        }

        public void DrawSky(GraphicsDeviceManager graphics, BasicEffect effect, AlphaTestEffect alpha)
        {
            foreach (var list in normalq)
                list.Value.Draw(graphics, effect, alpha);
        }

        public void Seal()
        {
            foreach (var ql in normalq)
                ql.Value.Seal();

            foreach (var ql in waterq)
                ql.Value.Seal();

            foreach (var ql in alphaq)
                ql.Value.Seal();

            foreach (var ql in animatedq)
                ql.Value.Seal();

            foreach (var ql in flagq)
                ql.Value.Seal();
        }
    }
}