using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRFramework
{
    class Vector4b
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public Vector4b(uint a)
        {
            X = (byte)(a >> 24 & 0xFF);
            Y = (byte)(a >> 16 & 0xFF);
            Z = (byte)(a >> 8 & 0xFF);
            W = (byte)(a & 0xFF);
        }

        public Vector4b(BinaryReader br)
        {
            X = br.ReadByte();
            Y = br.ReadByte();
            Z = br.ReadByte();
            W = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(W);
        }

        public string ToString(VecFormat format)
        {
            string fmt = "{0} {1} {2} {3}";

            switch (format)
            {
                case VecFormat.CommaSeparated: fmt = "{0}, {1}, {2}, {3}"; break;
                case VecFormat.Braced: fmt = "({0}, {1}, {2}, {3})"; break;
            }

            return String.Format(fmt, X, Y, Z, W);
        }

        public override string ToString()
        {
            return ToString(VecFormat.Braced);
        }
    }
}
