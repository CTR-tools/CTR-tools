using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms.VisualStyles;
using System.Xml;

namespace CTRFramework.Sound
{
    public class HowlContext
    {
        public Dictionary<int, string> SongNames = new Dictionary<int, string>();
        public Dictionary<int, string> banknames = new Dictionary<int, string>();
        public Dictionary<string, string> HashNames = new Dictionary<string, string>();

        public static HowlContext Create() => new HowlContext();
    }

    public enum HowlVersion
    {
        UsDemo = 0x6F,
        PalDemoOPSM = 0x71,
        PalDemoSpyro = 0x72,
        Proto = 0x7D,
        Ntsc = 0x80
    }

    public class Howl : IReadWrite
    {
        //remove later
        public static Dictionary<int, string> samplenames = new Dictionary<int, string>();

        public HowlContext Context;

        public HowlVersion version;     //freezes the game if changed, game code tests against fixed number for some reason. maybe like version.

        List<ushort> SpuPtrTable = new List<ushort>(); //this is speculated to be related to some vab offsets

        public Dictionary<int, Sample> Samples = new Dictionary<int, Sample>();

        public static List<InstrumentShort> SampleTable = new List<InstrumentShort>();
        public static List<InstrumentShort> EngineTable = new List<InstrumentShort>();

        public List<Bank> Banks = new List<Bank>();
        public List<Cseq> Songs = new List<Cseq>();

        public static string GetName(int x, Dictionary<int, string> dict)
        {
            string result = $"{x.ToString("0000")}_{x.ToString("X4")}";

            if (dict.ContainsKey(x))
                result += "_" + dict[x];

            return result;
        }

        List<int> ptrBanks = new List<int>();
        List<int> ptrSeqs = new List<int>();


        #region [Constructors, Factories]
        public Howl()
        {
        }

        public Howl(BinaryReaderEx br) => Read(br);

        public static Howl FromReader(BinaryReaderEx br) => new Howl(br);

        public static Howl FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }
        #endregion

        private void KnownFileCheck(BinaryReaderEx br)
        {
            int pos = (int)br.BaseStream.Position;

            Context.HashNames = Helpers.LoadTagList(Meta.HashPath);

            br.Jump(pos);

            string md5 = Helpers.CalculateMD5(br.BaseStream);

            br.Jump(pos);

            var doc = Helpers.LoadXml(Meta.XmlPath);

            foreach (XmlElement el in doc.SelectNodes("/data/howl/entry"))
            {
                if (md5.ToUpper() != el["md5"].InnerText.ToUpper()) continue;

                Console.WriteLine($"{md5}\r\n{el["name"].InnerText} [{el["region"].InnerText}] detected.");

                Context.banknames = Helpers.LoadNumberedList(el["banks"].InnerText);
                //Context.hashnames = Helpers.LoadTagList(el["samples"].InnerText);

                string[] lines = Helpers.GetLinesFromResource("howlnames.txt");

                foreach (var line in lines)
                {
                    if (line.Split(':')[0].Trim() == el["sequences"].InnerText)
                    {
                        string[] songs = line.Split(':')[1].Trim().Split(';');

                        for (int i = 0; i < songs.Length; i++)
                        {
                            Context.SongNames.Add(i, songs[i].Trim());
                        }

                        break;
                    }
                }

                return;
            }

            Console.WriteLine("Unknown HOWL file.");
        }


