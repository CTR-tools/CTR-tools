using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Text;
using System.Runtime.Remoting.Contexts;

namespace CTRFramework.Audio
{
    /// <summary>
    /// Describes CSEQ instrument.
    /// </summary>
    public class SpuInstrument : IReadWrite, IEquatable<SpuInstrument>
    {
        public SampleType? Type => Sample?.Type;

        public HowlContext Context;

        public string Name = "default_instrument";

        public Sample GetSample(int sampleIndex, HowlContext context) => context.SamplePool.ContainsKey(sampleIndex) ? context.SamplePool[sampleIndex] : null;

        public static readonly int SizeOf = 0x0C;

        [Description("probably used for sound category plus loop flag.\r\n0 - regular\r\n1 - music sample\r\n2 - looped sample\r\n4 - voice clip")]
        public byte flags { get; set; } = 0; //0 - regular, 1 - music sample, 2 - looped sample? but insts can be looped as well, 3 - ?, 4 - taunt?, others?
        public byte _volume = 255;
        public ushort _freq = 4096;

        [Description("Global sample index")]
        public ushort SampleID { get; set; } = 0;

        //must be populated based on SampleID
        public Sample Sample = null;

        //this is assumed to be sample length, 1 second is about 300. whatever it is.
        //0 in music samples
        [Description("Defines the delay to send NoteOff event for this sample. Should be 0 for music category. Usually same as sample length (300 ~ 1sec), but can differ.")]
        public short timeToPlay { get; set; } = 0;

        public uint ADSR { get; set; } = 0x1FC180FF;  // assumed to be the raw psx SPU adsr value passed directly to psyq.

        public MetaInst metaInst { get; set; }

        public string Tag => $"{ID}_{Frequency}";
        public string ID => $"{SampleID.ToString("0000")}_{SampleID.ToString("X4")}";

        [Description("Defines frequency played at C5 note (midi 60). Does not always coincide with actual C5. Stored as 4096 = 44100, so expect minor differences between introduced freq and actual freq.")]
        public int Frequency
        {
            get { return (int)Math.Round(_freq / 4096f * 44100f); }
            set { _freq = (ushort)Math.Round(value / 44100f * 4096f); }
        }

        [Description("Defines how loud this sample will be played in the game. Used for playback here as well. Game stores it in the byte range of 0-255, gui maps it to 0.0-1.0")]
        public float Volume
        {
            get { return (float)Math.Round(_volume / 255f, 3); }
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

        public SpuInstrument()
        {
        }

        public SpuInstrument(BinaryReaderEx br) => Read(br);

        public static SpuInstrument FromReader(BinaryReaderEx br) => new SpuInstrument(br);

        public virtual void Read(BinaryReaderEx br)
        {
            flags = br.ReadByte();
            _volume = br.ReadByte();
            timeToPlay = br.ReadInt16();
            _freq = br.ReadUInt16();
            SampleID = br.ReadUInt16();
            ADSR = br.ReadUInt32();

            if (Sample == null && Context != null)
                Sample = Context.FindSample(SampleID);

            if (flags != 1)
                Helpers.Panic(this, PanicType.Assume, $"magic1 != 1: {flags}");

            if (timeToPlay != 0)
                Helpers.Panic(this, PanicType.Assume, $"always0 != 0: {timeToPlay}");
        }

        public virtual void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(flags);
            bw.Write((byte)_volume);
            bw.Write((short)timeToPlay);
            bw.Write((short)_freq);
            bw.Write((short)SampleID);
            bw.Write(ADSR);
        }

        public VagSample GetVagSample(HowlContext context)
        {
            /*
            if (Sample is null)
            {
                Helpers.Panic(this, PanicType.Warning, $"Sample data not found! {this.ID}");
                //Console.ReadKey();
                return null;
            }
            */

            using (var br = new BinaryReaderEx(new MemoryStream(Sample.Data)))
            {
                VagSample vag;

                if (context.SamplePool.ContainsKey(this.SampleID))
                {
                    vag = context.SamplePool[SampleID].GetVag();
                    vag.sampleFreq = Frequency;

                    if (context.HashNames.ContainsKey(Sample.HashString))
                        vag.SampleName = context.HashNames[Sample.HashString];

                    Console.WriteLine($"{Sample.HashString}: {vag.SampleName}");

                    return vag;
                }
                else
                {
                    Helpers.Panic(this, PanicType.Warning, $"Missing sample ID???!!! {this.ID} {this.SampleID}");
                    return null;
                }
            }
        }

        public bool Equals(SpuInstrument other)
        {
            return
                flags == other.flags &&
                ADSR == other.ADSR &&
                Volume == other.Volume &&
                SampleID == other.SampleID &&
                timeToPlay == other.timeToPlay;
        }
    }

    /// <summary>
    /// Shorter version of CSEQ instrument used for percussion.
    /// </summary>
    public class SpuInstrumentShort : SpuInstrument, IEquatable<SpuInstrumentShort>
    {
        public static readonly new int SizeOf = 8;

        public SpuInstrumentShort()
        {
        }

        public SpuInstrumentShort(BinaryReaderEx br) : base(br)
        {
        }

        public new static SpuInstrumentShort FromReader(BinaryReaderEx br) => new SpuInstrumentShort(br);

        public override void Read(BinaryReaderEx br)
        {
            flags = br.ReadByte();
            _volume = br.ReadByte();
            _freq = br.ReadUInt16();
            SampleID = br.ReadUInt16();
            timeToPlay = br.ReadInt16();

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
            bw.Write((ushort)_freq);
            bw.Write((ushort)SampleID);
            bw.Write((short)timeToPlay);
        }

        public bool Equals(SpuInstrumentShort other)
        {
            return
                flags == other.flags &&
                Volume == other.Volume &&
                ADSR == other.ADSR &&
                SampleID == other.SampleID &&
                timeToPlay == other.timeToPlay;
        }

        public override string ToString() => $"magic:{flags}\tvol:{_volume}\tfreq:{_freq}\tid:{SampleID}\tzero:{timeToPlay}";
    }
}