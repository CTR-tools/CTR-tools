using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;
using System.Xml;

namespace CTRFramework.Audio
{
    public class HowlContext
    {
        public Howl howl;   //link to self in case some child needs it

        // this is intended to contain all samples, one sample per index
        public Dictionary<int, Sample> SamplePool = new Dictionary<int, Sample>();
        public List<SpuAddr> SpuPtrTable = new List<SpuAddr>();


        public List<SpuInstrument> EffectsPool = new List<SpuInstrument>();
        public List<SpuInstrument> InstrumentPool = new List<SpuInstrument>();
        public List<SpuInstrument> PercussionPool = new List<SpuInstrument>();

        public Dictionary<int, string> SongNames = new Dictionary<int, string>();
        public Dictionary<int, string> BankNames = new Dictionary<int, string>();
        public Dictionary<string, string> HashNames = new Dictionary<string, string>();


        // call this after howl is fully loaded, perform any actions required to repair all the data links
        // ideally shouldnt be required if all creation funcs are called properly, but just in case
        public void Validate()
        {
            foreach (var sample in SamplePool.Values)
                sample.Context = this;


            var pool = new List<SpuInstrument>();

            pool.AddRange(EffectsPool);
            pool.AddRange(InstrumentPool);
            pool.AddRange(PercussionPool);

            foreach (var instrument in pool)
            {
                instrument.Context = this;
                instrument.Sample = FindSample(instrument.SampleID);


                //make it when you add it to the pool, dummy

                if (instrument.Sample == null) continue;

                if (instrument.Sample.Type == SampleType.Unknown)
                {
                    if (EffectsPool.Contains(instrument)) instrument.Sample.Type = SampleType.SoundEffect;
                    if (InstrumentPool.Contains(instrument)) instrument.Sample.Type = SampleType.Instrument;
                    if (PercussionPool.Contains(instrument)) instrument.Sample.Type = SampleType.Percussion;
                }
                else
                {
                    if (EffectsPool.Contains(instrument) && instrument.Sample.Type != SampleType.SoundEffect)
                        MessageBox.Show($"conflicting types: {instrument.Sample?.Name} is in Effects and it is {instrument.Sample.Type}");

                    if (InstrumentPool.Contains(instrument) && instrument.Sample.Type != SampleType.Instrument)
                        MessageBox.Show($"conflicting types: {instrument.Sample?.Name} is in Instruments and it is {instrument.Sample.Type}");

                    if (PercussionPool.Contains(instrument) && instrument.Sample.Type != SampleType.Percussion)
                        MessageBox.Show($"conflicting types: {instrument.Sample?.Name} is in Percussions and it is {instrument.Sample.Type}");
                }
            }
        }

        public HowlContext(Howl pHowl)
        {
            howl = pHowl;

            // load context names. this actually should account for detected version.
            HashNames = Helpers.LoadTagList(Meta.HashPath);
            BankNames = Helpers.LoadNumberedList("banknames.txt");
        }

        public Sample FindSample(int id)
        {
            if (SamplePool.ContainsKey(id))
                return SamplePool[id];
            else
                return null;
        }

        public int MaxSampleIndex()
        {
            int max = 0;

            foreach (var sample in SamplePool)
            {
                if (sample.Key > max)
                    max = sample.Key;
            }

            return max + 1;
        }

        public static HowlContext Create(Howl howl = null) => new HowlContext(howl);
    }

    // warning, this can't be used for content validation
    // game just refuses to launch if this version mismatches in exe and howl, that's all
    public enum HowlVersion
    {
        DemoTestDrive = 0x6F,
        DemoOPSM = 0x71,
        DemoSpyro = 0x72,
        CrashBetaAug5 = 0x78,
        Proto = 0x7D,
        Release = 0x80 // all release regions have 0x80 here, despite having different layouts
    }

    public struct SpuAddr
    {
        public ushort Ptr; //always 0 in the file, populated at runtime
        public ushort Size; //this is vag data size / 8
    }

