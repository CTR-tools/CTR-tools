using CTRFramework.Shared;
using CTRFramework.Sound.CSeq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CTRFramework.Sound
{
    public class Howl
    {
        //Release rel;
        string name;
        string reg;

        public int version;     //freezes the game if changed, game code tests against fixed number for some reason. maybe like version.


        public static Dictionary<int, string> seqnames = new Dictionary<int, string>();
        public static Dictionary<int, string> banknames = new Dictionary<int, string>();
        public static Dictionary<int, string> samplenames = new Dictionary<int, string>();

        public static string GetName(int x, Dictionary<int, string> dict)
        {
            string result = $"{x.ToString("0000")}_{ x.ToString("X4")}";

            if (dict.ContainsKey(x))
                result += dict[x];

            return result;
        }

        List<ushort> unk = new List<ushort>();

        public static List<SampleDef> samplesSfx = new List<SampleDef>();
        List<SampleDef> samplesEngineSfx = new List<SampleDef>();

        List<int> ptrBanks = new List<int>();
        List<int> ptrSeqs = new List<int>();

        public List<Bank> banks = new List<Bank>();
        public List<CSEQ> sequences = new List<CSEQ>();

        public Howl()
        {
        }

        public Howl(BinaryReaderEx br)
        {
            Read(br);
        }

        public static Howl FromReader(BinaryReaderEx br)
        {
            return new Howl(br);
        }

        public static Howl FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        private void KnownFileCheck(BinaryReaderEx br)
        {
            br.Jump(0);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Helpers.GetTextFromResource(Meta.XmlPath));

            var hash = MD5.Create().ComputeHash(br.BaseStream);
            string md5 = BitConverter.ToString(hash).Replace("-", "");

            br.Jump(0);

            foreach (XmlElement el in doc.SelectNodes("/data/howl/entry"))
            {
                if (md5.ToLower() == el["md5"].InnerText.ToLower())
                {
                    Console.WriteLine($"{md5}\r\n{el["name"].InnerText} [{el["region"].InnerText}] detected.");
                    banknames = Meta.LoadNumberedList(el["banks"].InnerText);
                    samplenames = Meta.LoadNumberedList(el["samples"].InnerText);

                    string[] lines = Helpers.GetLinesFromResource("howlnames.txt");

                    foreach (var line in lines)
                    {
                        if (line.Split(':')[0].Trim() == el["sequences"].InnerText)
                        {
                            string[] songs = line.Split(':')[1].Split(';');

                            for (int i = 0; i < songs.Length; i++)
                            {
                                seqnames.Add(i, songs[i].Trim());
                            }

                            break;
                        }
                    }

                    return;
                }
            }

            Console.WriteLine("Unknown HOWL file.");
        }



        /*
         * 
         * 
         * Write
         
        bw.Write("HOWL".ToCharArray());
            bw.Write(version);
            bw.Write(reserved1);
            bw.Write(reserved2);

            bw.Write(cntUnk);
            bw.Write(cntSfx);
            bw.Write(cntEngineSfx);

            bw.Write(numBanks);
            bw.Write(numSequences);

            bw.Write(sampleDataSize);
         * */

        public void Read(BinaryReaderEx br)
        {
            KnownFileCheck(br);

            char[] magic = br.ReadChars(4);

            if (new string(magic) != "HOWL")
                throw new Exception("Not a CTR HOWL file.");

            version = br.ReadInt32();
            int reserved1 = br.ReadInt32();
            int reserved2 = br.ReadInt32();

            if (reserved1 != 0)
                Helpers.Panic(this, PanicType.Assume, "reserved1 is not null. Possible error.");

            if (reserved2 != 0)
                Helpers.Panic(this, PanicType.Assume, "reserved2 is not null. Possible error.");

            uint numUnkTable = br.ReadUInt32();     //number of entries in an unknown array, messes up all samples if anything is modified
            uint numSfx = br.ReadUInt32();          //number of sample declarations, contains all sfx entries (not instruments)
            uint numEngineSfx = br.ReadUInt32();    //number of engine sound array entries

            uint numBanks = br.ReadUInt32();        //number of banks
            uint numSequences = br.ReadUInt32();    //number of sequences

            int sampleDataSize = br.ReadInt32();    //whole sample data size to the last seq pointer

            for (int i = 0; i < numUnkTable; i++)
            {
                if (br.ReadUInt16() != 0)
                    Helpers.Panic(this, PanicType.Assume, "upper word is not 0.");

                unk.Add(br.ReadUInt16());
            }

            samplesSfx = InstanceList<SampleDef>.FromReader(br, (uint)br.BaseStream.Position, numSfx);
            samplesEngineSfx = InstanceList<SampleDef>.FromReader(br, (uint)br.BaseStream.Position, numEngineSfx);


            for (int i = 0; i < numBanks; i++)
                ptrBanks.Add(br.ReadUInt16() * Meta.SectorSize);

            for (int i = 0; i < numSequences; i++)
                ptrSeqs.Add(br.ReadUInt16() * Meta.SectorSize);


            foreach (var ptr in ptrBanks)
            {
                br.Jump(ptr);
                banks.Add(new Bank(br));
            }

            foreach (var ptr in ptrSeqs)
            {
                br.Jump(ptr);
                sequences.Add(CSEQ.FromReader(br));
            }

            for (int i = 0; i < sequences.Count; i++)
            {
                if (seqnames.ContainsKey(i))
                    sequences[i].name = seqnames[i];
                else
                    sequences[i].name = i.ToString("00");
            }
        }

        public void ExportCSEQ(string path)
        {
            foreach (var seq in sequences)
                seq.Save(Path.Combine(path, $"{seq.name}.cseq"));
        }

        public void ExportCSEQ(string path, BinaryReaderEx br)
        {
            string pathBank = Path.Combine(path, "banks");
            Helpers.CheckFolder(pathBank);

            for (int i = 0; i < ptrBanks.Count - 1; i++)
            {
                br.BaseStream.Position = ptrBanks[i];

                string fn = String.Format($"{i.ToString("00")}_{(banknames.ContainsKey(i) ? banknames[i] : "bank")}.bnk");
                Console.WriteLine("Extracting " + fn);

                fn = Path.Combine(pathBank, fn);

                Helpers.WriteToFile(fn, br.ReadBytes(ptrBanks[i + 1] - ptrBanks[i]));
            }

            Console.WriteLine("---");

            string pathSeq = Path.Combine(path, "sequences");
            Helpers.CheckFolder(pathSeq);

            int j = 0;

            foreach (int i in ptrSeqs)
            {
                string fn = "";

                if (reg != "")
                {
                    fn = String.Format(
                        "{0}_{1}.cseq",
                        j.ToString("00"),
                        seqnames.ContainsKey(j) ? seqnames[j] : "sequence"
                    );
                }
                else
                {
                    fn = String.Format("{0}_{1}.cseq", j.ToString("00"), "sequence");
                }

                Console.WriteLine("Extracting " + fn);

                fn = Path.Combine(pathSeq, fn);

                br.BaseStream.Position = i;
                int size = br.ReadInt32();
                br.BaseStream.Position = i;

                byte[] data = br.ReadBytes(size);
                Helpers.WriteToFile(fn, data);

                j++;
            }
        }

        public void ExportAllSamples(string path)
        {
            string output = Path.Combine(path, "samples");
            Helpers.CheckFolder(output);

            int i = 0;

            foreach (var bank in banks)
            {
                bank.ExportAll(i, output);
                i++;
            }
        }

        public static int GetFreq(int sampleId)
        {
            foreach (SampleDef sd in samplesSfx)
                if (sd.SampleID == sampleId)
                    return sd.Frequency;

            return -1;
        }

        public override string ToString()
        {
            return $"Samples: {samplesSfx.Count}\r\nBanks: {banks.Count}\r\nSequences: {sequences.Count}";
        }
    }
}
