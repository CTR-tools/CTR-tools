using CTRFramework.Lang;
using CTRFramework.Shared;

namespace lng2txt
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

            string fn = Path.GetFileName(filename);
            string ext = Path.GetExtension(filename);

            switch (ext.ToLower())
            {
                case ".lng":
                    var lng = LNG.FromFile(filename);
                    lng.Export(Path.ChangeExtension(filename, "txt"), true);

                    if (fn.ToLower() == "ja.lng")
                    {
                        lng = LNG.FromFile(filename, true);
                        lng.Export(Path.ChangeExtension(filename, "katakana.txt"));
                    }

                    break;

                case ".txt":
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