        public void Read(BinaryReaderEx br)
        {
            Context = HowlContext.Create();

            KnownFileCheck(br);

            char[] magic = br.ReadChars(4);

            if (new string(magic) != "HOWL")
                throw new Exception("Not a CTR HOWL file.");

            version = (HowlVersion)br.ReadInt32();
            int reserved1 = br.ReadInt32();
            int reserved2 = br.ReadInt32();

            if (reserved1 != 0)
                Helpers.Panic(this, PanicType.Assume, "reserved1 is not null. Possible error.");

            if (reserved2 != 0)
                Helpers.Panic(this, PanicType.Assume, "reserved2 is not null. Possible error.");

            uint numSpuPtrTable = br.ReadUInt32();  //number of entries in an unknown array, messes up all samples if anything is modified
            uint numSampleTable = br.ReadUInt32();  //number of sample declarations, contains all sfx entries (not instruments)
            uint numEngineTable = br.ReadUInt32();  //number of engine sound array entries

            uint numBanks = br.ReadUInt32();        //number of banks
            uint numSequences = br.ReadUInt32();    //number of sequences

            int sampleDataSize = br.ReadInt32();    //whole sample data size to the last seq pointer

            for (int i = 0; i < numSpuPtrTable; i++)
            {
                if (br.ReadUInt16() != 0)
                    Helpers.Panic(this, PanicType.Assume, "upper word is not 0.");

                SpuPtrTable.Add(br.ReadUInt16());
            }

            SampleTable = InstanceList<InstrumentShort>.FromReader(br, (UIntPtr)br.Position, numSampleTable);
            EngineTable = InstanceList<InstrumentShort>.FromReader(br, (UIntPtr)br.Position, numEngineTable);

            /*
            foreach (var instrument in samplesSfx)
                Helpers.Panic(this, PanicType.Info, instrument.ToString());

            Console.WriteLine("=======================");

            foreach (var instrument in samplesEngineSfx)
                Helpers.Panic(this, PanicType.Info, instrument.ToString());

            Console.ReadKey();
            */

            for (int i = 0; i < numBanks; i++)
                ptrBanks.Add(br.ReadUInt16() * Meta.SectorSize);

            for (int i = 0; i < numSequences; i++)
                ptrSeqs.Add(br.ReadUInt16() * Meta.SectorSize);


            for (int i = 0; i < numBanks; i++)
            {
                br.Jump(ptrBanks[i]);
                var bank = Bank.FromReader(br, Context);
                bank.Context = Context;
                bank.Name = Context.banknames[i];
                bank.Index = i;
                Banks.Add(bank);
            }


            foreach (var bank in Banks)
                foreach (var sample in bank.samples.Values)
                {
                    if (!Samples.ContainsKey(sample.ID))
                        Samples.Add(sample.ID, sample);
                }


            foreach (var ptr in ptrSeqs)
            {
                br.Jump(ptr);
                Songs.Add(Cseq.FromReader(br));
            }

            for (int i = 0; i < Songs.Count; i++)
            {
                Songs[i].name = Context.SongNames.ContainsKey(i) ? Context.SongNames[i] : i.ToString("00");
                Songs[i].PatchName = Songs[i].name;

                Songs[i].LoadMetaInstruments();
            }

            foreach (var sample in SampleTable)
                sample.Sample = FindSample(sample.SampleID);

            foreach (var cseq in Songs)
            {
                foreach (var sample in cseq.samples)
                    sample.Sample = FindSample(sample.SampleID);

                foreach (var sample in cseq.samplesReverb)
                    sample.Sample = FindSample(sample.SampleID);
            }

            Console.WriteLine(ToString());

            /*
            var sb = new StringBuilder();

            var samples = new Dictionary<int, Sample>();

            int maxid = 0;

            foreach (var bank in Banks)
                foreach (var sample in bank.samples.Values)
                    if (!samples.ContainsKey(sample.ID))
                    {
                        samples.Add(sample.ID, sample);
                        if (maxid < sample.ID)
                            maxid = sample.ID;
                    }

            for (int i = 0; i < maxid; i++)
                if (!samples.ContainsKey(i))
                    samples.Add(i, new Sample() { ID = -1, Data = new byte[] { } });


            foreach (var sample in samples)
            {
                sb.AppendLine($"{sample.Key},{sample.Value.Hash.ToString("X8")}");
            }
            */

            /*
            int maxid = 0;

            foreach (var bank in Banks)
                foreach (var sample in bank.samples.Values)
                    if (!samples.ContainsKey(sample.ID))
                    {
                        samples.Add(sample.ID, sample);
                        if (maxid < sample.ID)
                            maxid = sample.ID;
                    }

            for (int i = 0; i <= maxid; i++)
                sb.AppendLine($"{i}, {(samples.ContainsKey(i) ? samples[i].Hash.ToString("X8") : "")}");
s
            Helpers.WriteToFile(Helpers.PathCombine(Meta.BasePath, "test.txt"), sb.ToString());
            */
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            Console.WriteLine("Writing HOWL...");

            bw.Write("HOWL".ToCharArray());
            bw.Write((int)version);
            bw.Seek(8);

            bw.Write(SpuPtrTable.Count);
            bw.Write(SampleTable.Count);
            bw.Write(EngineTable.Count);

            bw.Write(Banks.Count);
            bw.Write(Songs.Count);

            bw.Write(SpuPtrTable.Count * 4 + (SampleTable.Count + EngineTable.Count) * 8 + (Banks.Count + Songs.Count) * 2); //sampleDataSize

            foreach (var value in SpuPtrTable)
            {
                bw.Write((short)0);
                bw.Write(value);
            }

            foreach (var instrument in SampleTable)
                instrument.Write(bw);

            foreach (var instrument in EngineTable)
                instrument.Write(bw);



            int ptrs = (int)bw.BaseStream.Position;

            bw.Seek(2 * (Banks.Count + Songs.Count));

            bw.JumpNextSector();

            var offsets = new List<uint>();

            foreach (var bank in Banks)
            {
                offsets.Add((uint)bw.BaseStream.Position);
                bank.Write(bw);
                bw.JumpNextSector();
            }

            foreach (var song in Songs)
            {
                offsets.Add((uint)bw.BaseStream.Position);
                song.Write(bw);
                bw.JumpNextSector();
            }

            bw.Jump(ptrs);

            foreach (var ptr in offsets)
                bw.Write((short)(ptr / Meta.SectorSize));

            Console.WriteLine("HOWL saved.");
        }

