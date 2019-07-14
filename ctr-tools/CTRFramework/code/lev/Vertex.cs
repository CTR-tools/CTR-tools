using System;
using System.IO;

namespace CTRFramework
{
    class Vertex : IRead, IWrite
    {
        public Vector4s coord;
        public Vector4b color;
        public Vector4b color_morph;    //what's this?

        public static Vector4b flagColor = new Vector4b(0x0000FF00); //blue for flags

        public Vertex(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            coord = new Vector4s(br);
            color = new Vector4b(br);
            color_morph = new Vector4b(br);
        }
        
        public void Write(BinaryWriter bw)
        {
            coord.Write(bw);
            color.Write(bw);
            color_morph.Write(bw);
        }

        public string ToString(bool flag)
        {
            string fmt = "v {0} {1}";

            return String.Format( fmt,
                coord.ToString(VecFormat.Numbers),
                (flag ? flagColor : color).ToString(VecFormat.Numbers)
            );
        }

        public string ToString(byte b)
        {
            return coord.ToString() + " " + b + " " + b + " " + b;
        }
    }
}
