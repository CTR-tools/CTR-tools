using CTRFramework.Big;
using CTRFramework.Shared;

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

            Console.WriteLine($"Input file: {filename}");
            Console.WriteLine("Current path: " + Environment.CurrentDirectory);

            string bigName = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            string bigPath = Path.GetDirectoryName(filename);

            //gotta revisit this stuff
            //it tries to be smart for the end user when we just call it as "bigtool bigfile.big"

            //if no root provided (as i get it, it happens when there is no .\ or disk C:\, right?)
            if (!Path.IsPathRooted(filename))
            {
                //maybe bigfile is in current terminal directory?
                bigPath = Environment.CurrentDirectory;

                if (!File.Exists(Helpers.PathCombine(bigPath, filename)))
                {
                    //maybe bigfile in tool's root?
                    bigPath = Meta.BasePath;

                    if (!File.Exists(Helpers.PathCombine(bigPath, filename)))
                    {
                        Console.WriteLine("Check filename.");
                        return;
                    }
                }
            }

            try
            {
                var bigfile = BigFile.FromFile(Helpers.PathCombine(bigPath, $"{bigName}{ext}"));

                if (bigfile.Count == 0)
                {
                    Console.WriteLine("No files to process.");
                    return;
                }

                switch (ext.ToLower())
                {
                    case ".big": bigfile.Extract(Helpers.PathCombine(bigPath, bigName)); break;
                    case ".txt": bigfile.Save(Helpers.PathCombine(bigPath, $"{bigName}.big")); break;
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