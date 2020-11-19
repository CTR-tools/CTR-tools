using System;
using System.IO;
using CTRFramework.Sound;
using CTRFramework.Shared;

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

                if (!File.Exists(fn))
                {
                    Console.WriteLine("{0} doesn't exist.", fn);
                    return;
                }

                using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(fn)))
                {
                    Howl hwl = Howl.FromReader(br);
                    hwl.DetectHowl(fn);
                    Console.Write(hwl.ToString());

                    hwl.ExportCSEQ(br);
                    hwl.ExportAllSamples();

                    Console.WriteLine("Done!");
                    return;
                }

            }
            else
            {
                Console.WriteLine("Usage:\r\n\thowl.exe <path to KART.HWL>\r\n\r\nPress any key to quit...");
            }
        }
    }
}
