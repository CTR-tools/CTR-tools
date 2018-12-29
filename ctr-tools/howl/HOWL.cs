using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
namespace howl
{



    struct intpair
    {
        int A;
        int B;

        public void Read(BinaryReader br)
        {
            A = br.ReadInt32();
            B = br.ReadInt32();
        }
    }


    class HOWL
    {
        Release rel;

        string name;

        string magic;

        int u1;
        int uz1;
        int uz2;

        int cnt4;
        int cnt81;
        int cnt82;

        public int bankCount;
        public int seqCount;

        int u2;

        List<int> offbanks = new List<int>();
        List<int> offseqs = new List<int>();

        public HOWL(string fn)
        {
            name = fn;

            string md5 = CalculateMD5(fn);

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
            string magic = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4));

            if (magic != "HOWL")
            {
                Console.WriteLine("Not a CTR HOWL file.");
                return false;
            }

            u1 = br.ReadInt32();
            uz1 = br.ReadInt32();
            uz2 = br.ReadInt32();

            if (uz1 != 0 || uz2 != 0)
            {
                Console.WriteLine("uz1 or uz2 is not null. Possible error.");
            }

            cnt4 = br.ReadInt32();
            cnt81 = br.ReadInt32();
            cnt82 = br.ReadInt32();

            bankCount = br.ReadInt32();
            seqCount = br.ReadInt32();

            u2 = br.ReadInt32();

            br.BaseStream.Position += cnt4 * 4;
            br.BaseStream.Position += cnt81 * 8;
            br.BaseStream.Position += cnt82 * 8;

            for (int i = 0; i < bankCount; i++)
                offbanks.Add(br.ReadUInt16() * (int)0x800);

            for (int i = 0; i < seqCount; i++)
                offseqs.Add(br.ReadUInt16() * (int)0x800);

            return true;
        }




        public void ExportCSEQ(BinaryReader br)
        {
            string dir = String.Format("{0}_data\\", name);
            Directory.CreateDirectory(dir);

            int j = 0;

            foreach (int i in offseqs)
            {
                string fn = String.Format(
                    "{0}_{1}.cseq",
                    j.ToString("00"), 
                    (j < rel.fileList.Length) ? rel.fileList[j] : "sequence"
                );

                Console.WriteLine("Extracting " + fn);

                fn = dir + fn;

                br.BaseStream.Position = i;
                int size = br.ReadInt32();
                br.BaseStream.Position = i;

                byte[] data = br.ReadBytes(size);
                File.WriteAllBytes(fn, data);

                j++;
            }
        }


        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("banks: " + bankCount + "\r\n");
            sb.Append("sequences: " + seqCount + "\r\n");

            return sb.ToString();
        }
    }
}
