using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework.Models
{
    public class CtrFrame
    {
        public Vector3 Offset = new Vector3(0, 0, 0);
        private float OffsetScale => Helpers.GteScaleSmall;

        public int ptrVerts = 0x1C;
        public List<Vector3b> Vertices = new List<Vector3b>();

        public CtrFrame()
        {
        }

        public CtrFrame(BinaryReaderEx br, int numVerts, List<CtrDelta> deltas = null) => Read(br, numVerts, deltas);

        public static CtrFrame FromReader(BinaryReaderEx br, int numVerts, List<CtrDelta> deltas = null) => new CtrFrame(br, numVerts, deltas);

        public void Read(BinaryReaderEx br, int numVerts, List<CtrDelta> deltas = null)
        {
            Offset = br.ReadVector3sPadded(OffsetScale); //new Vector4s(br);

            Helpers.Panic(this, PanicType.Debug, Offset.ToString());

            // skipping 16 zero bytes
            // this is used at runtime
            br.Seek(16);

            ptrVerts = br.ReadInt32();

            Helpers.Panic(this, PanicType.Assume, $"{br.HexPos()} vertOffset: {ptrVerts.ToString("X8")}");

            br.Seek(ptrVerts - 0x1C);

            // no deltas provided, assume raw vertices
            if (deltas == null) 
            {
                for (int i = 0; i < numVerts; i++)
                    Vertices.Add(new Vector3b(br));
            }
            // must be animated frame, decompress first
            else
            {
                // just read a chunk big enough to hold any anim. there will be useless junk in the end, but we won't use it anyways.
                // proper solution should use frame size from the upper level, cause this one may actually overflow
                using (var bs = BitStreamReader.FromByteArray(br.ReadBytes(1024 * 4)))
                {   
                    // provide bitstream and deltas
                    Vertices = DecompressVertices(bs, deltas);
                }
            }
        }

        /// <summary>
        /// Handles frame decompression. Refer to Crash Bandicoot 2 compression format documentation at this repo:
        /// https://github.com/warenhuis/Crash-Bandicoot-2-Modelexport
        /// </summary>
        /// <param name="temporal">Bitstream of temporal data</param>
        /// <param name="deltas">Array of shared deltas</param>
        public List<Vector3b> DecompressVertices(BitStreamReader bs, List<CtrDelta> deltas)
        {
            var result = new List<Vector3b>();

            int X = 0;
            int Y = 0;
            int Z = 0;

            foreach (var delta in deltas)
                result.Add(DeltaToVertex(ref X, ref Y, ref Z, bs, delta));

            return result;
        }

        // converts delta and temporal bits to actual coord
        private Vector3b DeltaToVertex(ref int X, ref int Y, ref int Z, BitStreamReader bs, CtrDelta delta)
        {
            // reset command (111 -> all 3 bits are set)
            if (delta.Bits.X == 7) X = 0;
            if (delta.Bits.Y == 7) Y = 0;
            if (delta.Bits.Z == 7) Z = 0;

            // recover temporal value
            int tX = GetTemporalValue(bs, delta.Bits.X);
            int tY = GetTemporalValue(bs, delta.Bits.Y);
            int tZ = GetTemporalValue(bs, delta.Bits.Z);

            // add temporal value to retrieve actual coords
            // X is shifted since there is one less bit stores in delta for X axis
            X = (X + (delta.Position.X << 1) + tX) % 256;
            Y = (Y + delta.Position.Y + tY) % 256;
            Z = (Z + delta.Position.Z + tZ) % 256;

            // save vertex position
            // notice Y and Z are swapped
            return new Vector3b((byte)X, (byte)Z, (byte)Y);
        }

        private int GetTemporalValue(BitStreamReader bs, int deltaBits)
        {
            int result = bs.TakeBit() == 1 ? -(1 << deltaBits) : 0;

            // TODO: maybe this can be simplified...
            for (int i = 0; i < deltaBits; i++)
                result |= bs.TakeBit() << (deltaBits - 1 - i);

            return result;
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            int pos = (int)bw.Position;

            //posOffset.Write(bw);
            bw.WriteVector3s(Offset, OffsetScale, VectorPadding.Yes);
            bw.Seek(16);

            ptrVerts = (int)(bw.Position - pos + 4);

            bw.Write(ptrVerts);

            // just save raw vertices, we don't support compression yet
            foreach (var vertex in Vertices)
                vertex.Write(bw);
        }
    }
}