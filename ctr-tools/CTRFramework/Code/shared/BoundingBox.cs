using System.Collections.Generic;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class BoundingBox : IReadWrite
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
            get { return max; }
            set { Max = value; }
        }


        private Vector3s min;
        private Vector3s max;

        public Vector3f minf;
        public Vector3f maxf;

        public BoundingBox()
        {
            min = new Vector3s(short.MaxValue);
            max = new Vector3s(short.MinValue);
        }

        public BoundingBox(Vector3s vmin, Vector3s vmax)
        {
            min = vmin.Clone();
            max = vmax.Clone();
        }

        public BoundingBox(Vector3f vf)
        {
            minf = vf.Clone();
            maxf = vf.Clone();

            min = minf.ToVector3s();
            max = maxf.ToVector3s();
        }

        public BoundingBox(Vector3f vmin, Vector3f vmax)
        {
            minf = vmin.Clone();
            maxf = vmax.Clone();

            min = minf.ToVector3s();
            max = maxf.ToVector3s();
        }

        public BoundingBox(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            min = new Vector3s(br);
            max = new Vector3s(br);
        }

        public void Write(BinaryWriterEx bw)
        {
            min.Write(bw);
            max.Write(bw);
        }

        public static BoundingBox operator +(BoundingBox a, Vector3f b)
        {
            return new BoundingBox(a.minf + b, a.maxf + b);
        }
        public static BoundingBox operator -(BoundingBox a, Vector3f b)
        {
            return new BoundingBox(a.minf - b, a.maxf - b);
        }

        public override string ToString()
        {
            return "BB: min " + minf.ToString() + " max " + maxf.ToString();
        }

        public BoundingBox Clone()
        {
            return new BoundingBox(Min, Max);
        }



        public static BoundingBox GetBB(List<Vector3f> vertices)
        {
            if (vertices.Count == 0)
                return null;

            BoundingBox bb = new BoundingBox(vertices[0]);

            foreach (var v in vertices)
            {
                bb.minf.Minimize(v);
                bb.maxf.Maximize(v);
            }

            bb.min = bb.minf.ToVector3s();
            bb.max = bb.maxf.ToVector3s();

            return bb;
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