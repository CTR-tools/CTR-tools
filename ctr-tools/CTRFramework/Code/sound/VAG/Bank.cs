using CTRFramework.Shared;
using Force.Crc32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CTRFramework.Audio
{
    // determine sample type by the instrument it is used in.
    // user should not be allowed to change this, it should be inherited from the instrument
    // aka fill in automatically based on the sample usage
    public enum SampleType
    {
        Unknown,
        Instrument,
        Percussion,
        SoundEffect
    }

    public class Sample
    {
        public SampleType Type = SampleType.Unknown;

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

        public VagSample GetVag(int freq = 11025) => new VagSample(Data) { sampleFreq = freq };

        public void SaveVag(string path, int freq = 11025) => GetVag(freq).Save(path);

        public void SaveWav(string path, int freq = 11025) => GetVag(freq).ExportWav(path);
    }


    public class Bank
    {
        public HowlContext Context;

        public string Name = "default_name";
        public int Index = -1;

        public List<short> Entries = new List<short>();

        public ushort numEntries => (ushort)Entries.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(short id)
        {
            return Entries.Contains(id);
        }

        public static void ReadNames()
        {
            //banknames = Helpers.LoadNumberedList("banknames.txt");
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

            // sanity check for a bank
            if (numSamples > 1023)
                throw new Exception("Unlikely a CTR bank.");

            // read all sample index entries
            Entries = br.ReadListInt16(numSamples);

            br.JumpNextSector();

            // foreach sample entry
            foreach (int index in Entries)
            {
                // calculate sample size
                int sampleSize = Context.SpuPtrTable[index].Size * 8;

                // if sample pool doesnt have this sample yet
                if (!Context.SamplePool.ContainsKey(index))
                {
                    // create a sample
                    var sample = new Sample()
                    {
                        Data = br.ReadBytes(sampleSize),
                        ID = index
                    };

                    // add a sample
                    Context.SamplePool.Add(index, sample);
                }
                else
                {
                    // skip sample data
                    br.Seek(sampleSize);
                }
            }
        }

        /// <summary>
        /// Writes bank binary data.
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="patchTable"></param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            // the amount of samples
            bw.Write(numEntries);

            // write every sample index
            foreach (short index in Entries)
                bw.Write(index);

            // pad to 0x800
            bw.JumpNextSector();

            // write sample data 
            foreach (short index in Entries)
            {
                if (!Context.SamplePool.ContainsKey(index))
                    throw new Exception($"sample {index} not found in SamplePool!!!");

                bw.Write(Context.SamplePool[index].Data);
            }
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

            if (Contains((short)id))
            {
                string vagname = id.ToString("0000"); // Howl.GetName(id, Howl.samplenames);

                if (File.Exists(Helpers.PathCombine(pathSfxVag, $"{vagname}.vag")))
                    return;

                if (File.Exists(Helpers.PathCombine(path, $"{vagname}.wav")))
                    return;

                using (var br = new BinaryReaderEx(new MemoryStream(Context.SamplePool[id].Data)))
                {
                    var vag = new VagSample();

                    if (freq != -1)
                        vag.sampleFreq = freq;

                    vag.ReadFrames(br, Context.SamplePool[id].Data.Length);

                    string hash = Context.SamplePool[id].HashString;

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

        /*
        public void ExportAll(int bnum, string path, string path2 = null)
        {
            string pathSfxVag = Helpers.PathCombine(path, "vag");
            Helpers.CheckFolder(pathSfxVag);

            Parallel.ForEach(Entries, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, sample =>
            {
                Export(sample.ID, Howl.GetFreq(sample.ID), path, pathSfxVag, path2, $"{sample.ID.ToString("0000")}_{sample.ID.ToString("X4")}");
            });

            Console.Write(".");
        }
        */


        /// <summary>
        /// Validates whether all CSEQ samples are contained in this bank.
        /// </summary>
        /// <param name="cseq"></param>
        /// <returns></returns>
        public bool MatchesCseq(Cseq cseq)
        {
            foreach (var inst in cseq.Instruments)
                if (!Entries.Contains((short)inst.Sample.ID))
                    return false;

            foreach (var inst in cseq.Percussions)
                if (!Entries.Contains((short)inst.Sample.ID))
                    return false;

            return true;
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
