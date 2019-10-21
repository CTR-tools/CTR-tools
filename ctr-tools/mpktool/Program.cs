using CTRFramework;
using CTRFramework.Shared;
using System;
using System.IO;

namespace mpktool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ModelPack mpk = new ModelPack(args[0]);

                using (BinaryReaderEx br = new BinaryReaderEx(File.Open("ui_textures.vram", FileMode.Open)))
                {
                    Tim x = CtrVrm.FromReader(br);
                    mpk.Extract(x);
                }

                Console.ReadKey();
            }
        }
    }

}