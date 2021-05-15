using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CTRFramework.Shared
{
    public class BoundingBox : IReadWrite
    {
        [CategoryAttribute("Values"), DescriptionAttribute("Mininum.")]
        public Vector3s Min
        {
            get => min;
            set => Min = value;
        }

        [CategoryAttribute("Values"), DescriptionAttribute("Maximum.")]
        public Vector3s Max
        {
            get => max;
            set => Max = value;
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

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
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
        public BoundingBox GetBB(List<Vertex> vertices)
        {
            BoundingBox bb_new = new BoundingBox();

            foreach (var v in vertices)
            {
                if (v.coord.X < bb_new.Min.X) bb_new.Min.X = v.coord.X;
                if (v.coord.X > bb_new.Max.X) bb_new.Max.X = v.coord.X;

                if (v.coord.Y < bb_new.Min.Y) bb_new.Min.Y = v.coord.Y;
                if (v.coord.Y > bb_new.Max.Y) bb_new.Max.Y = v.coord.Y;

                if (v.coord.Z < bb_new.Min.Z) bb_new.Min.Z = v.coord.Z;
                if (v.coord.Z > bb_new.Max.Z) bb_new.Max.Z = v.coord.Z;
            }

            return bb_new;
        }
        */
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