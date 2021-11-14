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

        public static VertexPositionColorTexture ToVptc(CTRFramework.Vertex v, System.Numerics.Vector2 uv, float scale = 1.0f)
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

        public static TriList ToTriList(CTRFramework.CtrModel model, float scale = 1f)
        {
            GameConsole.Write(model.Name);

            List<VertexPositionColorTexture> li = new List<VertexPositionColorTexture>();

            foreach (var x in model.Entries[0].verts)
                li.Add(DataConverter.ToVptc(x, new System.Numerics.Vector2(0, 0), 0.01f * scale));

            TriList t = new TriList();
            t.textureEnabled = false;
            t.textureName = "test";
            t.ScrollingEnabled = false;
            t.PushTri(li);
            t.Seal();

            return t;
        }
    }
}