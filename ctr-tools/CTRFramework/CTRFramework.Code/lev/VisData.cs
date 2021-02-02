using CTRFramework.Shared;
using System;
using System.Text;
using System.Collections.Generic;

namespace CTRFramework
{
    public class VisData : IReadWrite
    {
        public int pos;

        public VisDataFlags flag;
        public byte unk0;
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
        public uint reserved;     //assumed to be 0
        public uint ptrUnkData; //data goes right after visdata, relatively low amounts (20-30) or 0 links, seems to control trigger scripts somehow, can't shatter crates if 0
        public uint numQuadBlock;
        public uint ptrQuadBlock;

        public bool IsLeaf
        {
            get
            {
                return flag.HasFlag(VisDataFlags.Leaf);
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

        public static int[] counter = new int[8]; 

        public void Read(BinaryReaderEx br)
        {
            pos = (int)br.BaseStream.Position;

            flag = (VisDataFlags)br.ReadByte();
            unk0 = br.ReadByte();

            //flag is likely ushort, just testing if upper byte has any data
            if (unk0 != 0)
                throw new Exception("unk0 is not null");

            if (flag.HasFlag(VisDataFlags.Leaf)) counter[0]++;
            if (flag.HasFlag(VisDataFlags.Water)) counter[1]++;
            if (flag.HasFlag(VisDataFlags.Unk2)) counter[2]++;
            if (flag.HasFlag(VisDataFlags.Unk3)) counter[3]++;
            if (flag.HasFlag(VisDataFlags.Unk4)) counter[4]++;
            if (flag.HasFlag(VisDataFlags.Unk5)) counter[5]++;
            if (flag.HasFlag(VisDataFlags.Unk6)) counter[6]++;
            if (flag.HasFlag(VisDataFlags.Unk7)) counter[7]++;

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
                reserved = br.ReadUInt32();
                ptrUnkData = br.ReadUInt32();
                numQuadBlock = br.ReadUInt32();
                ptrQuadBlock = br.ReadUInt32();

                if (reserved != 0)
                    Helpers.Panic(this, "reserved is not 0: " + reserved.ToString("X8"));
            }


            //Console.WriteLine($"{flag.ToString("X4")}\t{numQuadBlock.ToString("0000")}\t{ptrQuadBlock.ToString("X8")}");
            //Console.ReadKey();
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write((byte)flag);
            bw.Write(unk0);
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
                bw.Write(reserved);
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

        public string ToObj(List<Vertex> v, Detail detail, ref int a, ref int b, List<QuadBlock> qb)
        {
            StringBuilder sb = new StringBuilder();

            if (IsLeaf)
            {
                sb.AppendFormat("g vis_{0}\r\n", id.ToString("X4"));
                sb.AppendFormat("o vis_{0}\r\n\r\n", id.ToString("X4"));

                foreach (QuadBlock q in qb)
                {
                    for (uint i = ptrQuadBlock; i <= ptrQuadBlock + numQuadBlock * 0x5C; i += 0x5C)
                        if (q.pos == i)
                        {
                            sb.Append(q.ToObj(v, detail, ref a, ref b));
                        }
                }
            }
            
            return sb.ToString();
        }


        public override string ToString()
        {
            return $"id = {id} | flag = {flag} | ptr = {ptrQuadBlock.ToString("X8")}\r\n\t{bbox.ToString()}\r\n\t";
        }
    }
}
