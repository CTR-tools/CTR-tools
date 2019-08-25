using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;


namespace howl
{
    class Program
    {

       static void Main(string[] args)
        {
           Console.WriteLine("Crash Team Racing HOWL Extractor by DCxDemo*.\r\n");

            if (args.Length == 1)
            {
                string fn = args[0];

                fn = "kart.hwl";

                if (File.Exists(fn))
                {
                    byte[] data = File.ReadAllBytes(fn);
                    MemoryStream ms = new MemoryStream(data);
                    BinaryReader br = new BinaryReader(ms);

                    HOWL hwl = new HOWL(fn);
                    hwl.Read(br);

                    Console.Write(hwl.ToString());

                    hwl.ExportCSEQ(br);

                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.Write(fn + " doesn't exist.");
                }

            }
            else
            {
                Console.WriteLine("Usage:\r\n\thowl.exe <path to KART.HWL>\r\n\r\nPress any key to quit...");
            }

            Console.ReadKey();
        }
    }
}
