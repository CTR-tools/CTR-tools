using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework.Sound
{
    public class VagSample : IReadWrite
    {
        public static int DefaultSampleRate = 11025;

        private int version = 3;
        private int reserved = 0;
        public int dataSize => numFrames * 16;
        public int numFrames => Frames.Count;
        public int sampleFreq = DefaultSampleRate;
        private int unk1 = 0;
        private int unk2 = 0;
        private int unk3 = 0;
        public string SampleName = "default_name";

        public bool isLooped
        {
            get
            {
                foreach (var frame in Frames)
                    if (frame.flags == 3 || frame.flags == 2 || frame.flags == 6)
                        return true;

                return false;
            }
        }

        public List<VagFrame> Frames = new List<VagFrame>();

        #region [Constructors, factories]
        public VagSample()
        {
        }

        public string HashString = "";
        public VagSample(BinaryReaderEx br) => Read(br);

        public static VagSample FromReader(BinaryReaderEx br) => new VagSample(br);

        /// <summary>
        /// Creates VagSample instance from file.
        /// </summary>
        /// <param name="filename">Source file name.</param>
        /// <returns></returns>
        public static VagSample FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }
        #endregion

        /// <summary>
        /// Read VAG data from stream using binary reader.
        /// </summary>
        /// <param name="br">BinaryReaderEx instance.</param>
        public void Read(BinaryReaderEx br)
        {
            var magic = new string(br.ReadChars(4));

            //vagedit exports vags without magic string for some reason, so only warn
            if (magic != "VAGp")
                Helpers.Panic(this, PanicType.Assume, $"No VAGp found. Possibly not a VAG file: magic = {magic}");

            version = br.ReadInt32Big();

            if (version != 3)
                Helpers.Panic(this, PanicType.Assume, $"Version != 3: {version}.");

            reserved = br.ReadInt32Big();

            if (reserved != 0)
                Helpers.Panic(this, PanicType.Assume, "reserved != 0...");

            int dataSize = br.ReadInt32Big();
            sampleFreq = br.ReadInt32Big();
            unk1 = br.ReadInt32Big();
            unk2 = br.ReadInt32Big();
            unk3 = br.ReadInt32Big();

            SampleName = br.ReadStringFixed(16);

            ReadFrames(br, dataSize);
        }

        public void ReadFrames(BinaryReaderEx br, int dataSize)
        {
            for (int i = 0; i < dataSize / 16; i++)
                Frames.Add(VagFrame.FromReader(br));
        }

        /// <summary>
        /// Saves VAG to file.
        /// </summary>
        /// <param name="filename">Target file name.</param>
        public void Save(string filename)
        {
            using (var bw = new BinaryWriterEx(File.Create(filename)))
            {
                Write(bw);
            }
        }

        /// <summary>
        /// Writes VAG data to stream using binary writer.
        /// </summary>
        /// <param name="bw">BinaryWriterEx object.</param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //make sure magic string and version are correct
            version = 3;

            bw.Write("VAGp".ToCharArray());
            bw.WriteBig(version);
            bw.WriteBig(reserved);
            bw.WriteBig(dataSize);
            bw.WriteBig(sampleFreq);
            bw.WriteBig(unk1);
            bw.WriteBig(unk2);
            bw.WriteBig(unk3);

            int pos = (int)bw.BaseStream.Position;
            bw.Write(SampleName.ToCharArray());
            bw.Jump(pos + 16);

            foreach (var frame in Frames)
                frame.Write(bw);
        }

        /// <summary>
        /// Exports decompressed WAV sound.
        /// </summary>
        /// <param name="filename">Target WAV filename.</param>
        public void ExportWav(string filename)
        {
            using (var wav = new BinaryWriterEx(File.Create(filename)))
            {
                wav.Write("RIFF".ToCharArray());
                wav.Write((int)0);
                wav.Write("WAVE".ToCharArray());
                wav.Write("fmt ".ToCharArray());
                wav.Write((int)16);
                wav.Write((short)1);
                wav.Write((short)1);
                wav.Write(sampleFreq);
                wav.Write(sampleFreq * 2);
                wav.Write((short)2);
                wav.Write((short)16);
                wav.Write("data".ToCharArray());
                wav.Write((int)0);

                //every next frame uses these values from previous frame, hence passed as ref
                double s_1 = 0.0;
                double s_2 = 0.0;

                foreach (var frame in Frames)
                    wav.Write(frame.GetRawData(ref s_1, ref s_2));

                int streamSize = (int)wav.BaseStream.Position;

                wav.Jump(4);
                wav.Write(streamSize - 8);

                wav.Jump(40);
                wav.Write(streamSize - 44);
            }
        }
    }
}