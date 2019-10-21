using System;

namespace lng2txt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Crash Team Racing LNG2TXT");

            if (args.Length > 0)
            {
                LNG lng = new LNG(args[0]);
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("No filename!");
            }
        }
    }
}