using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CTRFramework.Vram
{
    public enum TextureReplacerResult
    {
        OK,
        MissingContent,
        GeneralError
    }

    public class TextureReplacerContext
    {
        public Dictionary<string, TextureLayout> textures;
        public Tim vrm;

        public string vramPath;
        public string newtexPath;
        public bool dumpVram;

        public bool Validate()
        {
            if (textures is null)
            {
                Helpers.Panic(this, PanicType.Warning, "No texture to replace!");
                return false;
            }

            if (!File.Exists(vramPath))
            {
                Helpers.Panic(this, PanicType.Warning, "No VRAM file provided!");
                return false;
            }

            if (!Directory.Exists(newtexPath))
            {
                Helpers.Panic(this, PanicType.Warning, "No newtex folder provided!");
                return false;
            }

            if (vrm is null)
                vrm = CtrVrm.FromFile(vramPath).GetVram();

            return true;
        }
    }

    public class TextureReplacer
    {
        public TextureReplacerContext Context;

        public TextureReplacerResult TryReplace()
        {
            //validating context
            if (!Context.Validate())
                return TextureReplacerResult.MissingContent;

            try
            {
                return Replace();
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, PanicType.Error, $"Failed to replace textures: {ex.Message}\r\n{ex.ToString()}");
                return TextureReplacerResult.GeneralError;
            }
        }

        private TextureReplacerResult Replace()
        {

            string rootPath = Path.GetDirectoryName(Context.vramPath);
            string dumpfirstpath = Helpers.PathCombine(rootPath, "debug_vram_first.bmp");
            string dumplatestpath = Helpers.PathCombine(rootPath, "debug_vram_latest.bmp");

            //dumping vram before the change
            if (Context.dumpVram)
                //only dump if file doesn't exist, so can compare initial dump over later changes
                if (!File.Exists(dumpfirstpath))
                    Context.vrm.SaveBMP(dumpfirstpath, BMPHeader.GrayScalePalette(16));


            //read PNG images
            var replaceCandidates = Directory.GetFiles(Context.newtexPath, "*.png");

            if (replaceCandidates.Length == 0)
            {
                Helpers.Panic(this, PanicType.Warning, "No textures to replace");
                return TextureReplacerResult.MissingContent;
            }

            foreach (var png in replaceCandidates)
            {
                //treat texture name as tag generated from texturelayout
                //split here is for modelpack icon to keep icon name along with tag
                string tag = Path.GetFileNameWithoutExtension(png).Split(';')[0];

                //if texture not found, continue
                if (!Context.textures.ContainsKey(tag))
                {
                    Helpers.Panic(Context.vrm, PanicType.Warning, $"replacer texture not found in vram: {tag}");
                    continue;
                }

                Helpers.Panic(Context.vrm, PanicType.Info, $"Replacing {tag}... ");

                //if match found, read the original texture
                var newtex = Context.vrm.GetTimTexture(Context.textures[tag]);

                //then load png data, keeping the original tim location values
                newtex.LoadDataFromBitmap(png);

                //finally draw modified tim back to framebuffer tim
                Context.vrm.DrawTim(newtex);
            }

            //dump post replacement vram state
            if (Context.dumpVram)
                Context.vrm.SaveBMP(dumplatestpath, BMPHeader.GrayScalePalette(16));

            //now onto saving vram...

            // !!! ok so warning here, this part is hardcoded for level files that split vram in 2 parts
            // !!! proper implementation must keep ctrvrm frames and reuse that to properly rebuild correct layout

            //get 2 frames from new vram
            var tims = new List<Tim>() {
                Context.vrm.GetTrueColorTexture(new Rectangle(512, 0, 384, 256)),
                Context.vrm.GetTrueColorTexture(new Rectangle(512, 256, 512, 256))
            };

            // !!! special case for shared.vrm, a hack cause of the warning above
            // !!! hardcoded ntsc-u regions, pal has different layout !!!

            if (Path.GetFileName(Context.vramPath).ToUpper() == "SHARED.VRM")
            {
                tims = new List<Tim>() {
                    Context.vrm.GetTrueColorTexture(new Rectangle(896, 0, 128, 256)),
                    Context.vrm.GetTrueColorTexture(new Rectangle(0, 216, 512, 48)),
                };
            }

            //backup existing file
            Helpers.BackupFile(Context.vramPath);
            File.Delete(Context.vramPath);

            //should move this to ctrvram i guess
            using (var bw = new BinaryWriterEx(File.Create(Context.vramPath)))
            {
                bw.Write((int)0x20);

                foreach (var tim in tims)
                {
                    bw.Write(tim.Filesize);
                    tim.Write(bw);
                }

                bw.Write((int)0);
            }

            //ctr.GetTrueColorTexture(512, 0, 384, 256).Write(Helpers.PathCombine(Path.GetDirectoryName(pathFolder.Text), "x01.tim"));
            //ctr.GetTrueColorTexture(512, 256, 512, 256).Write(Helpers.PathCombine(Path.GetDirectoryName(pathFolder.Text), "x02.tim"));

            return TextureReplacerResult.OK;
        }


        public static Dictionary<string, TextureLayout> LoadLayoutList(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                if (new string(br.ReadChars(8)) != "_layouts")
                    return null;

                var layouts = new Dictionary<string, TextureLayout>();

                int numLayouts = br.ReadInt32();

                for (int i = 0; i < numLayouts; i++)
                {
                    var layout = TextureLayout.FromReader(br);
                    layouts.Add(layout.Tag, layout);
                }

                return layouts;
            }
        }
    }
}
