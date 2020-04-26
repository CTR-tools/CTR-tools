using System;
using System.IO;
using CTRFramework;
using CTRFramework.Shared;

namespace bigtool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                "CTR-tools: BigTool",
                "Builds and extracts Crash Team Racing BIG files",
                "CTRFramework " + Meta.GetVersion() );

            if (args.Length > 0)
            {
                string ext = Path.GetExtension(args[0]).ToLower();

                BIG2 big = new BIG2(args[0]);

                switch (ext)
                {
                    case ".big": big.Export(".\\bigfile"); break;
                    //case ".txt": Build(); break;
                    default: Console.WriteLine("{0}: {1}", "Unsupported file", ext); break;
                }

                return;
            }

            Console.WriteLine(
               "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}",
               "Usage",
               "Split example", "bigtool C:\\BIGFILE.BIG",
               "Merge example", "bigtool C:\\BIGFILE.TXT"
            );
        }
    }
}
