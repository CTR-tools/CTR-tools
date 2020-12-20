using CTRFramework.Shared;
using System.Text;

namespace CTRFramework
{
    public class VisData : IReadWrite
    {
        //bit0 defines some changes in data.
        //other bits can only present 1 at a time.
        public ushort flag;
        public ushort id;
        public BoundingBox bbox;
        public byte[] skip;

        //these are set if bit0 is set
        public uint numQuadBlock;
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
            skip = br.ReadBytes(8);
            numQuadBlock = br.ReadUInt32();
            ptrQuadBlock = br.ReadUInt32();

            //Console.WriteLine($"{flag.ToString("X4")}\t{numQuadBlock.ToString("0000")}\t{ptrQuadBlock.ToString("X8")}");
            //Console.ReadKey();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(flag);
            bbox.Write(bw);
            bw.Write(skip);
            bw.Write(numQuadBlock);
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
            return $"id = {id} | flag = {flag} | ptr = {ptrQuadBlock.ToString("X8")}\r\n\t{bbox.ToString()}\r\n\t";
        }
    }
}
