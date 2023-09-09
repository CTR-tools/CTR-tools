using CTRFramework.Lang;
using CTRFramework.Shared;
using System;
using System.IO;

namespace lang
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-Tools: lng2txt - {Meta.GetSignature()}",
                "Converts LNG localization files to TXT and back.",
                Meta.Version);

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}",
                    "Usage",
                    "Convert to text", "lng2txt c:\\en.lng",
                    "Convert to .lng", "lng2txt c:\\en.txt"
                    );
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Current path: " + Environment.CurrentDirectory);

            foreach (string filename in args)
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Missing file: {filename}");
                    continue;
                }

                try
                {
                    ProcessFile(filename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    continue;
                }
            }

            Console.WriteLine("Done.");
        }

        static void ProcessFile(string filename)
        {
            Console.WriteLine($"Input file: {filename}");

            string ext = Path.GetExtension(filename);

            switch (ext.ToUpper())
            {
                case ".LNG":
                    var lng = LNG.FromFile(filename);
                    lng.Export(Path.ChangeExtension(filename, "txt"), true);
                    break;

                case ".TXT":
                    lng = LNG.FromText(File.ReadAllLines(filename), true);
                    lng.Save(Path.ChangeExtension(filename, "lng"));
                    break;

                default:
                    Console.WriteLine($"Unsupported file.");
                    break;
            }
        }
    }
}