using CTRFramework.Lang;
using CTRFramework.Shared;
using System;
using System.IO;

namespace lng2txt
{
    class Program
    {
        static void Main(string[] args)
        {
            LNG lng;

            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-Tools: lng2txt - {Meta.GetSignature()}",
                "Converts LNG localization files to TXT and back.",
                Meta.GetVersion());

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}",
                    "Usage",
                    "Convert to text", "lng2txt c:\\en.lng",
                    "Convert to .lng", "lng2txt c:\\en.txt"
                    );
                return;
            }

            Console.WriteLine("Current path: " + Environment.CurrentDirectory);

            foreach (string filename in args)
            {
                if (File.Exists(filename))
                {
                    string ext = Path.GetExtension(filename).ToLower();

                    try
                    {
                        switch (ext)
                        {
                            case ".lng":
                                lng = LNG.FromFile(filename);
                                lng.Export(Path.ChangeExtension(filename, "txt"));
                                continue;

                            case ".txt":
                                lng = LNG.FromText(File.ReadAllLines(filename));
                                lng.Save(Path.ChangeExtension(filename, "lng"));
                                continue;

                            default:
                                Console.WriteLine("Unsupported file.");
                                continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        continue;
                    }
                }
            }

            Console.WriteLine("Done.");
        }
    }
}