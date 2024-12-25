using CTRFramework.Shared;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CTRFramework.Vram
{
    public class CtrVrm : IRead
    {
        #region [VRAM areas]
        // these rectangles describe certain hardcoded regions of vram 

        public static Rectangle FullVramRegion = new Rectangle(0, 0, 1024, 512);

        // level regions
        public static Rectangle UpperLevelRegion => new Rectangle(512, 0, 384, 256);
        public static Rectangle LowerLevelRegion => new Rectangle(512, 256, 512, 256);

        // shared.vrm regions (NTSC-U!)
        public static Rectangle MainSharedRegion => new Rectangle(896, 0, 128, 256);

        // this stripe is different in PAL
        public static Rectangle AdditionalSharedRegion => new Rectangle(0, 216, 512, 48);

        #endregion

        public List<Tim> Tims = new List<Tim>();

        public CtrVrm() { }

        public CtrVrm(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            Tims.Clear();

            // if it starts with 0x20 magic value, it's an endless stream of tims, however, actual game is hardcoded for 2 pages max
            if (br.ReadInt32() == 0x20)
            {
                for (int i = 0; i < 2; i++)
                {
                    int pos = (int)br.BaseStream.Position;
                    int size = br.ReadInt32(); //data size
                    Tims.Add(Tim.FromReader(br));

                    if (br.BaseStream.Position - pos != size)
                        Helpers.Panic(this, PanicType.Error, "VRAM: tim size mismatch.");
                }
            }
            else //it's supposed to be just a single tim file
            {
                br.Jump(0);
                Tims.Add(Tim.FromReader(br));
            }
        }

        public static CtrVrm FromReader(BinaryReaderEx br) => new CtrVrm(br);

        public static CtrVrm FromFile(string filename)
        {
            //return empty vram if no file found
            if (!File.Exists(filename))
            {
                Helpers.Panic("CtrVram", PanicType.Warning, "Missing VRAM file, return empty.");
                return new CtrVrm();
            }

            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        /// <summary>
        /// Generate PSX VRAM canvas from a list of TIM images.
        /// </summary>
        /// <returns>16 bit TIM image.</returns>
        public Tim GetVram()
        {
            // create a new TIM image to represent the entire VRAM
            var buffer = new Tim(FullVramRegion, BitDepth.Bit16);

            // draw all pages to the canvas
            // CTR only uses 2 pages in general, but we can handle any amount of pages
            foreach (var tim in Tims)
                buffer.DrawTim(tim);

            return buffer;
        }

        public override string ToString()
        {
            string result = "";

            foreach (var tim in Tims)
                result += tim.ToString() + "\r\n";

            return result;
        }
    }
}