using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework.Sound
{
    public class VagSample
    {
        public string magic;
        public int version;
        public int reserved;
        public int dataSize;
        public int sampleFreq;
        public int unk1;
        public int unk2;
        public int unk3;
        public string name;

        public List<VagFrame> Frames = new List<VagFrame>();

        public VagSample()
        {
        }

        public VagSample(BinaryReaderEx br)
        {
            Read(br);
        }

        public static VagSample FromReader(BinaryReaderEx br)
        {
            return new VagSample(br);
        }

        public static VagSample FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return new VagSample(br);
            }
        }

        public void Read(BinaryReaderEx br)
        {
            magic = new string(br.ReadChars(4));

            Console.WriteLine(magic);

            if (magic != "VAGp")
                throw new Exception("Not a VAG file.");

            version = br.ReadInt32Big();
            reserved = br.ReadInt32Big();

            if (reserved != 0)
                Console.WriteLine("reserved != 0...");

            dataSize = br.ReadInt32Big();
            sampleFreq = br.ReadInt32Big();
            unk1 = br.ReadInt32Big();
            unk2 = br.ReadInt32Big();
            unk3 = br.ReadInt32Big();

            name = br.ReadStringFixed(16);

            for (int i = 0; i < dataSize / 16; i++)
                Frames.Add(VagFrame.FromReader(br));
        }

        /// <summary>
        /// Converts VAG sample to WAV.
        /// Converted from C++ source: https://github.com/ColdSauce/psxsdk/blob/master/tools/vag2wav.c
        /// </summary>
        /// <param name="filename">Target WAV filename.</param>
        public void ConvertToWav(string filename)
        {
            using (BinaryWriterEx wav = new BinaryWriterEx(File.Create(filename)))
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

                int d, s;
                double s_1 = 0.0;
                double s_2 = 0.0;

                double[] samples = new double[28];
                double[,] f = {
                        { 0.0, 0.0 },
                        { 60.0 / 64.0,  0.0 },
                        { 115.0 / 64.0, -52.0 / 64.0 },
                        { 98.0 / 64.0, -55.0 / 64.0 },
                        { 122.0 / 64.0, -60.0 / 64.0 }
                    };


                foreach (VagFrame frame in Frames)
                {
                    if (frame.flags == 7)
                        break;

                    for (int i = 0; i < 28; i += 2)
                    {
                        d = frame.data[i / 2];
                        s = (d & 0xf) << 12;

                        if ((s & 0x8000) > 0)
                            s = (int)(s | 0xffff0000);

                        samples[i] = (double)(s >> frame.shift_factor);

                        s = (d & 0xf0) << 8;

                        if ((s & 0x8000) > 0)
                            s = (int)(s | 0xffff0000);

                        samples[i + 1] = (double)(s >> frame.shift_factor);
                    }

                    for (int i = 0; i < 28; i++)
                    {
                        samples[i] = samples[i] + s_1 * f[frame.predict_nr, 0] + s_2 * f[frame.predict_nr, 1];
                        s_2 = s_1;
                        s_1 = samples[i];
                        d = (int)Math.Round(samples[i] + 0.5f);

                        wav.Write((short)d);
                    }
                }

                int streamSize = (int)wav.BaseStream.Position;

                wav.Jump(4);
                wav.Write(streamSize - 8);

                wav.Jump(40);
                wav.Write(streamSize - 44);
            }
        }
    }
}