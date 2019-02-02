using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    public class Vector3s
    {
        public short X;
        public short Y;
        public short Z;

        public Vector3s(byte[] x)
        {
            if (x.Length != 6)
            {
                Console.WriteLine("WARNING Vector3s length is not 6");
            }

            MemoryStream ms = new MemoryStream(x);
            BinaryReader br = new BinaryReader(ms);

            X = br.ReadInt16();
            Y = br.ReadInt16();
            Z = br.ReadInt16();
        }

        public Vector3s(BinaryReader br)
        {
            X = br.ReadInt16();
            Y = br.ReadInt16();
            Z = br.ReadInt16();
        }

        public string ToObjVertex()
        {
            return "v " + X + " " + Y + " " + Z;
        }

        public override string ToString()
        {
            return "vec(" + X + "; " + Y + "; " + Z + ");";
        }
    }
}