        public void Save(string filename)
        {
            Helpers.CheckFolder(Path.GetDirectoryName(filename));

            using (var bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
        }

        public void ExportCSEQ(string path)
        {
            Helpers.CheckFolder(path);

            Cseq.PatchMidi = true;
            Cseq.IgnoreVolume = true;

            string pathSeq = Helpers.PathCombine(path, "songs");
            Helpers.CheckFolder(pathSeq);

            foreach (var song in Songs)
            {
                song.PatchName = song.name;
                song.LoadMetaInstruments();

                song.Save(Helpers.PathCombine(pathSeq, $"{song.name}.cseq"));
                int i = 0;
                foreach (var s in song.Songs)
                {
                    s.ExportMIDI(Helpers.PathCombine(pathSeq, $"{song.name}_{i}.mid"), song);
                    i++;
                }
            }
        }

        public Sample FindSample(int id)
        {
            foreach (var bank in Banks)
                if (bank.samples.ContainsKey(id))
                    return bank.samples[id];

            return null;
        }

        public void Export(string path, BinaryReaderEx br)
        {
            Cseq.PatchMidi = true;
            Cseq.IgnoreVolume = true;

            string pathBank = Helpers.PathCombine(path, "banks");
            Helpers.CheckFolder(pathBank);

            Banks.Clear();

            for (int i = 0; i < ptrBanks.Count; i++)
            {
                br.Jump(ptrBanks[i]);

                string filename = String.Format($"{i.ToString("00")}_{(Context.banknames.ContainsKey(i) ? Context.banknames[i] : "bank")}.bnk");
                filename = Helpers.PathCombine(pathBank, filename);

                var bank = Bank.FromReader(br, Context);
                bank.Save(filename);
                Banks.Add(bank);

                /*
                int pos = (int)br.Position;
                if (i < ptrBanks.Count - 1)
                    pos = ptrBanks[i + 1];

                br.Jump(ptrBanks[i]);
                Helpers.WriteToFile(fn, br.ReadBytes(pos - ptrBanks[i]));

                Banks.Add(Bank.FromFile(fn));
                */
            }

            Console.WriteLine("---");

            string pathSeq = Helpers.PathCombine(path, "songs");
            Helpers.CheckFolder(pathSeq);

            int songIndex = 0;

            foreach (int ptrSeq in ptrSeqs)
            {
                string seqFileName = "";
                
                if (Context.SongNames is null)
                {
                    //fallback for missing 
                    seqFileName = $"sequence_{songIndex.ToString("00")}.cseq";
                }
                else
                {
                    seqFileName = String.Format(
                        "{0}_{1}.cseq",
                        songIndex.ToString("00"),
                        Context.SongNames.ContainsKey(songIndex) ? Context.SongNames[songIndex] : "sequence"
                    );
                }

                Helpers.Panic("HOWL", PanicType.Info, $"Extracting {seqFileName}");

                var seqFullPath = Helpers.PathCombine(pathSeq, seqFileName);

                br.Jump(ptrSeq);
                int size = br.ReadInt32();
                br.Jump(ptrSeq);

                byte[] data = br.ReadBytes(size);
                Helpers.WriteToFile(seqFullPath, data);

                var seq = Cseq.FromFile(seqFullPath);
                seq.name = seqFileName;

                if (Context.SongNames.ContainsKey(songIndex))
                    seq.name = Context.SongNames[songIndex];

                seq.PatchName = seq.name;
                seq.LoadMetaInstruments();
                int i = 0;

                var midiFolder = Helpers.PathCombine(pathSeq, "midi");
                Helpers.CheckFolder(midiFolder);

                foreach (var s in seq.Songs)
                {
                    s.ExportMIDI(Helpers.PathCombine(midiFolder, $"{seq.name}_{i}.mid"), seq);
                    i++;
                }

                songIndex++;
            }
        }

        public void ExportAllSamples(string path)
        {
            string outputwav = Helpers.PathCombine(path, "samples");
            string outputvag = Helpers.PathCombine(outputwav, "vag");

            Helpers.CheckFolder(outputwav);
            Helpers.CheckFolder(outputvag);

            foreach (var sample in SampleTable)
            {
                var vag = sample.GetVagSample(Context);

                if (vag == null) continue;

                vag.Save(Helpers.PathCombine(outputvag, $"{vag.SampleName}.vag"));
                vag.ExportWav(Helpers.PathCombine(outputwav, $"{vag.SampleName}.wav"));
            }


            foreach (var song in Songs)
            {
                foreach (var sample in song.samples)
                {
                    var vag = sample.GetVagSample(Context);

                    vag.Save(Helpers.PathCombine(outputvag, $"{vag.SampleName}.vag"));
                    vag.ExportWav(Helpers.PathCombine(outputwav, $"{vag.SampleName}.wav"));
                }

                foreach (var sample in song.samplesReverb)
                {
                    var vag = sample.GetVagSample(Context);

                    vag.Save(Helpers.PathCombine(outputvag, $"{vag.SampleName}.vag"));
                    vag.ExportWav(Helpers.PathCombine(outputwav, $"{vag.SampleName}.wav"));
                }
            }


            /*
            int i = 0;

            foreach (var bank in Banks)
            {
                bank.ExportAll(i, output);
                i++;
            }
            */
        }

        public static int GetFreq(int sampleId)
        {
            foreach (var sd in SampleTable)
                if (sd.SampleID == sampleId)
                    return sd.Frequency;

            return -1;
        }

        public override string ToString()
        {
            return $"Version: {version}\r\nspuIndices: {SpuPtrTable.Count}\r\nSamples: {SampleTable.Count}\r\nBanks: {Banks.Count}\r\nSequences: {Songs.Count}";
        }
    }
}