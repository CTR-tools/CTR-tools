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

        public HowlHeader header;

        List<ushort> unk = new List<ushort>();
        public static List<SampleDef> samples1 = new List<SampleDef>();
        List<SampleDef> samples2 = new List<SampleDef>();

        List<int> ptrBanks = new List<int>();
        List<int> ptrSeqs = new List<int>();

        public List<Bank> banks = new List<Bank>();
        public List<CSEQ> sequences = new List<CSEQ>();

        public Howl()
        {
        }

        private void KnownFileCheck(BinaryReaderEx br)
        {
            br.Jump(0);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Helpers.GetTextFromResource(Meta.XmlPath));

            var hash = MD5.Create().ComputeHash(br.BaseStream);
            string md5 = BitConverter.ToString(hash).Replace("-", "");

            foreach (XmlElement el in doc.SelectNodes("/data/howl/entry"))
            {
                if (md5.ToLower() == el["md5"].InnerText.ToLower())
                {
                    Console.WriteLine($"{md5}\r\n{el["name"].InnerText} [{el["region"].InnerText}] detected.");
                    banknames = Meta.LoadNumberedList(el["banks"].InnerText);
                    seqnames = Meta.LoadNumberedList(el["sequences"].InnerText);
                    samplenames = Meta.LoadNumberedList(el["samples"].InnerText);
                    return;
                }
            }

            br.Jump(0);

            Console.WriteLine("Unknown HOWL file.");
        }

        /*
            public void DetectHowl(string fn)
        {
            name = fn;
            reg = Meta.Detect(fn, "howls", 0);

            if (reg != "")
            {
                names = ReadNames(reg);
            }
            else
            {
                Console.WriteLine("Unknown HWL.");
            }
        }
        */

        public Howl(BinaryReaderEx br)
        {
            Read(br);
        }


        public static Howl FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static Howl FromReader(BinaryReaderEx br)
        {
            return new Howl(br);
        }

        public void Read(BinaryReaderEx br)
        {
            KnownFileCheck(br);

            header = HowlHeader.FromReader(br);

            for (int i = 0; i < header.cntUnk; i++)
            {
                if (br.ReadUInt16() != 0)
                    Helpers.Panic(this, PanicType.Assume, "upper word is not 0.");

                unk.Add(br.ReadUInt16());
            }

            samples1 = InstanceList<SampleDef>.FromReader(br, (uint)br.BaseStream.Position, (uint)header.cntSfx);
            samples2 = InstanceList<SampleDef>.FromReader(br, (uint)br.BaseStream.Position, (uint)header.cntEngineSfx);


            for (int i = 0; i < header.cntBank; i++)
                ptrBanks.Add(br.ReadUInt16() * Meta.SectorSize);

            for (int i = 0; i < header.cntSeq; i++)
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
        }


        public void ExportCSEQ(string path)
        {
            int i = 0;

            foreach (var c in sequences)
            {
                c.Save(Path.Combine(path, $"{i.ToString("00")}.cseq"));
                i++;
            }
        }

        public void ExportCSEQ(string path, BinaryReaderEx br)
        {
            string pathBank = Path.Combine(path, "banks");
            Helpers.CheckFolder(pathBank);

            for (int i = 0; i < ptrBanks.Count - 1; i++)
            {
                br.BaseStream.Position = ptrBanks[i];

                string fn = String.Format($"{i.ToString("00")}_{(Bank.banknames.ContainsKey(i) ? Bank.banknames[i] : "bank")}.bnk");
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
            int i = 0;
            foreach (Bank b in banks)
            {
                b.ExportAll(i, Path.Combine(path, "samples"));
                i++;
            }
        }

        public static string[] ReadNames(string listname)
        {
            string[] lines = Helpers.GetLinesFromResource(Meta.HowlPath);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] x = lines[i].Replace("\t", "").Replace(" ", "").Split(':');

                if (x[0] == listname)
                    return x[1].Split(';');
            }

            return null;
        }

        public static int GetFreq(int sampleId)
        {
            foreach (SampleDef sd in samples1)
            {
                if (sd.SampleID == sampleId) return sd.Frequency;
            }

            return -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("banks: " + header.cntBank + "\r\n");
            sb.Append("sequences: " + header.cntSeq + "\r\n");

            return sb.ToString();
        }
    }
}
