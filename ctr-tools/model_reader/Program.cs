using System;

namespace model_reader2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test");

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