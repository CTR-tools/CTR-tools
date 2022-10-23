using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    [Flags]
    public enum VisDataFlags
    {
        None = 0,
        Leaf = 1 << 0,      // defines if entry is leaf or branch, other bits assumed to only be used 1 at a time (? check)
        Water = 1 << 1,     // renders quads node as water
        Unk2 = 1 << 2,      // ?? caps on cove bridge
        Subdiv4x1 = 1 << 3, // reduces 4x4 subdivision to 4x1
        Subdiv4x2 = 1 << 4, // reduces 4x4 subdivision to 4x2
        Unk5 = 1 << 5,      // additive blended?
        Hidden = 1 << 6,    // doesn't render child quads
        Instance = 1 << 7,  // denotes whether the box contains model instance (only used in linked hitbox nodes)
        All = -1
    }

    public class VisData : IReadWrite
    {
        public static readonly int SizeOf = 0x20;
        public int BaseAddress;

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
        public uint unk1;           //assumed to be 0

        //if leaf
        public uint reserved;           //assumed to be 0
        public uint ptrInstanceNodes;   //pointer to the array of nodes with a zero byte terminator
        public uint numQuadBlock;
        public uint ptrQuadBlock;

        //if hitbox
        public short boxX;
        public short boxY;
        public short boxZ;

        public short unkX;
        public short unkY;
        public short unkZ;

        public int ptrInst;


        public bool IsLeaf => flag.HasFlag(VisDataFlags.Leaf);

        #region [Constructors, Factories]
        public VisData()
        {
        }

        public VisData(BinaryReaderEx br) => Read(br);

        public static VisData FromReader(BinaryReaderEx br) => new VisData(br);

        #endregion

        public static int[] counter = new int[8];

        byte[] rawdata;

        public void Read(BinaryReaderEx br)
        {
            BaseAddress = (int)br.Position;

            rawdata = br.ReadBytes(0x20);

            br.Seek(-0x20);


            flag = (VisDataFlags)br.ReadByte();
            unk0 = br.ReadByte();

            //flag is likely ushort, just testing if upper byte has any data
            if (unk0 != 0)
                Helpers.Panic(this, PanicType.Assume, $"unk0 is not null: {unk0.ToString("X2")}");

            if (!IsLeaf && flag != VisDataFlags.None)
                Helpers.Panic(this, PanicType.Assume, $"branches assumed to have no flags, yet: {((int)flag).ToString("X8")}");

            if (flag.HasFlag(VisDataFlags.Subdiv4x1) && flag.HasFlag(VisDataFlags.Subdiv4x2))
                Helpers.Panic(this, PanicType.Assume, $"nodes not supposed to use both subdiv flags at the same time???");

            if (flag.HasFlag(VisDataFlags.Leaf)) counter[0]++;
            if (flag.HasFlag(VisDataFlags.Water)) counter[1]++;
            if (flag.HasFlag(VisDataFlags.Unk2)) counter[2]++;
            if (flag.HasFlag(VisDataFlags.Subdiv4x1)) counter[3]++;
            if (flag.HasFlag(VisDataFlags.Subdiv4x2)) counter[4]++;
            if (flag.HasFlag(VisDataFlags.Unk5)) counter[5]++;
            if (flag.HasFlag(VisDataFlags.Hidden)) counter[6]++;
            if (flag.HasFlag(VisDataFlags.Instance)) counter[7]++;

            id = br.ReadUInt16();
            bbox = BoundingBox.FromReader(br);

            if (flag.HasFlag(VisDataFlags.Instance))
            {
                boxX = br.ReadInt16();
                boxY = br.ReadInt16();
                boxZ = br.ReadInt16();

                unkX = br.ReadInt16();
                unkY = br.ReadInt16();
                unkZ = br.ReadInt16();

                ptrInst = br.ReadInt32();
            }
            else if (!IsLeaf)
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
                    Helpers.Panic(this, PanicType.Assume, $"{flag} {IsLeaf} {BaseAddress.ToString("X8")} divX = {divX.ToString("X8")}");

                if (!(divY == 4096 || divY == 0))
                    Helpers.Panic(this, PanicType.Assume, $"{flag} {IsLeaf} {BaseAddress.ToString("X8")} divY = {divY.ToString("X8")}");

                if (!(divZ == 4096 || divZ == 0))
                    Helpers.Panic(this, PanicType.Assume, $"{flag} {IsLeaf} {BaseAddress.ToString("X8")} divZ = {divZ.ToString("X8")}");

                if (unk != 0)
                    Helpers.Panic(this, PanicType.Assume, $"{flag} {IsLeaf} {BaseAddress.ToString("X8")} unk = {unk.ToString("X8")}");

                if (unk1 != 0)
                    Helpers.Panic(this, PanicType.Assume, $"{flag} {IsLeaf} {BaseAddress.ToString("X8")} unk1 = {unk1.ToString("X8")}");
            }
            else
            {
                reserved = br.ReadUInt32();
                ptrInstanceNodes = br.ReadUInt32();
                numQuadBlock = br.ReadUInt32();
                ptrQuadBlock = br.ReadUInt32();

                if (reserved != 0)
                    Helpers.Panic(this, PanicType.Assume, "reserved is not 0: " + reserved.ToString("X8"));
            }

            if (br.Position - BaseAddress != SizeOf)
                Helpers.Panic(this, PanicType.Error, "visdata sizeof mismatch");
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            var pos = bw.Position;

            bw.Write((byte)flag);
            bw.Write(unk0);
            bw.Write(id);
            bbox.Write(bw);

            if (flag.HasFlag(VisDataFlags.Instance))
            {
                bw.Write(boxX);
                bw.Write(boxY);
                bw.Write(boxZ);
                bw.Write(unkX);
                bw.Write(unkY);
                bw.Write(unkZ);
                bw.Write(ptrInst);
            }
            else if (!IsLeaf)
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
                bw.Write(ptrInstanceNodes);
                bw.Write(numQuadBlock);
                bw.Write(ptrQuadBlock);
            }

            if (bw.Position - pos != SizeOf)
                Helpers.Panic(this, PanicType.Error, "visdata sizeof mismatch");
        }

        public override string ToString()
        {
            string result = $"id = {id} | flag = {flag} | ptr = {ptrQuadBlock.ToString("X8")}\r\n\t{bbox.ToString()}\r\n\t";

            foreach (byte b in rawdata)
                result += b.ToString("X2");

            result += "\r\n";

            return result;
        }
    }
}