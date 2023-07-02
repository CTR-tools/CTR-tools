﻿using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;

namespace CTRFramework.Sound
{
    /// <summary>
    /// Describes CSEQ instrument.
    /// </summary>
    public class Instrument : IReadWrite
    {
        public static readonly int SizeOf = 0x0C;

        public byte flags;  //0 - regular, 1 - music sample, 2 - looped sample? but insts can be looped as well, 3 - ?, 4 - taunt?, others?
        public byte _volume;
        public ushort _freq;
        public ushort SampleID { get; set; }

        //must be populated based on SampleID
        public Sample Sample;

        public short _always0; //not 0 in main sample table, probably length, size or duration
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
                if (value <= 0)
                {
                    _volume = 0;
                    return;
                }

                if (value >= 1)
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

        public Instrument(BinaryReaderEx br) => Read(br);

        public static Instrument FromReader(BinaryReaderEx br) => new Instrument(br);

        public virtual void Read(BinaryReaderEx br)
        {
            flags = br.ReadByte();
            _volume = br.ReadByte();
            _always0 = br.ReadInt16();
            _freq = br.ReadUInt16();
            SampleID = br.ReadUInt16();
            ADSR = br.ReadUInt32();

            if (flags != 1)
                Helpers.Panic(this, PanicType.Assume, $"magic1 != 1: {flags}");

            if (_always0 != 0)
                Helpers.Panic(this, PanicType.Assume, $"always0 != 0: {_always0}");
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
        public VagSample GetVagSample(HowlContext context)
        {
            if (Sample is null)
            {
                Helpers.Panic(this, PanicType.Warning, $"Sample data not found! {this.ID}");
                //Console.ReadKey();
                return null;
            }

            using (var br = new BinaryReaderEx(new MemoryStream(Sample.Data)))
            {
                var vag = new VagSample();
                vag.sampleFreq = Frequency;
                vag.ReadFrames(br, Sample.Data.Length);
                vag.HashString = Sample.HashString;

                if (context.HashNames.ContainsKey(vag.HashString))
                    vag.SampleName = context.HashNames[vag.HashString];

                Console.WriteLine($"{vag.HashString}: {vag.SampleName}");

                return vag;
            }
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

        public new static InstrumentShort FromReader(BinaryReaderEx br) => new InstrumentShort(br);

        public override void Read(BinaryReaderEx br)
        {
            flags = br.ReadByte();
            _volume = br.ReadByte();
            _freq = br.ReadUInt16();
            SampleID = br.ReadUInt16();
            _always0 = br.ReadInt16();

            if (flags != 0 && flags != 1 && flags != 2 && flags != 4)
            {
                Helpers.Panic(this, PanicType.Info, $"instrument flag: {flags}");
                Console.ReadKey();
            }
        }

        public override void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((byte)flags);
            bw.Write((byte)_volume);
            bw.Write((short)_freq);
            bw.Write((ushort)SampleID);
            bw.Write((short)_always0);
        }

        public override string ToString() => $"magic:{flags}\tvol:{_volume}\tfreq:{_freq}\tid:{SampleID}\tzero:{_always0}";
    }
}