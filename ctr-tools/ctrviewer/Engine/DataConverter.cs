using CTRFramework;
using CTRFramework.Models;
using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    class DataConverter
    {
        public static Vector3 ToVector3(Vector3s vector, float scale = 1.0f) => new Vector3(vector.X * scale, vector.Y * scale, vector.Z * scale);

        //public static Vector3 ToVector3(Color color) => new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

        public static Vector3 ToVector3(System.Numerics.Vector3 vector, float scale = 1.0f) => new Vector3(vector.X, vector.Y, vector.Z) * scale;

        public static Vector3 ToVector3(Vector4s s, float scale = 1.0f) => new Vector3(s.X * scale, s.Y * scale, s.Z * scale);

        public static Color ToColor(Vector4b s) => new Color(s.X / 255f, s.Y / 255f, s.Z / 255f, 1f);// s.W);

        public static VertexPositionColorTexture ToVptc(Vertex v, System.Numerics.Vector2 uv, float scale = 1.0f)
        {
            return ToVptc(v, uv, Color.Gray, false, scale);
        }

        public static VertexPositionColorTexture ToVptc(Vertex v, System.Numerics.Vector2 uv, Color color, bool lerp = false, float scale = 1.0f)
        {
            return new VertexPositionColorTexture()
            {
                Position = ToVector3(v.Position, scale),
                Color = lerp ? Color.Lerp(color, ToColor(v.Color), 0.5f) : ToColor(v.Color),
                TextureCoordinate = new Microsoft.Xna.Framework.Vector2(uv.X / 255.0f, uv.Y / 255.0f)
            };
        }

        public static TriListCollection ToTriListCollection(CtrModel model, float scale = 1f)
        {
            return ToTriListCollection(model, Color.Gray, false, scale);
        }

        public static TriListCollection ToTriListCollection(CtrModel model, Color color, bool lerp = false, float scale = 1f)
        {
            //GameConsole.Write(model.Name);

            var coll = new TriListCollection();

            var kek = new Dictionary<string, TriList>();

            for (int i = 0; i < model[0].verts.Count / 3; i++)
            {
                var tex = model[0].matIndices[i];

                if (tex is not null)
                    if (tex.ParentLayout is not null)
                        tex = tex.ParentLayout;

                string texture = tex is null ? "test" : tex.Tag;

                if (!kek.ContainsKey(texture))
                    kek.Add(texture, new TriList());

                var li = new List<VertexPositionColorTexture>();

                for (int j = i * 3; j < i * 3 + 3; j++)
                {
                    var vert = model[0].verts[j];
                    li.Add(DataConverter.ToVptc(vert, vert.uv, color, lerp, 0.01f * scale));
                }

                var t = kek[texture];
                t.textureName = texture;
                t.textureEnabled = t.textureName == "test" ? false : true;
                t.ScrollingEnabled = false;
                if (t.textureEnabled)
                    if (model[0].matIndices[i].blendingMode == CTRFramework.Vram.BlendingMode.Additive)
                        t.blendState = BlendState.Additive;

                t.PushTri(li);
            }

            foreach (var list in kek.Values)
                list.Seal();

            coll.AddRange(kek.Values);

            return coll;
        }

        public static SimpleAnimation ToSimpleAnimation(NavPath path)
        {
            var anim = new SimpleAnimation();
            anim.Clear();

            int time = 0;

            foreach (var point in path.Frames)
            {
                anim.Add(new AnimationKey() { Position = ToVector3(point.position), Rotation = Vector3.One, Scale = Vector3.One, Time = time });
                time += 200;
            }

            return anim;
        }

        public static SimpleAnimation ToSimpleAnimation(List<Pose> poses)
        {
            var anim = new SimpleAnimation();
            anim.Clear();

            int time = 0;

            foreach (var point in poses)
            {
                anim.Add(new AnimationKey()
                {
                    Position = ToVector3(point.Position),
                    Rotation = new Vector3(ToVector3(point.Rotation).X, ToVector3(point.Rotation).Y, ToVector3(point.Rotation).Z),
                    Scale = new Vector3(1),
                    Time = time
                });

                time += 1000;
            }

            anim.Add(anim[0]);

            return anim;
        }

        public static SimpleAnimation ToSimpleAnimation(List<RespawnPoint> respawns)
        {
            if (respawns.Count == 0)
                return null;

            var anim = new SimpleAnimation();
            anim.Clear();

            int time = 0;


            var zerospawn = respawns[0];
            var spawn = zerospawn;

            do
            {
                var rot = ToVector3(spawn.Pose.Rotation);

                anim.Add(new AnimationKey()
                {
                    Position = ToVector3(spawn.Pose.Position),
                    Rotation = new Vector3(0),
                    Scale = new Vector3(1f),
                    Time = time
                });

                time += 500;
                spawn = spawn.Next;
            }
            while (spawn != zerospawn && spawn.Next != null);

            anim.Add(new AnimationKey()
            {
                Position = anim[0].Position,
                Rotation = new Vector3(0),//anim.Keys[0].Rotation,
                Scale = new Vector3(1f),
                Time = time + 500
            });

            return anim;
        }
    }
}