    public class Howl : IReadWrite
    {
        //remove later
        //public static Dictionary<int, string> samplenames = new Dictionary<int, string>();

        public HowlContext Context;

        public HowlVersion version;     //freezes the game if changed, game code tests against fixed number for some reason. maybe like version.

        public List<SpuInstrumentShort> EffectsTable = new List<SpuInstrumentShort>();
        public List<SpuInstrumentShort> EngineTable = new List<SpuInstrumentShort>();

        public List<Bank> Banks = new List<Bank>();
        public List<Cseq> Songs = new List<Cseq>();


        List<int> ptrBanks = new List<int>();
        List<int> ptrSeqs = new List<int>();

        // sample data size value is needed later
        int sampleDataSize => Context.SpuPtrTable.Count * 4 + (EffectsTable.Count + EngineTable.Count) * 8 + (Banks.Count + Songs.Count) * 2;

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

        // validates howl against known file checksums.
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

                Context.BankNames = Helpers.LoadNumberedList(el["banks"].InnerText);


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

        /// <summary>
        /// Reads HOWL data from a stream.
        /// </summary>
        /// <param name="br"></param>
        /// <exception cref="Exception"></exception>
        public void Read(BinaryReaderEx br)
        {
            // create new howl context
            Context = HowlContext.Create(this);

            // validate howl against known checksums
            KnownFileCheck(br);

            // read magic value
            char[] magic = br.ReadChars(4);

            if (new string(magic) != "HOWL")
                throw new Exception("Not a CTR HOWL file.");

            // read version
            version = (HowlVersion)br.ReadInt32();

            Helpers.PanicIf(!Enum.IsDefined(typeof(HowlVersion), version), this, PanicType.Warning, $"Unknown HOWL version! {version}");

            // read reserved pointer fields
            int reserved1 = br.ReadInt32(); // probably some runtime ptr
            int reserved2 = br.ReadInt32(); // probably some runtime ptr

            Helpers.PanicIf(reserved1 != 0, this, PanicType.Assume, "reserved1 is not null. Possible error.");
            Helpers.PanicIf(reserved2 != 0, this, PanicType.Assume, "reserved2 is not null. Possible error.");

            // read data sizes

            uint numSpuPtrTable = br.ReadUInt32();  //number of entries in the spu pointer table, entry per sound ID
            uint numSampleTable = br.ReadUInt32();  //number of sample declarations, contains all sfx entries (not instruments)
            uint numEngineTable = br.ReadUInt32();  //number of engine sound array entries

            uint numBanks = br.ReadUInt32();        //number of banks
            uint numSequences = br.ReadUInt32();    //number of sequences

            int sampleDataSize = br.ReadInt32();    //whole sample data size to the last seq pointer

            // read spu table
            // this is basically length of sample data, used to read sample from bank
            // spu pointer is popolated at runtime, this is basically address in spu ram

            Context.SpuPtrTable.Clear();

            for (int i = 0; i < numSpuPtrTable; i++)
            {
                Context.SpuPtrTable.Add(
                    new SpuAddr()
                    {
                        Ptr = br.ReadUInt16(),
                        Size = br.ReadUInt16()
                    }
                );

                // validate pointer value, should be 0
                Helpers.PanicIf(Context.SpuPtrTable[i].Ptr != 0, this, PanicType.Assume, $"spu ram pointer is not 0: {Context.SpuPtrTable[i].Ptr}");
            }


            // read all sfx instruments and push to context effects pool 

            for (int i = 0; i < numSampleTable; i++)
            {
                var x = SpuInstrumentShort.FromReader(br);

                EffectsTable.Add(x);
                Context.EffectsPool.Add(x);
            }

            for (int i = 0; i < numEngineTable; i++)
            {
                var x = SpuInstrumentShort.FromReader(br);

                EngineTable.Add(x);
                Context.EffectsPool.Add(x);
            }

            /*
            foreach (var instrument in samplesSfx)
                Helpers.Panic(this, PanicType.Info, instrument.ToString());

            Console.WriteLine("=======================");

            foreach (var instrument in samplesEngineSfx)
                Helpers.Panic(this, PanicType.Info, instrument.ToString());

            Console.ReadKey();
            */

            // read array pointers for songs and banks
            // values are stored in sectors, so we have to calculate the actual address

            for (int i = 0; i < numBanks; i++)
                ptrBanks.Add(br.ReadUInt16() * Meta.SectorSize);

            for (int i = 0; i < numSequences; i++)
                ptrSeqs.Add(br.ReadUInt16() * Meta.SectorSize);

            // read all banks
            for (int i = 0; i < numBanks; i++)
            {
                br.Jump(ptrBanks[i]);

                var bank = Bank.FromReader(br, Context);
                bank.Context = Context;
                bank.Name = Context.BankNames.ContainsKey(i) ? Context.BankNames[i] : $"bank_{i.ToString("0000")}";
                bank.Index = i;
                Banks.Add(bank);
            }

            // read all songs
            foreach (var ptr in ptrSeqs)
            {
                br.Jump(ptr);

                Songs.Add(Cseq.FromReader(br, Context));
            }

            // patch song data with names and MIDI stuff
            for (int i = 0; i < Songs.Count; i++)
            {
                // get song name
                Songs[i].Name = Context.SongNames.ContainsKey(i) ? Context.SongNames[i] : i.ToString("00");
                
                // get patch name
                Songs[i].PatchName = Songs[i].Name;

                // load midi stuff?
                Songs[i].LoadMetaInstruments();
            }

            // finalize context creation
            Context.Validate();

            // print howl
            Console.WriteLine(ToString());
        }

