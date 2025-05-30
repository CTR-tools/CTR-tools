﻿using CTRFramework.Audio;
using CTRFramework.Shared;
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
                Meta.Version);

            if (args.Length == 0)
            {
                Console.WriteLine("Usage:\r\n\tExtract HWL:\thowl.exe C:\\example\\KART.HWL\r\n\tExtract BNK:\thowl.exe C:\\example\\01_canyon.bnk");
                Console.Write("Press any key...");
                Console.ReadKey();
                return;
            }

            string filename = args[0];

            Console.WriteLine($"Input file: {filename}");

            if (!File.Exists(filename))
            {
                Console.WriteLine($"File not found: {filename}.");
                return;
            }

            string basepath = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);

            string path = Helpers.PathCombine(basepath, Path.GetFileNameWithoutExtension(filename));

            switch (ext.ToUpper())
            {
                case ".HWL":
                    using (var br = new BinaryReaderEx(File.OpenRead(filename)))
                    {
                        var hwl = Howl.FromReader(br);

                        hwl.Export(path, br);
                        hwl.ExportAllSamples(path);

                        Console.WriteLine("Done!");
                    }
                    break;

                //the lcd extension comes from doom/final doom
                //it is speculated, that the bank format comes from libWESS (Williams Entertainment Sound System) 
                case ".LCD":
                case ".BNK":
                    Bank.ReadNames();
                    var bnk = Bank.FromFile(filename);
                    throw new NotImplementedException("commented out bank export since need howl for this to work.");
                    //bnk.ExportAll(0, Helpers.PathCombine(basepath, name));
                    break;

                case ".XNF":
                    var xnf = XaInfo.FromFile(filename);
                    Console.WriteLine(xnf.ToString());
                    break;

                case ".CSEQ":
                    var seq = Cseq.FromFile(filename);
                    seq.Songs[0].ExportMIDI(Path.ChangeExtension(filename, ".mid"), seq);
                    break;

                /*
                case ".MID":
                    var midseq = Cseq.FromMidi(filename);
                    midseq.Save(Path.ChangeExtension(filename, ".cseq"));
                    break;
                */

                case ".XA":
                    var xa = XaStackedFrameCollection.FromFile(filename);
                    Console.WriteLine(xa.ToString());
                    break;

                default:
                    Console.WriteLine("Unsupported file.");
                    break;
            }
        }
    }
}