using CTRFramework.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Force.Crc32;
using System;

namespace CTRFramework.Sound
{
    public class Sample
    {
        private const uint nullhash = 0xFFFFFFFF;

        public int ID;

        private byte[] _data;

        public byte[] Data {
            get { return _data;  }
            set
            {
                _data = value;
                _hash = nullhash;
            }
        }

        private uint _hash = nullhash;

        public uint Hash {
            get
            {
                if (_hash != nullhash)
                    return _hash;

                _hash = BitConverter.ToUInt32(Crc32Algorithm.Create().ComputeHash(Data), 0);

                return _hash;
            }
        }
    }

    public class Bank
    {
        public static Dictionary<int, string> banknames = new Dictionary<int, string>();

        public Dictionary<int, Sample> samples = new Dictionary<int, Sample>();

        public static void ReadNames()
        {
            banknames = Meta.LoadNumberedList("banknames.txt");
        }

        public Bank()
        {
        }

        public Bank(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Bank FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return Bank.FromReader(br);
            }
        }

        public static Bank FromReader(BinaryReaderEx br)
        {
            return new Bank(br);
        }

        public void Read(BinaryReaderEx br)
        {
            int bankoffset = (int)br.Position;
            int sampCnt = br.ReadInt16();
            short[] info = br.ReadArrayInt16(sampCnt);

            br.Jump(bankoffset + 0x800);

            int sample_start = 0;
            int sample_end = 0;


            byte[] buf;

            for (int i = 0; i < sampCnt; i++)
            {
                sample_start = (int)br.Position;

                br.Seek(16);

                do
                {
                    buf = br.ReadBytes(16);
                }
                while (!frameIsEmpty(buf));

                sample_end = (int)br.Position;

                br.Jump(sample_start);

                if (!samples.ContainsKey(info[i]))
                {
                    samples.Add(info[i], new Sample() { ID = info[i], Data = br.ReadBytes(sample_end - sample_start) });
                }
                else
                {
                    Helpers.Panic(this, PanicType.Warning, $"dupe key: {info[i]}");
                }
            }
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write((short)samples.Keys.Count);

            foreach (var sample in samples)
                bw.Write((short)sample.Key);

            bw.Jump((int)((bw.BaseStream.Position + 2047) >> 11 << 11));

            foreach (var sample in samples)
                bw.Write(sample.Value.Data);
        }

        public bool Contains(int key)
        {
            return samples.ContainsKey(key);
        }

        public void Export(int id, int freq, string path, string path2 = null, string name = null)
        {
            string pathSfxVag = Path.Combine(path, "wav");
            string pathSfxWav = Path.Combine(path, "vag");

            Helpers.CheckFolder(pathSfxVag);
            Helpers.CheckFolder(pathSfxWav);

            if (Contains(id))
            {
                string vagpath = Path.Combine(path, (path2 == null ? "vag" : path2));
                Directory.CreateDirectory(vagpath);

                //string vagname = vagpath + "\\" +  (name == null ?  (Howl.sampledict.ContainsKey(id) ? Howl.sampledict[id] : "sample_" + id.ToString("0000")) : name) + ".vag";
                string vagname = Path.Combine(vagpath, $"{Howl.GetName(id, Howl.samplenames)}.vag");


                /*
                if (name != null)
                {
                    vagname += (name != null ? name : "sample") + (Howl.samplenames.ContainsKey(id) ? "_" + Howl.samplenames[id] : "") + ".vag";
                }
                else
                {
                    vagname += "sample_" + id.ToString("0000") + ".vag";
                }
                */

                //Console.WriteLine(vagname);

                using (var br = new BinaryReaderEx(new MemoryStream(samples[id].Data)))
                {
                    VagSample vag = new VagSample();
                    if (freq != -1)
                        vag.sampleFreq = freq;
                    if (Howl.samplenames.ContainsKey(id))
                        vag.SampleName = Howl.samplenames[id];
                    vag.ReadFrames(br, samples[id].Data.Length);

                    vag.Save(vagname);
                    vag.ExportWav(vagname.Replace("vag", "wav")); //lmao
                }
            }
        }

        public void ExportAll(int bnum, string path, string path2 = null)
        {
            int i = 0;

            foreach (var s in samples)
            {
                Export(s.Key, Howl.GetFreq(s.Key), path, path2, s.Key.ToString("0000") + "_" + s.Key.ToString("X4"));
                i++;
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(samples.Count + " samples total\r\n");

            int cnt = 0;

            foreach (var s in samples)
            {
                sb.Append(s.ToString() + "\r\n");
                cnt++;
            }

            return sb.ToString();
        }



        #region [Private functions]

        private bool frameIsEmpty(byte[] buf)
        {
            foreach (byte b in buf)
                if (b != 0) return false;

            return true;
        }

        #endregion
    }
}
