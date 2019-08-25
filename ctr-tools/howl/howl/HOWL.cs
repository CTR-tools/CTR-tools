using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CTRFramework.Shared;

namespace howl
{
    class HOWL
    {
        Release rel;
        string name;

        public HowlHeader header;

        List<ushort> unk = new List<ushort>();
        List<SampleDecl> samples1 = new List<SampleDecl>();
        List<SampleDecl> samples2 = new List<SampleDecl>();

        List<int> offbanks = new List<int>();
        List<int> offseqs = new List<int>();

        List<Bank> banks = new List<Bank>();

        public HOWL(string fn)
        {
            name = fn;

            string md5 = Helpers.CalculateMD5(fn);

            Console.WriteLine("Reading " + fn);
            Console.WriteLine("MD5 = " + md5);

            rel = Release.Find(md5);

            if (rel == null)
            {
                Console.WriteLine("Unknown HWL.");
            }
            else
            {
                Console.WriteLine("Detected {0} ({1}).", rel.name, rel.timestamp);
            }
        }

        public bool Read(BinaryReader br)
        {
            header = new HowlHeader(br);

            for (int i = 0; i < header.cnt4; i++)
            {
                if (br.ReadUInt16() != 0) Console.WriteLine("now what");
                unk.Add(br.ReadUInt16());
            }

            for (int i = 0; i < header.cnt81; i++)
                samples1.Add(new SampleDecl(br));

            for (int i = 0; i < header.cnt82; i++)
                samples2.Add(new SampleDecl(br));

            for (int i = 0; i < header.bankCount; i++)
                offbanks.Add(br.ReadUInt16() * (int)0x800);

            for (int i = 0; i < header.seqCount; i++)
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
                            Console.WriteLine("MD5 differs for same ID!!! " + id + "\r\n" + xx + "\r\n" + yy);
                        }
                    }
                }
            }
        

            return true;
        }



        public void ExportCSEQ(BinaryReader br)
        {
            string seqdir = String.Format("{0}_cseq\\", name);
            Directory.CreateDirectory(seqdir);

            string bankdir = String.Format("{0}_bank\\", name);
            Directory.CreateDirectory(bankdir);

            for (int i = 0; i<offbanks.Count-1; i++)
            {
                br.BaseStream.Position = offbanks[i];

                string fn = String.Format("{0}.bnk", i.ToString("00"));
                Console.WriteLine("Extracting " + fn);

                fn = bankdir + fn;

                File.WriteAllBytes(fn, br.ReadBytes(offbanks[i+1]-offbanks[i]));
            }

            Console.WriteLine("---");

            int j = 0;

            foreach (int i in offseqs)
            {
                string fn = String.Format(
                    "{0}_{1}.cseq",
                    j.ToString("00"), 
                    (j < rel.fileList.Length) ? rel.fileList[j] : "sequence"
                );

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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("banks: " + header.bankCount + "\r\n");
            sb.Append("sequences: " + header.seqCount + "\r\n");

            return sb.ToString();
        }
    }
}
