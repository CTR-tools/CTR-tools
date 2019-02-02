using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
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
    }
}
