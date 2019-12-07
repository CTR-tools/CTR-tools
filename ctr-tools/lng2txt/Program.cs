using CTRFramework;
using System;
using p = lng2txt.Properties.Resources;

namespace lng2txt
{
    class Program
    {
        static void Main(string[] args)
        {
            Greet();

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

        static void Greet()
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                p.project_name,
                p.app_desc,
                p.copyright);
        }
    }
}