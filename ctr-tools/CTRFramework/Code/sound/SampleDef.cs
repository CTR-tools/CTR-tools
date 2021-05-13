using CTRFramework.Shared;
using System;
using System.ComponentModel;

namespace CTRFramework.Sound
{
    public class SampleDef : IReadWrite
    {
        [CategoryAttribute("Meta info"), DescriptionAttribute("Instrument title.")]
        public string Title
        {
            get => title;
            set => title = value;
        }

        [CategoryAttribute("Meta info"), DescriptionAttribute("MIDI instrument number.")]
        public byte MIDI
        {
            get => midi;
            set => midi = value;
        }

        [CategoryAttribute("Meta info"), DescriptionAttribute("Pitch shift.")]
        public string PitchShift
        {
            get => pitchshift;
            set => pitchshift = value;
        }


        private string title;
        private byte midi;
        private string pitchshift;

        public string Tag => SampleID.ToString("X4") + "_" + Frequency;


        [CategoryAttribute("General"), DescriptionAttribute("Sample volume.")]
        public byte Volume
        {
            get => volume;
            set => volume = value;
        }

        [CategoryAttribute("General"), DescriptionAttribute("Sample pitch.")]
        public ushort Pitch
        {
            get => pitch;
            set => pitch = value;
        }

        [CategoryAttribute("General"), DescriptionAttribute("Sample ID.")]
        public ushort SampleID
        {
            get => sampleID;
            set => sampleID = value;
        }


        private byte magic1;
        private byte volume;
        private ushort pitch;  //4096 is considered to be 44100
        private ushort sampleID;
        private short always0;

        public int Frequency
        {
            //cents needed?
            get => (int)Math.Round(pitch / 4096.0f * 44100.0f);
        }

        public SampleDef()
        {
        }

        public SampleDef(BinaryReaderEx br)
        {
            Read(br);
        }

        public static SampleDef FromReader(BinaryReaderEx br)
        {
            return new SampleDef(br);
        }

        public virtual void Read(BinaryReaderEx br)
        {
            magic1 = br.ReadByte();
            volume = br.ReadByte();
            pitch = br.ReadUInt16();
            sampleID = br.ReadUInt16();
            always0 = br.ReadInt16();

            //sanity checks
            if (magic1 != 1)
                Helpers.Panic(this, PanicType.Assume, $"magic1 != 1: {magic1}");

            if (always0 != 0)
                Helpers.Panic(this, PanicType.Assume, $"always0 != 0: {always0}");
        }

        public virtual void Write(BinaryWriterEx bw)
        {
            bw.Write((byte)magic1);
            bw.Write((byte)volume);
            bw.Write((ushort)pitch);
            bw.Write((ushort)sampleID);
            bw.Write((short)always0);
        }
    }
}
