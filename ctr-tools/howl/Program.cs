using CTRFramework.Shared;
using CTRFramework.Sound;
using System;
using System.IO;

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

                if (File.Exists(fn))
                {
                    byte[] data = File.ReadAllBytes(fn);
                    MemoryStream ms = new MemoryStream(data);
                    BinaryReaderEx br = new BinaryReaderEx(ms);

                    Howl hwl = new Howl(fn);
                    hwl.Read(br);

                    Console.Write(hwl.ToString());

                    hwl.ExportCSEQ(br);
                    hwl.ExportAllSamples();

                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.WriteLine("{0} doesn't exist.", fn);
                }

            }
            else
            {
                Console.WriteLine("Usage:\r\n\thowl.exe <path to KART.HWL>\r\n\r\nPress any key to quit...");
            }

            //Console.ReadKey();
        }
    }
}
