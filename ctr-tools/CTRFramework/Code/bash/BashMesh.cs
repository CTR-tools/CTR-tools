using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CTRFramework.Bash
{
    public class BashMesh
    {
        public short u11;
        public short u12;
        public int uzero;

        public int ptrVerts;
        public int ptrModel;
        public int ptrAfterVerts;
        public int ptrSmthElse;
        public int ptrSmthElse2;
        public int ptrLast;

        public List<Vector3> Vertices = new List<Vector3>();

        public BashMesh(BinaryReaderEx br) => Read(br);

        public static BashMesh FromReader(BinaryReaderEx br) => new BashMesh(br);

        public void Read(BinaryReaderEx br)
        {
            br.ReadBytes(8);

            u11 = br.ReadInt16();
            u12 = br.ReadInt16();
            uzero = br.ReadInt32();

            if (uzero != 0)
                Console.WriteLine("uzero not null");

            ptrVerts = (int)(br.Position + br.ReadInt32());
            ptrModel = (int)(br.Position + br.ReadInt32());
            ptrAfterVerts = (int)(br.Position + br.ReadInt32());

            int numVerts = (ptrAfterVerts - ptrVerts - 0x14) / 8;

            ptrSmthElse = (int)(br.Position + br.ReadInt32());
            ptrSmthElse2 = (int)(br.Position + br.ReadInt32());
            ptrLast = (int)(br.Position + br.ReadInt32());

            br.ReadBytes(12);

            int pos = (int)br.Position;

            br.Jump(ptrVerts + 0x14);

            for (int i = 0; i < numVerts; i++)
                Vertices.Add(br.ReadVector3sPadded());

            br.Jump(pos);
        }

        public string ToObj()
        {
            var sb = new StringBuilder();

            foreach (var v in Vertices)
                sb.AppendLine($"v {v.X} {-v.Y} {-v.Z}");

            //assume it's all quads (it's not)
            //placeholder until tristrips figured out just to show some broken mesh
            for (int i = 0; i < Vertices.Count / 4; i++)
            {
                sb.AppendLine($"f {4 * i + 1} {4 * i + 2} {4 * i + 3}");
                sb.AppendLine($"f {4 * i + 3} {4 * i + 2} {4 * i + 4}");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return
                ptrModel.ToString("X8") + " " +
                ptrVerts.ToString("X8") + " " +
                ptrAfterVerts.ToString("X8") + " " +
                ptrSmthElse.ToString("X8") + " " +
                ptrSmthElse2.ToString("X8") + " " +
                ptrLast.ToString("X8") + "numVerts=" + (ptrAfterVerts - ptrVerts - 0x14) / 8;
        }
    }
}
