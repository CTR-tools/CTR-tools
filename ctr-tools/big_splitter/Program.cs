using System;

namespace big_splitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CTR-Tools - Crash Team Racing BIGFILE Extractor by DCxDemo*\r\n");

            if (args.Length > 0)
            {
                BIG big = new BIG(args[0]);
                big.Export();

                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("Usage:\r\n\tSplit: big_splitter <path to bigfile.big>\r\n\tMerge: (to be implemented yet)\r\n");
                Console.WriteLine("Press any key to quit...");
            }

            Console.ReadKey();
        }
    }
}