        /*
        //replace with global indexed sample table later
        public void UpdateSpuTable()
        {
            //fixup spuaddr table
            Context.SpuPtrTable.Clear();

            //populate with empty entries
            for (int i = 0; i < Context.MaxSampleIndex() + 1; i++)
                Context.SpuPtrTable.Add(new SpuAddr());

            //iterate through all samples and calculate the correct value
            for (int i = 0; i < Context.SpuPtrTable.Count() + 1; i++)
            {
                if (Context.SamplePool.ContainsKey(i))
                {
                    Context.SpuPtrTable[i] = new SpuAddr() { Size = (ushort)(Context.SamplePool[i].Data.Length / 8) };
                }
            }
        }

        */

        /// <summary>
        /// Writes HOWL data to binaryWriter.
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="patchTable"></param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            //UpdateSpuTable();

            Console.WriteLine("Writing HOWL...");

            // TODO: check how this acually works under the hood. aint char is utf16 here?
            bw.Write("HOWL".ToCharArray());
            bw.Write((int)version);
            bw.Seek(8);

            bw.Write(Context.SpuPtrTable.Count);
            bw.Write(EffectsTable.Count);
            bw.Write(EngineTable.Count);

            bw.Write(Banks.Count);
            bw.Write(Songs.Count);

            bw.Write(sampleDataSize);

            foreach (var value in Context.SpuPtrTable)
            {
                bw.Write(value.Ptr);
                bw.Write(value.Size);
            }

            foreach (var instrument in EffectsTable)
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

        /// <summary>
        /// Save HOWL to file.
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            Helpers.CheckFolder(Path.GetDirectoryName(filename));

            using (var bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
        }

        // this function exports parsed songs as opposed to raw howl data
        public void ExportCSEQ(string path)
        {
            Helpers.CheckFolder(path);

            Cseq.PatchMidi = true;
            Cseq.IgnoreVolume = true;

            string pathSeq = Helpers.PathCombine(path, "songs");
            Helpers.CheckFolder(pathSeq);

            foreach (var song in Songs)
            {
                song.PatchName = song.Name;
                song.LoadMetaInstruments();

                song.Save(Helpers.PathCombine(pathSeq, $"{song.Name}.cseq"));
                int i = 0;
                foreach (var s in song.Songs)
                {
                    s.ExportMIDI(Helpers.PathCombine(pathSeq, $"{song.Name}_{i}.mid"), song);
                    i++;
                }
            }
        }

