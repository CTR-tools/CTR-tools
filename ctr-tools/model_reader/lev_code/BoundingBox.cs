using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    class BoundingBox
    {
        Vector3s min;
        Vector3s max;

        public BoundingBox(BinaryReader br)
        {
            min = new Vector3s(br);
            max = new Vector3s(br);
        }

        public override string ToString()
        {
            return "BB: min " + min.ToString() + " max " + max.ToString();
        }
    }
}
