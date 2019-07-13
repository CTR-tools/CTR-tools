using System.IO;
using System.ComponentModel;

namespace model_reader
{
    class BoundingBox : IRead, IWrite
    {
        [CategoryAttribute("Values"), DescriptionAttribute("Mininum.")]
        public Vector3s Min
        {
            get { return min; }
            set { Min = value; }
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Maximum.")]
        public Vector3s Max
        {
            get { return min; }
            set { Min = value; }
        }


        private Vector3s min;
        private Vector3s max;


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


        /*
        public int Max(int x, int y, int z)
        {
            int max = x;
            if (y > max) max = y;
            if (z > max) max = z;
            return max;
        }

        public int Min(int x, int y, int z)
        {
            int min = x;
            if (y < min) min = y;
            if (z < min) min = z;
            return min;
        }
        */

    }
}