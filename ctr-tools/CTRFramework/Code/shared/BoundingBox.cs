using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace CTRFramework.Shared
{
    public class CtrBoundingBox : IReadWrite
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

        public Vector3 numericMin = Vector3.Zero;
        public Vector3 numericMax = Vector3.Zero;


        private Vector3s min;
        private Vector3s max;

        public Vector3f minf;
        public Vector3f maxf;

        public CtrBoundingBox()
        {
            min = new Vector3s(short.MaxValue);
            max = new Vector3s(short.MinValue);
        }

        public CtrBoundingBox(Vector3s vmin, Vector3s vmax)
        {
            min = vmin.Clone();
            max = vmax.Clone();
        }

        public CtrBoundingBox(Vector3f vf)
        {
            minf = vf.Clone();
            maxf = vf.Clone();

            min = minf.ToVector3s();
            max = maxf.ToVector3s();
        }

        public CtrBoundingBox(Vector3f vmin, Vector3f vmax)
        {
            minf = vmin.Clone();
            maxf = vmax.Clone();

            min = minf.ToVector3s();
            max = maxf.ToVector3s();
        }

        public CtrBoundingBox(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CtrBoundingBox FromReader(BinaryReaderEx br)
        {
            return new CtrBoundingBox(br);
        }

        public void Read(BinaryReaderEx br)
        {
            numericMin = br.ReadVector3s(Helpers.GteScaleSmall);
            numericMax = br.ReadVector3s(Helpers.GteScaleSmall);

            br.Seek(-2 * 3 * 2);

            min = new Vector3s(br);
            max = new Vector3s(br);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            min.Write(bw);
            max.Write(bw);
        }

        public static CtrBoundingBox operator +(CtrBoundingBox a, Vector3f b)
        {
            return new CtrBoundingBox(a.minf + b, a.maxf + b);
        }
        public static CtrBoundingBox operator -(CtrBoundingBox a, Vector3f b)
        {
            return new CtrBoundingBox(a.minf - b, a.maxf - b);
        }

        public override string ToString()
        {
            return "BB: min " + numericMin.ToString() + " max " + numericMax.ToString();
        }

        public CtrBoundingBox Clone()
        {
            return new CtrBoundingBox(Min, Max);
        }



        public static CtrBoundingBox GetBB(List<Vector3f> vertices)
        {
            if (vertices.Count == 0)
                return null;

            CtrBoundingBox bb = new CtrBoundingBox(vertices[0]);

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