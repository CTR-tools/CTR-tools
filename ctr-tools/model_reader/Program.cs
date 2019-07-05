using System;
using System.IO;
using System.Diagnostics;

namespace model_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("model_reader, CTR-Tools");

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

                string ext = Path.GetExtension(args[0]);

                switch (ext)
                {
                    case ".lev":
                        {
                            CTRModel ctrm = new CTRModel(args[0], format);
                            ctrm.Export();

                            /*
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.Arguments = 

                            Process.Start(
                            */

                            break;
                        }
                    case ".ctr":
                        {
                            LODModel ctrm = new LODModel(args[0]);
                            break;
                        }

                }
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