using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class Vector4b
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public Vector4b(BinaryReader br)
        {
            X = br.ReadByte();
            Y = br.ReadByte();
            Z = br.ReadByte();
            W = br.ReadByte();
        }

        public string ToObj() { return "v " + ToString(); }
        public override string ToString() { return X + " " + Y + " " + Z; }
    }
}
