using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Drawing;

namespace model_reader
{
    class CTRVertex
    {
        public Vector4s coord;
        public Vector4b color;
        public Vector4b color2;    //what's this?

        public CTRVertex(BinaryReader br)
        {
            coord = new Vector4s(br);
            color = new Vector4b(br);
            color2 = new Vector4b(br);
        }

        /*
        public void Write(BinaryWriter bw)
        {
            coord.Write(bw);
            color.Write(bw);
            color2.Write(bw);
        }

        public void SetColor1(Color x)
        {
            color.X = x.R;
            color.Y = x.G;
            color.Z = x.B;
            color.W = 0;
        }

        public void SetColor2(Color x)
        {
            color2.X = x.R;
            color2.Y = x.G;
            color2.Z = x.B;
            color2.W = 0;
        }*/

        public string ToString(bool flag)
        {
            return coord.ToString() + " " + (flag ? " 0 0 255" : color.ToString());
        }

        public string ToString(byte b)
        {
            return coord.ToString() + " " + b + " " + b + " " + b;
        }
    }
}
