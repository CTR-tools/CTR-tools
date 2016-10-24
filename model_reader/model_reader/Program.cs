using System;

namespace big_splitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CTR Model Reader");

            if (args.Length > 0)
            {
                CTRModel ctrm = new CTRModel(args[0]);
                ctrm.Export();

                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("No filename!");
            }

            var name = Console.ReadLine();
        }
    }
}