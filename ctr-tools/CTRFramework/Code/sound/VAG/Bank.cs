﻿using CTRFramework.Shared;
using Force.Crc32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CTRFramework.Sound
{
    public class Sample
    {
        private const uint nullhash = 0xFFFFFFFF;

        public int ID;

        private byte[] _data;

        public byte[] Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _hash = nullhash;
            }
        }

        private uint _hash = nullhash;

        public uint Hash
        {
            get
            {
                if (_hash != nullhash)
                    return _hash;

                return GetHash();
            }
        }

        public uint GetHash()
        {
            _hash = BitConverter.ToUInt32(Crc32Algorithm.Create().ComputeHash(Data), 0);
            return _hash;
        }
    }

    public class Bank : IReadWrite
    {
        public static Dictionary<string, string> hashnames = new Dictionary<string, string>();
        public static Dictionary<int, string> banknames = new Dictionary<int, string>();

        public Dictionary<int, Sample> samples = new Dictionary<int, Sample>();

        public bool Contains(int key) => samples.ContainsKey(key);

        public static void ReadNames()
        {
            banknames = Meta.LoadNumberedList("banknames.txt");
            hashnames = Meta.LoadTagList("samplehashes.txt");
        }

        #region [Constructors, factories]
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
                return FromReader(br);
            }
        }

        public static Bank FromReader(BinaryReaderEx br)
        {
            return new Bank(br);
        }
        #endregion

        public void Read(BinaryReaderEx br)
        {
            int bankoffset = (int)br.Position;

            int sampCnt = br.ReadInt16();
            short[] info = br.ReadArrayInt16(sampCnt);

            int sam_start = bankoffset + 0x800;

            int flag = 0;

            int loops = 0;

            int frames = 0;

            br.Jump(sam_start);

            do
            {
                br.Seek(1);
                flag = br.ReadByte();
                br.Seek(14);

                frames++;

                if (flag == 7 || flag == 3)
                {
                    br.Jump(sam_start);

                    var sample = new Sample()
                    {
                        Data = br.ReadBytes(frames * 16),
                        ID = info[loops]
                    };

                    samples.Add(sample.ID, sample);

                    sam_start = (int)br.Position;

                    frames = 0;

                    loops++;
                }
            }
            while (loops < sampCnt);

            Parallel.ForEach(samples.Values, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, sample => { sample.GetHash(); });

            return;

            int sample_start = 0;
            int sample_end = 0;


            byte[] buf;

            buf = br.ReadBytes(16);

            if (!frameIsEmpty(buf))
                Helpers.Panic(this, PanicType.Assume, "Expected this to be a null frame.");

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

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((short)samples.Keys.Count);

            foreach (var sample in samples)
                bw.Write((short)sample.Key);

            bw.Jump((int)((bw.BaseStream.Position + 2047) >> 11 << 11));

            foreach (var sample in samples)
                bw.Write(sample.Value.Data);
        }

        public void Export(int id, int freq, string path, string path2 = null, string name = null)
        {
            string pathSfxVag = Path.Combine(path, "vag");

            Helpers.CheckFolder(pathSfxVag);

            if (Contains(id))
            {
                string vagname = id.ToString("0000"); // Howl.GetName(id, Howl.samplenames);

                using (var br = new BinaryReaderEx(new MemoryStream(samples[id].Data)))
                {
                    VagSample vag = new VagSample();

                    if (freq != -1)
                        vag.sampleFreq = freq;

                    if (Howl.samplenames.ContainsKey(id))
                        vag.SampleName = Howl.samplenames[id];

                    vag.ReadFrames(br, samples[id].Data.Length);

                    string hash = samples[id].Hash.ToString("X8").ToUpper();

                    vagname = $"{id.ToString("0000")}_{samples[id].Hash.ToString("X8")}";

                    if (Bank.hashnames.ContainsKey(hash))
                        vagname += $"_{Bank.hashnames[hash]}";

                    vag.Save(Path.Combine(pathSfxVag, $"{vagname}.vag"));
                    vag.ExportWav(Path.Combine(path, $"{vagname}.wav"));
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
