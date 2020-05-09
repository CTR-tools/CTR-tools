using CTRFramework;
using CTRFramework.Vram;


namespace mpktool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ModelPack mpk = new ModelPack(args[0]);
                mpk.Extract(CtrVrm.FromFile("ui_textures.vram"));

                //Console.ReadKey();
            }
        }
    }
}