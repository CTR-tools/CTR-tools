using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    class Vector4s
    {
        public short X;
        public short Y;
        public short Z;
        public short W;

        public Vector4s(BinaryReader br)
        {
            X = br.ReadInt16();
            Y = br.ReadInt16();
            Z = br.ReadInt16();
            W = br.ReadInt16();
        }

        public  string ToString(float scale ) { return X * scale  + " " + Y * scale + " " + Z * scale; }
    }
}
