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
                "Extracts samples and music sequences from HOWL and BNK",
                Meta.GetVersion());

            if (args.Length == 0)
            {
                Console.WriteLine("Usage:\r\n\tExtract HWL:\thowl.exe C:\\example\\KART.HWL\r\n\tExtract BNK:\thowl.exe C:\\example\\01_canyon.bnk");
                return;
            }

            string filename = args[0];

            Console.WriteLine($"Input file: {filename}");

            if (!File.Exists(filename))
            {
                Console.WriteLine($"{filename} doesn't exist.");
                return;
            }

            string basepath = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename).ToLower();

            string path = Helpers.PathCombine(basepath, Path.GetFileNameWithoutExtension(filename));

            switch (ext)
            {
                case ".hwl":
                    using (var br = new BinaryReaderEx(File.OpenRead(filename)))
                    {
                        Howl hwl = Howl.FromReader(br);

                        hwl.ExportCSEQ(path, br);
                        hwl.ExportAllSamples(path);

                        //hwl.Banks[1].samples[0x1ae] = hwl.Banks[1].samples[0x143];
                        //hwl.Save(Path.ChangeExtension(filename, ".hwl_test"));

                        Console.WriteLine("Done!");
                    }
                    break;

                case ".lcd":
                case ".bnk":
                    Bank.ReadNames();
                    var bnk = Bank.FromFile(filename);
                    bnk.ExportAll(0, Helpers.PathCombine(basepath, name));
                    break;

                case ".xnf":
                    var xnf = XaInfo.FromFile(filename);
                    Console.WriteLine(xnf.ToString());
                    break;

                case ".cseq":
                    var seq = Cseq.FromFile(filename);
                    seq.Songs[0].ExportMIDI(Path.ChangeExtension(filename, ".mid"), seq);
                    break;

                case ".mid":
                    var midseq = Cseq.FromMidi(filename);
                    midseq.Save(Path.ChangeExtension(filename, ".cseq"));
                    break;

                default:
                    Console.WriteLine("Unsupported file.");
                    break;
            }
        }
    }
}