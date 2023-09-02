using CTRFramework.Shared;
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
        public HowlContext Context;

        private const uint nullhash = 0xFFFFFFFF;

        public string Name
        {
            get
            {
                var name = $"0x{HashString}";

                if (Context is null)
                {
                    return name;
                }

                if (Context.HashNames.ContainsKey(HashString))
                    name = Context.HashNames[HashString];

                return name;

            }
        }

        public int ID;

        private byte[] _data;
        private uint _hash = nullhash;

        public byte[] Data
        {
            get => _data;
            set
            {
                _data = value;
                _hash = GetHash();
            }
        }

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

        private static Crc32Algorithm algo = new Crc32Algorithm();

        private uint GetHash()
        {
            byte[] hashdata = algo.ComputeHash(Data);
            Array.Reverse(hashdata, 0, 4);
            _hash = BitConverter.ToUInt32(hashdata, 0);
            return _hash;
        }

        public VagSample GetVag(int freq = 22100) => new VagSample(Data) { sampleFreq = freq };

        public void SaveVag(string path, int freq = 22100) => GetVag(freq).Save(path);

        public void SaveWav(string path, int freq = 22100) => GetVag(freq).ExportWav(path);
    }


    public class Bank
    {
        public HowlContext Context;

        public string Name = "default_name";
        public int Index = -1;

        public static Dictionary<int, string> banknames = new Dictionary<int, string>();

        public Dictionary<int, Sample> Entries = new Dictionary<int, Sample>();
        public ushort numEntries => (ushort)Entries.Count;


        public bool Contains(int key) => Entries.ContainsKey(key);

        public static void ReadNames()
        {
            banknames = Helpers.LoadNumberedList("banknames.txt");
        }

        #region [Constructors, factories]
        public Bank()
        {
        }

        public Bank(BinaryReaderEx br, HowlContext context = null) => Read(br, context);

        public static Bank FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static Bank FromReader(BinaryReaderEx br, HowlContext context = null) => new Bank(br, context);
        #endregion

        public void Read(BinaryReaderEx br, HowlContext context = null)
        {
            if (context != null) Context = context;

            int bankoffset = (int)br.Position;

            ushort numSamples = br.ReadUInt16();

            if (numSamples > 1023)
                throw new Exception("Unlikely a bank.");

            short[] indexTable = br.ReadArrayInt16(numSamples);

            br.JumpNextSector();


            foreach (int index in indexTable)
            {
                //if sample cache doesnt have this sample yet
                if (!Context.Samples.ContainsKey(index))
                {
                    //create a sample
                    var sample = new Sample()
                    {
                        Data = br.ReadBytes(Context.SpuPtrTable[index].Size * 8),
                        ID = index
                    };

                    //add a sample
                    Context.Samples.Add(index, sample);
                }
                else
                {
                    br.Seek(Context.SpuPtrTable[index].Size * 8);
                }

                //add link to the list
                Entries.Add(index, Context.Samples[index]);
            }

            return;

            /*
            //this is the older sample guessing algo
            int sam_start = (int)br.BaseStream.Position;

            int flag = 0;
            int loops = 0;
            int frames = 0;

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
                        ID = indexTable[loops]
                    };

                    sample.Context = context;

                    if (!context.Samples.ContainsKey(indexTable[loops]))
                        context.Samples.Add(indexTable[loops], sample);

                    Entries.Add(sample.ID, sample);

                    sam_start = (int)br.Position;

                    frames = 0;

                    loops++;
                }
            }
            while (loops < numSamples);
            */
        }

        /// <summary>
        /// Writes bank binary data.
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="patchTable"></param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //the amount of samples
            bw.Write(numEntries);

            //write every sample index
            foreach (var sample in Entries)
                bw.Write((short)sample.Key);

            //pad to 0x800
            bw.JumpNextSector();

            //write sample data 
            foreach (var sample in Entries)
                bw.Write(Context.Samples[sample.Value.ID].Data);
        }

        public void Save(string filename)
        {
            using (var bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
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

                using (var br = new BinaryReaderEx(new MemoryStream(Entries[id].Data)))
                {
                    var vag = new VagSample();

                    if (freq != -1)
                        vag.sampleFreq = freq;

                    vag.ReadFrames(br, Entries[id].Data.Length);

                    string hash = Entries[id].HashString;

                    if (Context.HashNames.ContainsKey(hash))
                        vag.SampleName = Context.HashNames[hash];

                    vagname = $"{id.ToString("0000")}_{hash}";

                    if (vag.SampleName != "")
                        vagname += $"_{vag.SampleName}";

                    vag.Save(Helpers.PathCombine(pathSfxVag, $"{vagname}.vag"));
                    vag.ExportWav(Helpers.PathCombine(path, $"{vagname}.wav"));
                }
            }
        }

        public void ExportAll(int bnum, string path, string path2 = null)
        {
            string pathSfxVag = Helpers.PathCombine(path, "vag");
            Helpers.CheckFolder(pathSfxVag);

            Parallel.ForEach(Entries, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, sample =>
            {
                Export(sample.Key, Howl.GetFreq(sample.Key), path, pathSfxVag, path2, $"{sample.Key.ToString("0000")}_{sample.Key.ToString("X4")}");
            });

            Console.Write(".");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Entries.Count + " samples total\r\n");

            int cnt = 0;

            foreach (var sample in Entries)
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
