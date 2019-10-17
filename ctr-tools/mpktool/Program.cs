using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CTRFramework;

namespace mpktool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ModelPack mpk = new ModelPack(args[0]);
                Console.ReadKey();
            }
        }
    }

}