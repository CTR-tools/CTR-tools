using CTRFramework.Shared;
using Force.Crc32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTRFramework.Sound
{
    public class Sample
    {
        public string Name = "default_name";

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

        public string HashString => Hash.ToString("X8").ToUpper();

        private Crc32Algorithm algo = new Crc32Algorithm();

        public uint GetHash()
        {
            byte[] hashdata = algo.ComputeHash(Data);
            Array.Reverse(hashdata, 0, 4);
            _hash = BitConverter.ToUInt32(hashdata, 0);
            return _hash;
        }
    }

    public class Bank : IReadWrite
    {
        public string Name = "default_name";
        public int Index = -1;

        public static Dictionary<string, string> hashnames = new Dictionary<string, string>();
        public static Dictionary<int, string> banknames = new Dictionary<int, string>();

        public Dictionary<int, Sample> samples = new Dictionary<int, Sample>();
        public Dictionary<int, Sample> Entries = new Dictionary<int, Sample>();

        public bool Contains(int key) => samples.ContainsKey(key);

        public static void ReadNames()
        {
            banknames = Helpers.LoadNumberedList("banknames.txt");
            hashnames = Helpers.LoadTagList("samplehashes.txt");
        }

        #region [Constructors, factories]
        public Bank()
        {
        }

        public Bank(BinaryReaderEx br) => Read(br);

        public static Bank FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static Bank FromReader(BinaryReaderEx br) => new Bank(br);
        #endregion

        public void Read(BinaryReaderEx br)
        {
            int bankoffset = (int)br.Position;

            int sampCnt = br.ReadInt16();

            if (sampCnt > 1023)
                throw new Exception("Unlikely a bank.");

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

            Parallel.ForEach(samples.Values, new ParallelOptions() {
                MaxDegreeOfParallelism = 32 }, 
                sample => {
                    sample.GetHash();
                    sample.Name = hashnames.ContainsKey(sample.HashString) ? hashnames[sample.HashString] : "default_name";
                });
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((short)samples.Keys.Count);

            foreach (var sample in samples)
                bw.Write((short)sample.Key);

            bw.Jump((int)((bw.BaseStream.Position + Meta.SectorSize - 1) >> 11 << 11));

            foreach (var sample in samples)
                bw.Write(sample.Value.Data);
        }

        public void Export(int id, int freq, string path, string pathSfxVag = null, string path2 = null, string name = null)
        {
            //string pathSfxVag = Helpers.PathCombine(path, "vag");

            //Helpers.CheckFolder(pathSfxVag);

            if (Contains(id))
            {
                string vagname = id.ToString("0000"); // Howl.GetName(id, Howl.samplenames);

                if (File.Exists(Helpers.PathCombine(pathSfxVag, $"{vagname}.vag")))
                    return;

                if (File.Exists(Helpers.PathCombine(path, $"{vagname}.wav")))
                    return;

                using (var br = new BinaryReaderEx(new MemoryStream(samples[id].Data)))
                {
                    var vag = new VagSample();

                    if (freq != -1)
                        vag.sampleFreq = freq;

                    if (Howl.samplenames.ContainsKey(id))
                        vag.SampleName = Howl.samplenames[id];

                    vag.ReadFrames(br, samples[id].Data.Length);

                    string hash = samples[id].HashString;

                    vagname = $"{id.ToString("0000")}_{hash}";

                    if (Bank.hashnames.ContainsKey(hash))
                        vagname += $"_{Bank.hashnames[hash]}";

                    vag.Save(Helpers.PathCombine(pathSfxVag, $"{vagname}.vag"));
                    vag.ExportWav(Helpers.PathCombine(path, $"{vagname}.wav"));
                }
            }
        }

        public void ExportAll(int bnum, string path, string path2 = null)
        {
            string pathSfxVag = Helpers.PathCombine(path, "vag");
            Helpers.CheckFolder(pathSfxVag);

            Parallel.ForEach(samples, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, sample => {
                Export(sample.Key, Howl.GetFreq(sample.Key), path, pathSfxVag, path2, $"{sample.Key.ToString("0000")}_{sample.Key.ToString("X4")}");
            });

            Console.Write(".");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(samples.Count + " samples total\r\n");

            int cnt = 0;

            foreach (var sample in samples)
            {
                sb.AppendLine(sample.ToString());
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