        // and this function dumps byte[] arrays
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

                string filename = String.Format($"{i.ToString("00")}_{(Context.BankNames.ContainsKey(i) ? Context.BankNames[i] : "bank")}.bnk");
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
                    //fallback for missing song list
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
                seq.Name = seqFileName;

                if (Context.SongNames.ContainsKey(songIndex))
                    seq.Name = Context.SongNames[songIndex];

                seq.PatchName = seq.Name;
                seq.LoadMetaInstruments();
                int i = 0;

                var midiFolder = Helpers.PathCombine(pathSeq, "midi");
                Helpers.CheckFolder(midiFolder);

                foreach (var s in seq.Songs)
                {
                    s.ExportMIDI(Helpers.PathCombine(midiFolder, $"{seq.Name}_{i}.mid"), seq);
                    i++;
                }

                songIndex++;
            }
        }

        // TODO: probably should move it to context
        // not even sure if it makes any sense, since freq is stored in instrument.
        // maybe better add instrument export, as well as deduplication, in case it's shared between songs
        public void ExportAllSamples(string path)
        {
            string outputwav = Helpers.PathCombine(path, "samples");
            string outputvag = Helpers.PathCombine(outputwav, "vag");

            Helpers.CheckFolder(outputwav);
            Helpers.CheckFolder(outputvag);

            foreach (var sample in EffectsTable)
            {
                var vag = sample.GetVagSample(Context);

                if (vag == null) continue;

                vag.Save(Helpers.PathCombine(outputvag, $"{vag.SampleName}.vag"));
                vag.ExportWav(Helpers.PathCombine(outputwav, $"{vag.SampleName}.wav"));
            }

            foreach (var song in Songs)
            {
                foreach (var sample in song.Percussions)
                {
                    var vag = sample.GetVagSample(Context);

                    vag.Save(Helpers.PathCombine(outputvag, $"{vag.SampleName}.vag"));
                    vag.ExportWav(Helpers.PathCombine(outputwav, $"{vag.SampleName}.wav"));
                }

                foreach (var sample in song.Instruments)
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

        // replaces vag sample
        public void ReplaceVagSample(int sampleIndex, string filename, SpuInstrument instr = null)
        {
            if (!File.Exists(filename)) return;

            if (!Context.SamplePool.ContainsKey(sampleIndex))
            {
                Helpers.Panic(this, PanicType.Error, "no sample index found in the table!");
                return;
            }

            //read vag sample
            var vag = VagSample.FromFile(filename);

            //copy data to existing sample in the table and update the size
            Context.SamplePool[sampleIndex].Data = vag.GetData();
            Context.SpuPtrTable[sampleIndex] = new SpuAddr() { Size = (ushort)(Context.SamplePool[sampleIndex].Data.Length / 8) };

            if (instr != null)
            {
                instr.Frequency = vag.sampleFreq;
            }

            //now find all inst entries and copy the frequency there
            //should not do this for instruments, or like implement some relative frequency updates?
            foreach (var inst in EffectsTable)
            {
                if (inst.SampleID == sampleIndex)
                {
                    inst.Frequency = vag.sampleFreq;
                    inst.Volume = 1.0f;
                    inst.timeToPlay = 0;
                }
            }
        }

        /*
        public static int GetFreq(int sampleId)
        {
            foreach (var sd in Effects)
                if (sd.SampleID == sampleId)
                    return sd.Frequency;

            return -1;
        }
        */

        /// <summary>
        /// Prints basic HOWL data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return 
                $"Version: {version}\r\n"+ 
                $"Spu entries: {Context.SpuPtrTable.Count}\r\n" + 
                $"Samples: {EffectsTable.Count}\r\n" +
                $"Engine samples: {EngineTable.Count}\r\n" +
                $"Banks: {Banks.Count}\r\n" + 
                $"Sequences: {Songs.Count}";
        }
    }
}