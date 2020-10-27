using CTRFramework.Shared;
using System;
using System.IO;
using System.Text;

namespace CTRFramework
{
    public class VisData : IReadWrite
    {
        public ushort flag;
        public ushort id;
        public BoundingBox bbox;
        public Vector3s v3;
        public Vector3s v4;
        public uint ptrQuadBlock;

        public VisData()
        {
        }

        public VisData(BinaryReaderEx br)
        {
            Read(br);
        }

        public static VisData FromReader(BinaryReaderEx br)
        {
            return new VisData(br);
        }

        public void Read(BinaryReaderEx br)
        {
            flag = br.ReadUInt16();
            id = br.ReadUInt16();
            bbox = new BoundingBox(br);
            v3 = new Vector3s(br);
            v4 = new Vector3s(br);
            ptrQuadBlock = br.ReadUInt32();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(flag);
            bbox.Write(bw);
            v3.Write(bw);
            v4.Write(bw);
            bw.Write(ptrQuadBlock);
        }


        public string ToObj()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"o vis_{id}");
            sb.AppendLine($"v {bbox.Min.ToString(VecFormat.Numbers)}");
            sb.AppendLine($"v {bbox.Max.ToString(VecFormat.Numbers)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return  $"id = {id} | flag = {flag} | ptr = {ptrQuadBlock.ToString("X8")}\r\n\t{bbox.ToString()}\r\n\t{v3.ToString(VecFormat.Braced)} {v4.ToString(VecFormat.Braced)}";
        }
    }
}
