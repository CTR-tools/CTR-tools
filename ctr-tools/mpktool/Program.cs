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

                using (BinaryReader br = new BinaryReader(File.Open("ui_textures.vram", FileMode.Open)))
                {
                    Tim x = CtrVrm.FromReader(br);
                    mpk.Extract(x);
                }

                Console.ReadKey();
            }
        }
    }

}