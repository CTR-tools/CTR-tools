using System;
using System.IO;

namespace bigtool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CTR-Tools\r\nCrash Team Racing BIGFILE Tool\r\n\r\n2018, DCxDemo*\r\n");

            if (args.Length > 0)
            {
                if (Path.GetExtension(args[0]).ToLower() == ".big")
                {
                    BIG big = new BIG(args[0]);
                    big.Export();

                    Console.WriteLine("Done!");
                }
                else
                    if (Path.GetExtension(args[0]).ToLower() == ".txt")
                    {
                        BIG big = new BIG();
                        big.Build(args[0]);
                    }
                    else
                    {
                        Console.WriteLine("Sorry, this doesn't look like a supported file.");
                    }

            }
            else
            {
                Console.WriteLine("Usage:\r\n\tSplit example: bigtool C:\\BIGFILE.BIG\r\n\tMerge example: bigtool C:\\BIGFILE.TXT\r\n");
                Console.WriteLine("Press any key to quit...");

                Console.ReadKey();
            }


        }
    }
}
