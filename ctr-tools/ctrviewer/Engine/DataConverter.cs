using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer.Engine
{
    class DataConverter
    {
        public static Vector3 ToVector3(CTRFramework.Shared.Vector3s vector, float scale = 1.0f)
        {
            return new Vector3(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        public static Vector3 ToVector3(System.Numerics.Vector3 vector, float scale = 1.0f)
        {
            return new Vector3(vector.X, vector.Y, vector.Z) * scale;
        }
        public static Vector3 ToVector3(CTRFramework.Shared.Vector4s s, float scale = 1.0f)
        {
            return new Vector3(s.X * scale, s.Y * scale, s.Z * scale);
        }

        public static Color ToColor(CTRFramework.Shared.Vector4b s)
        {
            return new Color(s.X, s.Y, s.Z, s.W);
        }

        public static VertexPositionColorTexture[] ToLineList(CTRFramework.Shared.BoundingBox bbox, Color color)
        {
            Vector3 min = new Vector3(bbox.Min.X, bbox.Min.Y, bbox.Min.Z) / 100f;
            Vector3 max = new Vector3(bbox.Max.X, bbox.Max.Y, bbox.Max.Z) / 100f;

            return new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture(new Vector3(min.X, min.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, min.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, min.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, max.Y, min.Z), color, new Vector2(0,0)),

                new VertexPositionColorTexture(new Vector3(max.X, max.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, max.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, max.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, min.Y, min.Z), color, new Vector2(0,0)),

                new VertexPositionColorTexture(new Vector3(min.X, min.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, min.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, min.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, max.Y, max.Z), color, new Vector2(0,0)),

                new VertexPositionColorTexture(new Vector3(max.X, max.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, max.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, max.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, min.Y, max.Z), color, new Vector2(0,0)),


                new VertexPositionColorTexture(new Vector3(max.X, min.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, min.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, max.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(max.X, max.Y, max.Z), color, new Vector2(0,0)),

                new VertexPositionColorTexture(new Vector3(min.X, max.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, max.Y, max.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, min.Y, min.Z), color, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(min.X, min.Y, max.Z), color, new Vector2(0,0))
            };
        }

        public static VertexPositionColorTexture ToVptc(CTRFramework.Vertex v, CTRFramework.Shared.Vector2b uv, float scale = 1.0f)
        {
            VertexPositionColorTexture mono_v = new VertexPositionColorTexture();
            mono_v.Position = ToVector3(v.coord, scale);
            mono_v.Color = new Color(
                v.color.X / 255f,
                v.color.Y / 255f,
                v.color.Z / 255f
                );
            mono_v.TextureCoordinate = new Microsoft.Xna.Framework.Vector2(uv.X / 255.0f, uv.Y / 255.0f);
            return mono_v;
        }
    }
}