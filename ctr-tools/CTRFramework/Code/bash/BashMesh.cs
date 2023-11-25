using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CTRFramework.Bash
{
    public class BashMesh
    {
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
        public int ptrTriLinks;
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

        public List<ushort> triLinks = new List<ushort>();

        public BashMesh(BinaryReaderEx br) => Read(br);

        public static BashMesh FromReader(BinaryReaderEx br) => new BashMesh(br);

        public void Read(BinaryReaderEx br)
        {
            zerostart0 = br.ReadInt32();
            zerostart1 = br.ReadInt32();

            u11 = br.ReadInt16();
            u12 = br.ReadInt16();
            anotherZero = br.ReadInt32();

            if (anotherZero != 0)
            {
                Console.WriteLine("uzero not null!");
                Console.ReadKey();
            }

            ptrVerts = (int)(br.Position + br.ReadInt32());
            ptrTriLinks = (int)(br.Position + br.ReadInt32());
            ptrAfterVerts = (int)(br.Position + br.ReadInt32());

            int numVerts = (ptrAfterVerts - ptrVerts - 0x14) / 8;


            ptrSmthElse = (int)(br.Position + br.ReadInt32());
            ptrSmthElse2 = (int)(br.Position + br.ReadInt32());
            ptrLast = (int)(br.Position + br.ReadInt32());

            zeroend0 = br.ReadInt32();
            zeroend1 = br.ReadInt32();
            zeroend2 = br.ReadInt32();

            int pos = (int)br.Position;


            br.Jump(ptrTriLinks);

            ushort index = 0;

            do
            {
                index = br.ReadUInt16();

                if (index != 0xFFFF)
                    triLinks.Add(index);
            }
            while (index != 0xFFFF);



            br.Jump(ptrVerts + 0x14);

            for (int i = 0; i < numVerts; i++)
                Vertices.Add(br.ReadVector3sPadded(0.01f));

            br.Jump(pos);
        }

        int curVertIndex = 0;

        public Vector3 GetNextVertex()
        {
            curVertIndex++;
            return Vertices[curVertIndex - 1];
        }


        List<Vector3> window = new List<Vector3>();

        public string ToObj()
        {
            var sb = new StringBuilder();

            curVertIndex = 0;

            int numFaces = 0;

            foreach (var link in triLinks)
            {
                ushort flags = (ushort)(link & 0xFF);
                ushort count = (ushort)(link >> 8);

                //reached end of the list
                if (link == 0xFFFF) return sb.ToString();

                for (int i = 0; i < count; i++)
                {
                    var a = Vertices[curVertIndex];
                    var b = Vertices[curVertIndex + 1];
                    var c = Vertices[curVertIndex + 2];

                    sb.AppendLine($"v {a.X} {-a.Y} {-a.Z}");
                    sb.AppendLine($"v {b.X} {-b.Y} {-b.Z}");
                    sb.AppendLine($"v {c.X} {-c.Y} {-c.Z}");

                    //just throw in junk UV for meshlab's broken parser
                    sb.AppendLine($"vt 0 0");
                    sb.AppendLine($"vt 1 0");
                    sb.AppendLine($"vt 1 1");

                    //flip normals each face, but check flag for the initial state
                    if ( i % 2 == ( (flags & 0x08) > 0 ? 1 : 0) )
                    {
                        sb.AppendLine($"f {3 * numFaces + 1}/{3 * numFaces + 1} {3 * numFaces + 3}/{3 * numFaces + 3} {3 * numFaces + 2}/{3 * numFaces + 2}");
                    }
                    else
                    {
                        sb.AppendLine($"f {3 * numFaces + 1}/{3 * numFaces + 1} {3 * numFaces + 2}/{3 * numFaces + 2} {3 * numFaces + 3}/{3 * numFaces + 3}");
                    }

                    sb.AppendLine();

                    numFaces++;
                    curVertIndex++;
                }

                curVertIndex += 2;
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return
                ptrTriLinks.ToString("X8") + " " +
                ptrVerts.ToString("X8") + " " +
                ptrAfterVerts.ToString("X8") + " " +
                ptrSmthElse.ToString("X8") + " " +
                ptrSmthElse2.ToString("X8") + " " +
                ptrLast.ToString("X8") + "numVerts=" + (ptrAfterVerts - ptrVerts - 0x14) / 8;
        }
    }
}
