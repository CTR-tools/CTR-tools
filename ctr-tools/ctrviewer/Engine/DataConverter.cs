using CTRFramework;
using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ctrviewer.Engine
{
    class DataConverter
    {
        public static Vector3 ToVector3(Vector3s vector, float scale = 1.0f)
        {
            return new Vector3(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        public static Vector3 ToVector3(Color color)
        {
            return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
        }

        public static Vector3 ToVector3(System.Numerics.Vector3 vector, float scale = 1.0f)
        {
            return new Vector3(vector.X, vector.Y, vector.Z) * scale;
        }
        public static Vector3 ToVector3(Vector4s s, float scale = 1.0f)
        {
            return new Vector3(s.X * scale, s.Y * scale, s.Z * scale);
        }

        public static Color ToColor(Vector4b s)
        {
            return new Color(s.X, s.Y, s.Z, s.W);
        }

        public static VertexPositionColorTexture ToVptc(Vertex v, System.Numerics.Vector2 uv, float scale = 1.0f)
        {
            VertexPositionColorTexture mono_v = new VertexPositionColorTexture();
            mono_v.Position = ToVector3(v.Position, scale);
            mono_v.Color = new Color(
                v.Color.X / 255f,
                v.Color.Y / 255f,
                v.Color.Z / 255f
                );
            mono_v.TextureCoordinate = new Microsoft.Xna.Framework.Vector2(uv.X / 255.0f, uv.Y / 255.0f);
            return mono_v;
        }


        public static TriListCollection ToTriListCollection(CtrModel model, float scale = 1f)
        {
            GameConsole.Write(model.Name);

            TriListCollection coll = new TriListCollection();

            Dictionary<string, TriList> kek = new Dictionary<string, TriList>();

            for (int i = 0; i < model.Entries[0].verts.Count / 3; i++)
            {
                string texture = model.Entries[0].matIndices[i] == null ? "test" : model.Entries[0].matIndices[i].Tag;

                if (!kek.ContainsKey(texture))
                    kek.Add(texture, new TriList());

                List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

                for (int j = i * 3; j < i * 3 + 3; j++)
                {
                    Vertex x = model.Entries[0].verts[j];
                    li.Add(DataConverter.ToVptc(x, x.uv, 0.01f * scale));
                }

                TriList t = kek[texture];
                t.textureName = texture;
                t.textureEnabled = t.textureName == "test" ? false : true;
                t.ScrollingEnabled = false;
                t.PushTri(li);
            }

            foreach (var list in kek.Values)
                list.Seal();

            coll.Entries.AddRange(kek.Values);

            return coll;
        }

        public static SimpleAnimation ToSimpleAnimation(BotPath path)
        {
            var anim = new SimpleAnimation();
            anim.Keys.Clear();

            int time = 0;

            foreach (var point in path.Frames)
            {
                anim.Keys.Add(new AnimationKey() { Position = ToVector3(point.position), Rotation = new Vector3(0), Scale = new Vector3(1), TimeValue = time });
                time += 200;
            }

            anim.State = anim.Keys[0];

            return anim;
        }

        public static SimpleAnimation ToSimpleAnimation(List<Pose> poses)
        {
            var anim = new SimpleAnimation();
            anim.Keys.Clear();

            int time = 0;

            foreach (var point in poses)
            {
                anim.Keys.Add(new AnimationKey()
                {
                    Position = ToVector3(point.Position),
                    Rotation = new Vector3(ToVector3(point.Rotation).X, ToVector3(point.Rotation).Y, ToVector3(point.Rotation).Z),
                    Scale = new Vector3(1),
                    TimeValue = time
                });

                time += 200;
            }

            anim.Keys.Add(anim.Keys[0]);

            anim.State = anim.Keys[0];

            return anim;
        }

        public static RespawnPoint GetByIndex(List<RespawnPoint> respawns, int index)
        {
            foreach (var resp in respawns)
                if (resp.index == index)
                    return resp;

            return null;
        }

        public static SimpleAnimation ToSimpleAnimation(List<RespawnPoint> respawns)
        {
            if (respawns.Count == 0)
            {
                GameConsole.Write("go fuck yourself");
                return null;
            }

            var anim = new SimpleAnimation();
            anim.Keys.Clear();

            int time = 0;


            var zerospawn = GetByIndex(respawns, 0);
            var spawn = zerospawn;

            do
            {
                var rot = ToVector3(spawn.Pose.Rotation);

                anim.Keys.Add(new AnimationKey()
                {
                    Position = ToVector3(spawn.Pose.Position),
                    Rotation = new Vector3(0), //rot.X, rot.Y, rot.Z),
                    Scale = new Vector3(1f),
                    TimeValue = time
                });

                GameConsole.Write($"added key! {spawn.index} {spawn.prevIndex} {spawn.Pose.Position} {time}");

                time += 250;
                spawn = spawn.Next;
            }
            while (spawn != zerospawn);

            anim.Keys.Add(anim.Keys[0]);
            anim.State = anim.Keys[0];

            return anim;
        }
    }
}