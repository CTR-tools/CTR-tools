using CTRFramework.Shared;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CTRFramework.Bash
{
    //Bash mesh header struct
    public class BashMesh
    {
        public static readonly int SizeOf = 0x34;

        //0x0 - null?
        public int zerostart0;
        //0x4 - null?
        public int zerostart1;
        //0x08
        public short u11;
        //0x0a
        public short u12;
        //0x0c
        public int anotherZero;
        //0x10
        public int ptrVerts;
        //0x14
        public int ptrTristripInfo;
        //0x18
        public int ptrAfterVerts;
        //0x1C
        public int ptrSmthElse;
        //0x20
        public int ptrSmthElse2;
        //0x24
        public int ptrLast;
        //0x28
        public int zeroend0;
        //0x2C
        public int zeroend1;
        //0x30
        public int zeroend2;

        public List<Vector3> Vertices = new List<Vector3>();

        public List<ushort> tristripInfo = new List<ushort>();

        public BashMesh(BinaryReaderEx br) => Read(br);

        public static BashMesh FromReader(BinaryReaderEx br) => new BashMesh(br);

        public void Read(BinaryReaderEx br)
        {
            //bash pointers are relative to current position instead of the file origin

            zerostart0 = br.ReadInt32();
            zerostart1 = br.ReadInt32();
            u11 = br.ReadInt16();
            u12 = br.ReadInt16();
            anotherZero = br.ReadInt32();

            ptrVerts = (int)(br.Position + br.ReadInt32());
            ptrTristripInfo = (int)(br.Position + br.ReadInt32());
            ptrAfterVerts = (int)(br.Position + br.ReadInt32());
            ptrSmthElse = (int)(br.Position + br.ReadInt32());
            ptrSmthElse2 = (int)(br.Position + br.ReadInt32());
            ptrLast = (int)(br.Position + br.ReadInt32());

            zeroend0 = br.ReadInt32();
            zeroend1 = br.ReadInt32();
            zeroend2 = br.ReadInt32();


            Helpers.PanicIf(zerostart0 != 0, this, PanicType.Assume, $"zerostart0 = {zerostart0}");
            Helpers.PanicIf(zerostart1 != 0, this, PanicType.Assume, $"zerostart1 = {zerostart1}");
            Helpers.PanicIf(anotherZero != 0, this, PanicType.Assume, $"anotherZero = {zerostart0}");
            Helpers.PanicIf(zeroend0 != 0, this, PanicType.Assume, $"zeroend0 = {zeroend0}");
            Helpers.PanicIf(zeroend1 != 0, this, PanicType.Assume, $"zeroend1 = {zeroend1}");
            Helpers.PanicIf(zeroend2 != 0, this, PanicType.Assume, $"zeroend2 = {zeroend2}");


            int pos = (int)br.Position;

            //read tristrip structs

            br.Jump(ptrTristripInfo);

            ushort index = 0;

            do
            {
                index = br.ReadUInt16();

                if (index != 0xFFFF)
                    tristripInfo.Add(index);
            }
            while (index != 0xFFFF);

            //read vertices

            //is there a better way to get number of vertices?
            int numVerts = (ptrAfterVerts - ptrVerts - 0x14) / 8;

            br.Jump(ptrVerts + 0x14);

            for (int i = 0; i < numVerts; i++)
                Vertices.Add(br.ReadVector3sPadded(Helpers.GteScaleSmall)); //like in CTR x / 256 looks like proper scale

            //jump back to the end of the header, so we dont cofuse the parser
            br.Jump(pos);
        }

        /// <summary>
        /// Extracts Bash model header to OBJ file.
        /// </summary>
        /// <returns></returns>
        public string ToObj()
        {
            var sb = new StringBuilder();

            int curVertIndex = 0;
            int numFaces = 0;

            foreach (var strip in tristripInfo)
            {
                //tristrip info is 2 bytes, flags + number of faces
                ushort flags = (ushort)(strip & 0xFF);
                ushort count = (ushort)(strip >> 8);

                //reached end of the list
                if (strip == 0xFFFF) return sb.ToString();

                for (int i = 0; i < count; i++)
                {
                    //take 3 vertices at the cursor

                    var a = Vertices[curVertIndex];
                    var b = Vertices[curVertIndex + 1];
                    var c = Vertices[curVertIndex + 2];

                    //flip Y and Z, so we have correct OBJ orientation
                    sb.AppendLine($"v {a.X} {-a.Y} {-a.Z}");
                    sb.AppendLine($"v {b.X} {-b.Y} {-b.Z}");
                    sb.AppendLine($"v {c.X} {-c.Y} {-c.Z}");

                    //just throw in junk UV for meshlab's broken parser
                    sb.AppendLine($"vt 0 0");
                    sb.AppendLine($"vt 1 0");
                    sb.AppendLine($"vt 1 1");

                    //flip normals each face, but check flag for the initial state
                    if (i % 2 == ((flags & 0x08) > 0 ? 1 : 0))
                    {
                        sb.AppendLine($"f {3 * numFaces + 1}/{3 * numFaces + 1} {3 * numFaces + 3}/{3 * numFaces + 3} {3 * numFaces + 2}/{3 * numFaces + 2}");
                    }
                    else
                    {
                        sb.AppendLine($"f {3 * numFaces + 1}/{3 * numFaces + 1} {3 * numFaces + 2}/{3 * numFaces + 2} {3 * numFaces + 3}/{3 * numFaces + 3}");
                    }

                    sb.AppendLine();

                    //we added a face
                    numFaces++;

                    //move cursor
                    curVertIndex++;
                }

                //at the end of the strip, skip the last 2 vertices used.
                curVertIndex += 2;
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return
                ptrTristripInfo.ToString("X8") + " " +
                ptrVerts.ToString("X8") + " " +
                ptrAfterVerts.ToString("X8") + " " +
                ptrSmthElse.ToString("X8") + " " +
                ptrSmthElse2.ToString("X8") + " " +
                ptrLast.ToString("X8") + "numVerts=" + (ptrAfterVerts - ptrVerts - 0x14) / 8;
        }
    }
}
