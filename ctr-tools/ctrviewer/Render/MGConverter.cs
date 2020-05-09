using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ctrviewer
{
    class MGConverter
    {
        public static Vector3 ToVector3(CTRFramework.Shared.Vector3s s)
        {
            return new Vector3(s.X, s.Y, s.Z);
        }

        public static Color ToColor(CTRFramework.Shared.Vector4b s)
        {
            return new Color(s.X, s.Y, s.Z, s.W);
        }

        public static VertexPositionColorTexture ToVptc(CTRFramework.Vertex v, CTRFramework.Shared.Vector2b uv)
        {
            VertexPositionColorTexture mono_v = new VertexPositionColorTexture();
            mono_v.Position = new Microsoft.Xna.Framework.Vector3(v.coord.X, v.coord.Y, v.coord.Z);
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