using CTRFramework.Shared;
using System;
using System.Text;

namespace CTRFramework
{
    public class VisData : IReadWrite
    {
        public int pos;

        //bit0 defines some changes in data.
        //other bits can only present 1 at a time.
        public ushort flag;
        public ushort id;
        public BoundingBox bbox;

        //if branch
        public ushort divX;
        public ushort divY;
        public ushort divZ;
        public ushort unk;
        public ushort leftChild;
        public ushort rightChild;
        public uint unk1;   //assumed to be 0

        //if leaf
        public uint u1;     //assumed to be 0
        public uint ptrUnkData; //data goes right after visdata, relatively low amounts (20-30) or 0 links
        public uint numQuadBlock;
        public uint ptrQuadBlock;

        bool IsLeaf
        {
            get
            {
                return (flag & 1) == 1;
            }
        }

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
            pos = (int)br.BaseStream.Position;

            flag = br.ReadUInt16();
            id = br.ReadUInt16();
            bbox = new BoundingBox(br);

            if (!IsLeaf)
            {
                divX = br.ReadUInt16();
                divY = br.ReadUInt16();
                divZ = br.ReadUInt16();
                unk = br.ReadUInt16();
                leftChild = br.ReadUInt16();
                rightChild = br.ReadUInt16();
                unk1 = br.ReadUInt32();

                //test leaf assumptions

                if (!(divX == 4096 || divX == 0))
                    throw new Exception($"{flag} {IsLeaf} {pos.ToString("X8")} divX = {divX.ToString("X8")}");

                if (!(divY == 4096 || divY == 0))
                    throw new Exception($"{flag} {IsLeaf} {pos.ToString("X8")} divY = {divY.ToString("X8")}");

                if (!(divZ == 4096 || divZ == 0))
                    throw new Exception($"{flag} {IsLeaf} {pos.ToString("X8")} divZ = {divZ.ToString("X8")}");

                if (unk1 != 0)
                    throw new Exception($"{flag} {IsLeaf} {pos.ToString("X8")} unk1 = {unk1.ToString("X8")}");
            }
            else
            {
                u1 = br.ReadUInt32();
                ptrUnkData = br.ReadUInt32();
                numQuadBlock = br.ReadUInt32();
                ptrQuadBlock = br.ReadUInt32();

                if (u1 != 0)
                    throw new Exception(u1.ToString("X8"));
            }


            //Console.WriteLine($"{flag.ToString("X4")}\t{numQuadBlock.ToString("0000")}\t{ptrQuadBlock.ToString("X8")}");
            //Console.ReadKey();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(flag);
            bw.Write(id);
            bbox.Write(bw);

            if (!IsLeaf)
            {
                bw.Write(divX);
                bw.Write(divY);
                bw.Write(divZ);
                bw.Write(unk);
                bw.Write(leftChild);
                bw.Write(rightChild);
                bw.Write(unk1);
            }
            else
            {
                bw.Write(u1);
                bw.Write(ptrUnkData);
                bw.Write(numQuadBlock);
                bw.Write(ptrQuadBlock);
            }
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
