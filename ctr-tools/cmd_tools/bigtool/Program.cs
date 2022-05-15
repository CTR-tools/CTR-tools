using CTRFramework.Big;
using CTRFramework.Shared;
using System;
using System.IO;

namespace bigtool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                $"CTR-tools: bigtool - {Meta.GetSignature()}",
                "Builds and extracts Crash Team Racing BIG files",
                Meta.GetVersion());

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}",
                    "Usage",
                    "Extract example", "bigtool C:\\BIGFILE.BIG",
                    "Build example", "bigtool C:\\BIGFILE.TXT"
                    );
                return;
            }

            string filename = Path.GetFullPath(args[0]);

            Console.WriteLine("Current path: " + Environment.CurrentDirectory);

            string name = Path.GetFileNameWithoutExtension(filename).ToLower();
            string ext = Path.GetExtension(filename).ToLower();
            string bigpath = Path.GetDirectoryName(filename);

            if (!Path.IsPathRooted(filename))
            {
                bigpath = Environment.CurrentDirectory;

                if (!File.Exists(Path.Combine(bigpath, filename)))
                {
                    bigpath = Meta.BasePath;

                    if (!File.Exists(Path.Combine(bigpath, filename)))
                    {
                        Console.WriteLine("Check filename.");
                        return;
                    }
                }
            }

            string path = Path.Combine(bigpath, name);

            try
            {
                BigFile big = BigFile.FromFile(Path.Combine(bigpath, $"{name}{ext}"));

                if (big.Entries.Count == 0)
                {
                    Console.WriteLine("No files to process.");
                    return;
                }

                switch (ext)
                {
                    case ".big": big.Extract(path); break;
                    case ".txt": big.Save(Path.Combine(bigpath, $"{name}.big")); break;
                    default: Console.WriteLine($"Unsupported file type: {ext}"); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}