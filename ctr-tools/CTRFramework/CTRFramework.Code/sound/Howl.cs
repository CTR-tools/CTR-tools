using CTRFramework.Shared;
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
        public static List<SampleDecl> samples1 = new List<SampleDecl>();
        List<SampleDecl> samples2 = new List<SampleDecl>();

        List<int> offbanks = new List<int>();
        List<int> offseqs = new List<int>();

        List<Bank> banks = new List<Bank>();

        public static int GetFreq(int sampleId)
        {
            foreach (SampleDecl sd in samples1)
            {
                if (sd.sampleID == sampleId) return sd.frequency;
            }

            return -1;
        }

        public Howl(string fn)
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


        public bool Read(BinaryReaderEx br)
        {
            if (File.Exists(Meta.BasePath + "CTRFramework.Data\\samplenames.txt"))
                ReadSampleNames(Meta.BasePath + "CTRFramework.Data\\samplenames.txt");

            header = new HowlHeader(br);

            for (int i = 0; i < header.cnt4; i++)
            {
                if (br.ReadUInt16() != 0)
                {
                    Console.WriteLine("HOWL Read: upper word not 0.");
                    Console.ReadKey();
                }

                unk.Add(br.ReadUInt16());
            }

            for (int i = 0; i < header.cnt81; i++)
                samples1.Add(new SampleDecl(br));
            /*
             * 
             * dump whole header to sql later
            StringBuilder sb = new StringBuilder();
            foreach (SampleDecl sd in samples1)
            {
                sb.Append(sd.)
            }
            */

            for (int i = 0; i < header.cnt82; i++)
                samples2.Add(new SampleDecl(br));

            for (int i = 0; i < header.cntBank; i++)
                offbanks.Add(br.ReadUInt16() * (int)0x800);

            for (int i = 0; i < header.cntSeq; i++)
                offseqs.Add(br.ReadUInt16() * (int)0x800);


            foreach (int i in offbanks)
            {
                br.BaseStream.Position = i;
                Bank x = new Bank();
                x.Read(br);
                banks.Add(x);
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



        public void ExportCSEQ(BinaryReaderEx br)
        {
            string seqdir = String.Format("{0}_cseq\\", name);
            Directory.CreateDirectory(seqdir);

            string bankdir = String.Format("{0}_bank\\", name);
            Directory.CreateDirectory(bankdir);

            for (int i = 0; i < offbanks.Count - 1; i++)
            {
                br.BaseStream.Position = offbanks[i];

                string fn = String.Format("{0}.bnk", i.ToString("00"));
                Console.WriteLine("Extracting " + fn);

                fn = bankdir + fn;

                File.WriteAllBytes(fn, br.ReadBytes(offbanks[i + 1] - offbanks[i]));
            }

            Console.WriteLine("---");

            int j = 0;

            foreach (int i in offseqs)
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
