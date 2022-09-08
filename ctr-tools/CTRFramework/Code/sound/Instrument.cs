using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework.Sound
{
    /// <summary>
    /// Describes CSEQ instrument.
    /// </summary>
    public class Instrument : IReadWrite
    {
        public static readonly int SizeOf = 0x0C;

        public byte _magic1;
        public byte _volume;
        public ushort _freq;
        public ushort SampleID { get; set; }
        public short _always0;
        public uint ADSR { get; set; }  // assumes to be raw psx adsr value passed directly to psyq

        public MetaInst metaInst { get; set; }

        public string Tag => $"{ID}_{Frequency}";
        public string ID => $"{SampleID.ToString("0000")}_{SampleID.ToString("X4")}";

        public int Frequency
        {
            get { return (int)Math.Round(_freq / 4096f * 44100f); }
            set { _freq = (ushort)Math.Round(value / 44100f * 4096f); }
        }

        public float Volume
        {
            get { return _volume / 255f; }
            set
            {
                if (value < 0)
                {
                    _volume = 0;
                    return;
                }

                if (value > 1)
                {
                    _volume = 255;
                    return;
                }

                _volume = (byte)Math.Round(value * 255f);
            }
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
            ADSR = br.ReadUInt32();
        }

        public virtual void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((byte)1);
            bw.Write((byte)_volume);
            bw.Write((short)0);
            bw.Write((short)_freq);
            bw.Write((short)SampleID);
            bw.Write(ADSR);
        }
    }

    /// <summary>
    /// Shorter version of CSEQ instrument used for percussion.
    /// </summary>
    public class InstrumentShort : Instrument
    {
        public static readonly new int SizeOf = 8;

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
            bw.Write((byte)_magic1);
            bw.Write((byte)_volume);
            bw.Write((short)_freq);
            bw.Write((ushort)SampleID);
            bw.Write((short)_always0);
        }

        public override string ToString() => $"magic:{_magic1}\tvol:{_volume}\tfreq:{_freq}\tid:{SampleID}\tzero:{_always0}";
    }
}