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
    }
}