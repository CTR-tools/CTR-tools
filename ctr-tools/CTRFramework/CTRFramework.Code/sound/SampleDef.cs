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
            get { return title; }
            set { title = value; }
        }

        [CategoryAttribute("Meta info"), DescriptionAttribute("MIDI instrument number.")]
        public byte MIDI
        {
            get { return midi; }
            set { midi = value; }
        }

        [CategoryAttribute("Meta info"), DescriptionAttribute("Pitch shift.")]
        public string PitchShift
        {
            get { return pitchshift; }
            set { pitchshift = value; }
        }


        private string title;
        private byte midi;
        private string pitchshift;

        public string Tag => SampleID.ToString("X4") + "_" + frequency;


        [CategoryAttribute("General"), DescriptionAttribute("Sample volume.")]
        public byte Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        [CategoryAttribute("General"), DescriptionAttribute("Sample pitch.")]
        public ushort Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }

        [CategoryAttribute("General"), DescriptionAttribute("Sample ID.")]
        public ushort SampleID
        {
            get { return sampleID; }
            set { sampleID = value; }
        }


        private byte magic1;
        private byte volume;
        private ushort pitch;  //4096 is considered to be 44100
        private ushort sampleID;
        private short always0;

        public int frequency
        {
            //cents needed?
            get { return (int)Math.Round(pitch * 44100.0f / 4096.0f); }
        }

        public SampleDef()
        {
        }

        public SampleDef(BinaryReaderEx br)
        {
            Read(br);
        }

        public virtual void Read(BinaryReaderEx br)
        {
            magic1 = br.ReadByte();
            volume = br.ReadByte();
            pitch = br.ReadUInt16();
            sampleID = br.ReadUInt16();
            always0 = br.ReadInt16();

            if (magic1 != 1)
                throw new Exception(String.Format("SampleDef magic1 = {0}", magic1));

            if (always0 != 0)
                throw new Exception(String.Format("SampleDef always0 = {0} ", always0));
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
