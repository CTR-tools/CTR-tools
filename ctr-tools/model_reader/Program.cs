using System;

namespace model_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test");

            if (args.Length > 0)
            {

                string format = "obj";

                try
                {
                    if (args[1] == "ply")
                        format = args[1];
                    else
                        format = "obj";
                }
                catch
                {
                    //wow
                }

                CTRModel ctrm = new CTRModel(args[0], format);
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