using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class BoundingBox : IRead, IWrite
    {
        public Vector3s min;
        public Vector3s max;

        public BoundingBox(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            min = new Vector3s(br);
            max = new Vector3s(br);
        }

        public void Write(BinaryWriter bw)
        {
            min.Write(bw);
            max.Write(bw);
        }

        public override string ToString()
        {
            return "BB: min " + min.ToString() + " max " + max.ToString();
        }
    }
}
