using CTRFramework.Shared;
using CTRFramework.Sound.CSeq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Sound
{
    public class Howl
    {
        //Release rel;
        string name;
        string reg;
        string[] names;

        public HowlHeader header;

        List<ushort> unk = new List<ushort>();
        public static List<SampleDef> samples1 = new List<SampleDef>();
        List<SampleDef> samples2 = new List<SampleDef>();

        List<int> ptrBanks = new List<int>();
        List<int> ptrSeqs = new List<int>();

        public List<Bank> banks = new List<Bank>();
        public List<CSEQ> sequences = new List<CSEQ>();

        public static int GetFreq(int sampleId)
        {
            foreach (SampleDef sd in samples1)
            {
                if (sd.SampleID == sampleId) return sd.Frequency;
            }

            return -1;
        }

        public Howl(string fn)
        {
            DetectHowl(fn);
        }

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



        public static Dictionary<int, string> sampledict = new Dictionary<int, string>();

        public static bool ReadSampleNames(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    sampledict.Clear();

                    string[] buf = File.ReadAllLines(path);

                    foreach (string b in buf)
                    {
                        if (b.Trim() != "")
                        {
                            if (b.ToCharArray()[0] != '#')
                            {
                                string[] bb = b.Replace(" ", "").Split('=');

                                int x = -1;
                                Int32.TryParse(bb[0], out x);

                                if (x == -1)
                                {
                                    Console.WriteLine("List parsing error at: {0}", b);
                                    continue;
                                }

                                Console.WriteLine(x + " " + bb[1]);
                                sampledict.Add(x, bb[1]);
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public Howl(BinaryReaderEx br)
        {
            Read(br);
        }


        public static Howl FromFile(string filename)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return Howl.FromReader(br);
            }
        }


        public static Howl FromReader(BinaryReaderEx br)
        {
            return new Howl(br);
        }


        public bool Read(BinaryReaderEx br)
        {
            if (File.Exists(Meta.SmplPath))
                ReadSampleNames(Meta.SmplPath);

            header = new HowlHeader(br);

            for (int i = 0; i < header.cntUnk; i++)
            {
                if (br.ReadUInt16() != 0)
                {
                    Console.WriteLine("HOWL Read: upper word not 0.");
                    Console.ReadKey();
                }

                unk.Add(br.ReadUInt16());
            }

            for (int i = 0; i < header.cntSfx; i++)
                samples1.Add(new SampleDef(br));
            /*
             * 
             * dump whole header to sql later
            StringBuilder sb = new StringBuilder();
            foreach (SampleDecl sd in samples1)
            {
                sb.Append(sd.)
            }
            */

            for (int i = 0; i < header.cntEngineSfx; i++)
                samples2.Add(new SampleDef(br));

            for (int i = 0; i < header.cntBank; i++)
                ptrBanks.Add(br.ReadUInt16() * (int)0x800);

            for (int i = 0; i < header.cntSeq; i++)
                ptrSeqs.Add(br.ReadUInt16() * (int)0x800);


            foreach (int ptr in ptrBanks)
            {
                br.Jump(ptr);
                Bank x = new Bank();
                x.Read(br);
                banks.Add(x);
            }

            foreach (int ptr in ptrSeqs)
            {
                br.Jump(ptr);
                sequences.Add(CSEQ.FromReader(br));
            }


            Bank sfx = new Bank();

            foreach (Bank b in banks)
            {
                foreach (var x in b.samples)
                {
                    int id = x.Key;

                    if (!sfx.samples.ContainsKey(id))
                    {
                        sfx.samples.Add(x.Key, x.Value);
                    }
                    else
                    {
                        string xx = Helpers.CalculateMD5(new MemoryStream(sfx.samples[id]));
                        string yy = Helpers.CalculateMD5(new MemoryStream(b.samples[id]));
                        if (xx != yy)
                        {
                            //Console.WriteLine("MD5 differs for same ID!!! " + id + "\r\n" + xx + "\r\n" + yy);
                        }
                    }
                }
            }


            return true;
        }



        public void ExportCSEQ()
        {
            int i = 0;
            foreach (var c in sequences)
            {
                c.Export($".\\seqs\\{i}.cseq");
                i++;
            }
        }

        public void ExportCSEQ(BinaryReaderEx br)
        {
            string seqdir = String.Format("{0}_cseq\\", name);
            Directory.CreateDirectory(seqdir);

            string bankdir = String.Format("{0}_bank\\", name);
            Directory.CreateDirectory(bankdir);

            for (int i = 0; i < ptrBanks.Count - 1; i++)
            {
                br.BaseStream.Position = ptrBanks[i];

                string fn = String.Format("{0}.bnk", i.ToString("00"));
                Console.WriteLine("Extracting " + fn);

                fn = bankdir + fn;

                File.WriteAllBytes(fn, br.ReadBytes(ptrBanks[i + 1] - ptrBanks[i]));
            }

            Console.WriteLine("---");

            int j = 0;

            foreach (int i in ptrSeqs)
            {
                string fn = "";

                if (reg != "")
                {
                    fn = String.Format(
                        "{0}_{1}.cseq",
                        j.ToString("00"),
                        (j < names.Length) ? names[j] : "sequence"
                    );
                }
                else
                {
                    fn = String.Format("{0}_{1}.cseq", j.ToString("00"), "sequence");
                }

                Console.WriteLine("Extracting " + fn);

                fn = seqdir + fn;

                br.BaseStream.Position = i;
                int size = br.ReadInt32();
                br.BaseStream.Position = i;

                byte[] data = br.ReadBytes(size);
                File.WriteAllBytes(fn, data);

                j++;
            }
        }

        public void ExportAllSamples()
        {
            int i = 0;
            foreach (Bank b in banks)
            {
                b.ExportAll(i, Path.GetDirectoryName(name));
                i++;
            }
        }

        public static string[] ReadNames(string listname)
        {
            string[] lines = File.ReadAllLines(Meta.HowlPath);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] x = lines[i].Replace("\t", "").Replace(" ", "").Split(':');

                if (x[0] == listname)
                    return x[1].Split(';');
            }

            return null;
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
