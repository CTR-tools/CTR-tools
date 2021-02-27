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
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-Tools: howl - {Meta.GetSignature()}",
                "Extracts samples and music sequences from HOWL",
                Meta.GetVersion());

            if (args.Length == 0)
            {
                Console.WriteLine("Usage:\r\n\thowl.exe <path to KART.HWL>");
                return;
            }

            string filename = args[0];

            if (!File.Exists(filename))
            {
                Console.WriteLine("{0} doesn't exist.", filename);
                return;
            }

            using (BinaryReaderEx br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                Howl hwl = Howl.FromReader(br);
                hwl.DetectHowl(filename);
                Console.Write(hwl.ToString());

                hwl.ExportCSEQ(br);
                hwl.ExportAllSamples();

                Console.WriteLine("Done!");
                return;
            }
        }
    }
}