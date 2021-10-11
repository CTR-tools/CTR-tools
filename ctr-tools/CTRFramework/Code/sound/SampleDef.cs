using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CTRFramework.Sound
{
    public class SampleDef : IReadWrite
    {
        private byte magic1;
        public byte Velocity;
        public int Frequency;  //4096 is considered to be 44100
        public ushort sampleID;
        private short always0;

        private string title;
        private byte midi;
        private int pitchshift;

        #region [Component model]

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
        public int PitchShift
        {
            get => pitchshift;
            set => pitchshift = value;
        }

        public string Tag => SampleID.ToString("X4") + "_" + Frequency;

        [CategoryAttribute("General"), DescriptionAttribute("Sample volume.")]
        public byte Volume
        {
            get => Velocity;
            set => Velocity = value;
        }

        [CategoryAttribute("General"), DescriptionAttribute("Sample pitch.")]
        public int Pitch
        {
            get => Frequency;
            set => Frequency = value;
        }

        [CategoryAttribute("General"), DescriptionAttribute("Sample ID.")]
        public ushort SampleID
        {
            get => sampleID;
            set => sampleID = value;
        }

        #endregion

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
            Velocity = br.ReadByte();
            Frequency = (int)Math.Round(br.ReadUInt16() / 4096.0f * 44100.0f);
            sampleID = br.ReadUInt16();
            always0 = br.ReadInt16();

            //sanity checks
            if (magic1 != 1)
                Helpers.Panic(this, PanicType.Assume, $"magic1 != 1: {magic1}");

            if (always0 != 0)
                Helpers.Panic(this, PanicType.Assume, $"always0 != 0: {always0}");
        }

        public virtual void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((byte)magic1);
            bw.Write((byte)Velocity);
            bw.Write((short)Math.Round(Frequency * 4096.0f / 44100.0f));
            bw.Write((ushort)sampleID);
            bw.Write((short)always0);
        }
    }
}