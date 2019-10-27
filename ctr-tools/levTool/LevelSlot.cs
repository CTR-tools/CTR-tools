using CTRFramework.Shared;

namespace levTool
{
    class LevelSlot
    {
        public short unk1;
        public short unk1z;
        public uint ptrunk1;
        public int title_index;
        public int unk2;
        public short unk3;
        public short unk4;
        public short unk5;
        public short unk6;

        public LevelSlot(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            unk1 = br.ReadInt16();
            unk1z = br.ReadInt16();
            ptrunk1 = br.ReadUInt32();
            title_index = br.ReadInt32();
            unk2 = br.ReadInt32();
            unk3 = br.ReadInt16();
            unk4 = br.ReadInt16();
            unk5 = br.ReadInt16();
            unk6 = br.ReadInt16();
        }

        public override string ToString()
        {
            return unk1 + "\t" + unk1z + "\t" + ptrunk1.ToString("X8") + "\t" + title_index + "\t" + unk2 + "\t" + unk3 + "\t" + unk4 + "\t" + unk5 + "\t" + unk6 + "\t";
        }
    }
}
