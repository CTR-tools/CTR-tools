using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CTRFramework.Sound
{
    /// <summary>
    /// Describes CSEQ instrument.
    /// </summary>
    public class Instrument : IReadWrite
    {
        protected byte _magic1;
        protected byte _volume;
        protected ushort _freq;
        public ushort SampleID { get; set; }
        protected short _always0;

        public short unknownFF80;
        public byte reverb;
        public byte reverb2;

        public MetaInst metaInst { get; set; }

        public string Tag => SampleID.ToString("X4") + "_" + Frequency;
        public int Frequency
        {
            get { return (int)Math.Round(_freq / 4096f * 44100f); }
            set { _freq = (ushort)Math.Round(value / 44100f * 4096f); }
        }

        public float Volume
        {
            get { return _volume / 255f; }
            set {
                if (_volume < 0)
                    _volume = 0;

                if (_volume > 1)
                    _volume = 1;

                _volume = (byte)(value * 255f); }
        }

        public Instrument()
        {
        }

        public Instrument(BinaryReaderEx br)
        {
            Read(br);

            if (_magic1 != 1)
                Helpers.Panic(this, PanicType.Assume, $"magic1 != 1: {_magic1}");

            if (_always0 != 0)
                Helpers.Panic(this, PanicType.Assume, $"always0 != 0: {_always0}");
        }

        public static Instrument FromReader(BinaryReaderEx br)
        {
            return new Instrument(br);
        }

        public virtual void Read(BinaryReaderEx br)
        {
            _magic1 = br.ReadByte();
            _volume = br.ReadByte();
            _always0 = br.ReadInt16();
            _freq = br.ReadUInt16();
            SampleID = br.ReadUInt16();
            unknownFF80 = br.ReadInt16();
            reverb = br.ReadByte();
            reverb2 = br.ReadByte();
        }

        public virtual void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((byte)1);
            bw.Write((byte)_volume);
            bw.Write((short)0);
            bw.Write((short)_freq);
            bw.Write((short)SampleID);
            bw.Write((short)unknownFF80);
            bw.Write((byte)reverb);
            bw.Write((byte)reverb2);
        }
    }

    /// <summary>
    /// Shorter version of CSEQ instrument used for percussion.
    /// </summary>
    public class InstrumentShort : Instrument
    {
        public InstrumentShort()
        {
        }

        public InstrumentShort(BinaryReaderEx br) : base(br) 
        {
        }

        public new static InstrumentShort FromReader(BinaryReaderEx br)
        {
            return new InstrumentShort(br);
        }

        public override void Read(BinaryReaderEx br)
        {
            _magic1 = br.ReadByte();
            _volume = br.ReadByte();
            _freq = br.ReadUInt16();
            SampleID = br.ReadUInt16();
            _always0 = br.ReadInt16();
        }

        public override void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((byte)1);
            bw.Write((byte)_volume);
            bw.Write((short)_freq);
            bw.Write((ushort)SampleID);
            bw.Write((short)0);
        }
    }
}