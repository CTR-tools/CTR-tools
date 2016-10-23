using System;

namespace big_splitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BIGFILE.BIG Extractor");

            if (args.Length > 0)
            {
                BIG big = new BIG(args[0]);
                big.Export();

                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("No filename!");
            }
        }
    }
}
