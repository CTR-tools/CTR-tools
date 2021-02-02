using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer
{
    class MGConverter
    {
        public static Vector3 ToVector3(CTRFramework.Shared.Vector3s s, float scale = 1.0f)
        {
            return new Vector3(s.X * scale, s.Y * scale, s.Z * scale);
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
            Vector3 min = ToVector3(bbox.Min, 0.01f);
            Vector3 max = ToVector3(bbox.Max, 0.01f);

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

        public static Color Blend(Color c1, Color c2)
        {
            Color x = Color.White;
            x.R = (byte)((c1.R + c2.R) / 2);
            x.G = (byte)((c1.G + c2.G) / 2);
            x.B = (byte)((c1.B + c2.B) / 2);
            return x;
        }

    }
}