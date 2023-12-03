using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Windows.Forms.VisualStyles;

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

            //skipping 16 zero bytes
            //this is used at runtime
            br.Seek(16);

            ptrVerts = br.ReadInt32();

            Helpers.Panic(this, PanicType.Assume, $"vertOffset: {ptrVerts.ToString("X8")}");

            br.Seek(ptrVerts - 0x1C);

            if (deltas == null) //no deltas provided, assume raw vertices
            { 
                for (int i = 0; i < numVerts; i++)
                    Vertices.Add(new Vector3b(br));
            }
            else //must be animated frame, decompress first
            {
                int X = 0;
                int Y = 0;
                int Z = 0;

                int[] SignTable = { -1, -2, -4, -8, -16, -32, -64, -128 }; // used for decompression

                //just read a chunk big enough to hold any anim. there will be useless junk in the end, but we won't use it anyways.
                var temporal = br.ReadBytes(1024 * 1024);

                using (var bs = new BitStreamReader(new MemoryStream(temporal)))
                {
                    foreach (var delta in deltas)
                    {
                        //Helpers.Panic(this, PanicType.Info, delta.ToString());
                        //Console.ReadKey();

                        //111 reset
                        if (delta.Bits.X == 7) X = 0;
                        if (delta.Bits.Y == 7) Y = 0;
                        if (delta.Bits.Z == 7) Z = 0;


                        int tX = bs.ReadBits(1) == 1 ? SignTable[delta.Bits.X] : 0;

                        for (int i = 0; i < delta.Bits.X; ++i)
                        {
                            tX |= Convert.ToByte(bs.ReadBits(1)) << (delta.Bits.X - 1 - i);
                        }

                        int tY = bs.ReadBits(1) == 1 ? SignTable[delta.Bits.Y] : 0;

                        for (int i = 0; i < delta.Bits.Y; ++i)
                        {
                            tY |= Convert.ToByte(bs.ReadBits(1)) << (delta.Bits.Y - 1 - i);
                        }

                        int tZ = bs.ReadBits(1) == 1 ? SignTable[delta.Bits.Z] : 0;

                        for (int i = 0; i < delta.Bits.Z; ++i)
                        {
                            tZ |= Convert.ToByte(bs.ReadBits(1)) << (delta.Bits.Z - 1 - i);
                        }


                        //add temporal delta
                        X = (X +(delta.Position.X << 1) + tX) % 256;
                        Y = (Y + delta.Position.Y + tY) % 256;
                        Z = (Z + delta.Position.Z + tZ) % 256;

                        Vertices.Add(new Vector3b((byte)X, (byte)Y, (byte)Z));
                    }
                }
            }
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //posOffset.Write(bw);
            bw.WriteVector3sPadded(Offset, OffsetScale);
            bw.Write(new byte[16]);
            bw.Write(ptrVerts);

            foreach (var vertex in Vertices)
                vertex.Write(bw);
        }
    }
}