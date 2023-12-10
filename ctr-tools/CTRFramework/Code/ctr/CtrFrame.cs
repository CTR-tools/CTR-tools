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

            Helpers.Panic(this, PanicType.Assume, $"{br.HexPos()} vertOffset: {ptrVerts.ToString("X8")}");

            //Console.ReadKey();

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

                //just read a chunk big enough to hold any anim. there will be useless junk in the end, but we won't use it anyways.
                var temporal = br.ReadBytes(1024 * 4);


                using (var bs = BitStreamReader.FromByteArray(temporal))
                {
                    foreach (var delta in deltas)
                    {
                        //Helpers.Panic(this, PanicType.Info, delta.ToString());
                        //Console.ReadKey();
                        //Helpers.Panic(this, PanicType.Info, $"temporal: {delta.value.ToString("X8")}");
                        //Helpers.Panic(this, PanicType.Info, $"pos: {delta.Position.X} {delta.Position.Y} {delta.Position.Z}");
                        //Helpers.Panic(this, PanicType.Info, $"bits: {delta.Bits.X} {delta.Bits.Y} {delta.Bits.Z}");

                        //111 reset
                        if (delta.Bits.X == 7) X = 0;
                        if (delta.Bits.Y == 7) Y = 0;
                        if (delta.Bits.Z == 7) Z = 0;


                        int tX = bs.ReadBits(1) == 1 ? -(1 << delta.Bits.X) : 0;

                        for (int i = 0; i < delta.Bits.X; i++)
                        {
                            byte bit = Convert.ToByte(bs.ReadBits(1));
                            tX |= bit << (delta.Bits.X - 1 - i);
                        }


                        int tY = bs.ReadBits(1) == 1 ? - (1 << delta.Bits.Y) : 0;

                        for (int i = 0; i < delta.Bits.Y; i++)
                        {
                            byte bit = Convert.ToByte(bs.ReadBits(1));
                            tY |= bit << (delta.Bits.Y - 1 - i);
                        }
                        

                        int tZ = bs.ReadBits(1) == 1 ? -(1 << delta.Bits.Z) : 0;

                        for (int i = 0; i < delta.Bits.Z; i++)
                        {
                            byte bit = Convert.ToByte(bs.ReadBits(1));
                            tZ |= bit << (delta.Bits.Z - 1 - i);
                        }

                        //Helpers.Panic(this, PanicType.Info, $"result: {tX} {tY} {tZ}");

                        //add temporal delta
                        X = (X + (delta.Position.X << 1) + tX) % 256;
                        Y = (Y + delta.Position.Y + tY) % 256;
                        Z = (Z + delta.Position.Z + tZ) % 256;

                        //Helpers.Panic(this, PanicType.Info, $"result: {X} {Y} {Z}");

                        //swap z and y
                        Vertices.Add(new Vector3b((byte)X, (byte)Z, (byte)Y));
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