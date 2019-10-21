using System;
using System.IO;
using p = bigtool.Properties.Resources;

namespace bigtool
{
    class Program
    {

        static void Main(string[] args)
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            Greet();

            if (args.Length > 0)
            {
                string ext = Path.GetExtension(args[0]).ToLower();

                switch (ext)
                {
                    case ".big": Extract(args[0]); break;
                    case ".txt": Build(args[0]); break;
                    default: Console.WriteLine("{0}: {1}", p.file_not_supported, ext); break;
                }

                //Console.ReadKey();
                return;
            }

            Info();
            //Console.ReadKey();
        }


        static void Extract(string path)
        {
            BIG big = new BIG(path);
            big.Export();
            Console.WriteLine(p.done);
        }


        static void Build(string path)
        {
            BIG big = new BIG();
            big.Build(path);
            Console.WriteLine(p.done);
        }


        static void Greet()
        {
            Console.WriteLine(
                "{0}\r\n{1}\r\n\r\n{2}\r\n",
                p.project_name,
                p.app_desc,
                p.copyright);
        }


        static void Info()
        {
            Console.WriteLine(
               "{0}:\r\n\t{1}: {2}\r\n\t{3}: {4}\r\n\r\n{5}",
               p.usage,
               p.split_example, "bigtool C:\\BIGFILE.BIG",
               p.merge_example, "bigtool C:\\BIGFILE.TXT",
               p.confirm_quit
            );
        }
    }
}
