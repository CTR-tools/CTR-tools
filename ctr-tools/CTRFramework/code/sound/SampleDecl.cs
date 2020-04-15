using CTRFramework.Shared;
using System;

namespace CTRFramework.Sound
{
    public class SampleDecl
    {
        public byte unk1;
        public byte volume;
        public ushort pitch;
        public ushort sampleID;
        public ushort unk2;

        public int frequency
        {
            //cents needed?
            get { return (int)Math.Round(pitch * 44100.0f / 4096.0f); }
        }

        public SampleDecl()
        {
        }

        public SampleDecl(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            unk1 = br.ReadByte();
            volume = br.ReadByte();
            pitch = br.ReadUInt16();
            sampleID = br.ReadUInt16();
            unk2 = br.ReadUInt16();
        }

        public override string ToString()
        {
            return String.Format(
                "unk1={0}, vol={1}, pitch={2}, id={3}, unk2={4}",
                unk1, volume, pitch, sampleID, unk2
                );
        }
    }

}
