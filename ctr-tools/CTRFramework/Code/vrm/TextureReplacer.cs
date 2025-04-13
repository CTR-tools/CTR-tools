using CTRFramework.Shared;
using System;
using System.Collections.Generic;
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

        // it's only used once, so can keep it this way
        public bool IsSharedVramFile => Path.GetFileName(vramPath).ToUpper() == "SHARED.VRM";

        /// <summary>
        /// Validates replacer context by checking the provided folders and VRAM file. 
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (textures is null)
            {
                Helpers.Panic(this, PanicType.Warning, "No texture to replace!");
                return false;
            }

            if (String.IsNullOrEmpty(vramPath) || !File.Exists(vramPath))
            {
                Helpers.Panic(this, PanicType.Warning, "No VRAM file provided!");
                return false;
            }

            if (String.IsNullOrEmpty(newtexPath) || !Directory.Exists(newtexPath))
            {
                Helpers.Panic(this, PanicType.Warning, "No newtex folder provided!");
                return false;
            }

            // only proceed to vram parsing if all paths are fine
            if (vrm is null)
                vrm = CtrVrm.FromFile(vramPath).GetVram();

            return true;
        }
    }

    public class TextureReplacer
    {
        public TextureReplacerContext Context;

        /// <summary>
        /// A guarded "replace textures" method. Validates the context and tries to execute the replacement code.
        /// </summary>
        /// <returns>[TextureReplacerResult] Replacement status, GeneralError in case of exception.</returns>
        public TextureReplacerResult TryReplace()
        {
            // validating context
            if (!Context.Validate())
                return TextureReplacerResult.MissingContent;

            try
            {
                // actual replacement method
                return Replace();
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, PanicType.Error, $"Failed to replace textures: {ex.Message}\r\n{ex.ToString()}");
                return TextureReplacerResult.GeneralError;
            }
        }

        /// <summary>
        /// A private method to replace textures, doesnt check for errors.
        /// </summary>
        /// <returns>[TextureReplacerResult] Replacement status.</returns>
        private TextureReplacerResult Replace()
        {
            string rootPath = Path.GetDirectoryName(Context.vramPath);
            string dumpfirstpath = Helpers.PathCombine(rootPath, "debug_vram_first.bmp");
            string dumplatestpath = Helpers.PathCombine(rootPath, "debug_vram_latest.bmp");

            // if instructed to dump vram as image
            if (Context.dumpVram)
                // only dump if bitmap file doesn't exist yet, so can compare latest changes to the initial dump
                if (!File.Exists(dumpfirstpath))
                    Context.vrm.SaveBMP(dumpfirstpath, BMPHeader.GrayScalePalette(16));


            // read PNG images from newtex folder
            var replaceCandidates = Directory.GetFiles(Context.newtexPath, "*.png");

            // early exit if no textures found
            if (replaceCandidates.Length == 0)
            {
                Helpers.Panic(this, PanicType.Warning, "No textures to replace");
                return TextureReplacerResult.MissingContent;
            }

            // iterate over all textures found
            foreach (var png in replaceCandidates)
            {
                // treat texture name as tag generated from texturelayout
                // split here is for modelpack icon to keep the icon name along with its tag
                string tag = Path.GetFileNameWithoutExtension(png).Split(';')[0];

                // if texture not found, continue
                if (!Context.textures.ContainsKey(tag))
                {
                    Helpers.Panic(Context.vrm, PanicType.Warning, $"replacer texture not found in vram: {tag}");
                    continue;
                }

                Helpers.Panic(Context.vrm, PanicType.Info, $"Replacing {tag}... ");

                // if match found, read the original texture
                var newtex = Context.vrm.GetTimTexture(Context.textures[tag]);

                // then load png data to existing tim, to keep the original tim header data
                newtex.LoadDataFromBitmap(png);

                // finally draw modified tim back to the framebuffer tim
                Context.vrm.DrawTim(newtex);
            }

            // dump post replacement vram state if instructed to
            if (Context.dumpVram)
                Context.vrm.SaveBMP(dumplatestpath, BMPHeader.GrayScalePalette(16));


            // now onto the saving the updated vram...

            // !!! ok so warning here, this part is hardcoded for level files that split vram in 2 parts !!!
            // !!! proper implementation must keep ctrvrm frames and reuse that to properly rebuild correct layout !!!

            // get 2 frames from new vram
            var tims = new List<Tim>() {
                // upper half
                Context.vrm.GetTrueColorTexture(CtrVrm.UpperLevelRegion),
                // lower half
                Context.vrm.GetTrueColorTexture(CtrVrm.LowerLevelRegion)
            };

            // !!! special case for shared.vrm, a hack cause of the warning above !!!
            // !!! hardcoded ntsc-u regions, pal has different layout !!!

            if (Context.IsSharedVramFile)
            {
                tims = new List<Tim>() {
                    Context.vrm.GetTrueColorTexture(CtrVrm.MainSharedRegion),
                    Context.vrm.GetTrueColorTexture(CtrVrm.AdditionalSharedRegion),
                };
            }

            // backup existing file
            Helpers.BackupFile(Context.vramPath);

            // should move this to ctrvram i guess
            using (var bw = new BinaryWriterEx(File.Create(Context.vramPath)))
            {
                bw.Write((int)0x20);

                foreach (var tim in tims)
                {
                    bw.Write(tim.Filesize);
                    tim.Write(bw);
                }

                bw.Write((int)0);

                bw.Truncate();
            }

            //ctr.GetTrueColorTexture(512, 0, 384, 256).Write(Helpers.PathCombine(Path.GetDirectoryName(pathFolder.Text), "x01.tim"));
            //ctr.GetTrueColorTexture(512, 256, 512, 256).Write(Helpers.PathCombine(Path.GetDirectoryName(pathFolder.Text), "x02.tim"));

            return TextureReplacerResult.OK;
        }


        private static string MagicLayoutHeader = "_layouts";

        /// <summary>
        /// Creates a binary list of texture layouts. Used by replacer to avoid additional lev/mpk parsing.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="layouts"></param>
        public static void DumpTextureLayoutList(string filename, List<TextureLayout> layouts)
        {
            using (var bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                //magick!
                bw.Write(MagicLayoutHeader.ToCharArray());

                bw.Write((uint)layouts.Count);

                foreach (var layout in layouts)
                    layout.Write(bw);
            }
        }

        /// <summary>
        /// Loads binary level layouts list from file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Dictionary<string, TextureLayout> LoadLayoutList(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                if (new string(br.ReadChars(8)) != MagicLayoutHeader)
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
