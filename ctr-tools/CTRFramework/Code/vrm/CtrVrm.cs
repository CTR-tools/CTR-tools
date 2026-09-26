using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CTRFramework.Vram
{
    [Flags]
    public enum VramRect
    {
        Empty = 0,
        Full = 1 << 0,
        LowerLevel = 1 << 1,
        UpperLevel = 1 << 2,
        MainShared = 1 << 3,
        ExtraShared = 1 << 4,
        MainSharedPal = 1 << 5,
        ExtraSharedPal = 1 << 6
    }

    public class CtrVrm : IRead
    {
        public const int FullSize = 1024 * 2 * 512;

        // A hardcoded list of common CTR cram
        public static Dictionary<VramRect, Rectangle> RegionsList = new Dictionary<VramRect, Rectangle>()
        {
            { VramRect.Empty,            Rectangle.Empty },
            { VramRect.Full,             new Rectangle(0, 0, 1024, 512) },
            { VramRect.UpperLevel,       new Rectangle(512, 0, 384, 256) },
            { VramRect.LowerLevel,       new Rectangle(512, 256, 512, 256) },
            { VramRect.MainShared,       new Rectangle(896, 0, 128, 256) },
            { VramRect.ExtraShared,      new Rectangle(0, 216, 512, 48) },

            // TODO -- match PAL shared regions, these are currently copies of NTSC-U !!!
            { VramRect.MainSharedPal,    new Rectangle(896, 0, 128, 256) },
            { VramRect.ExtraSharedPal,   new Rectangle(0, 216, 512, 48) }
        };

        public List<Tim> Entries = new List<Tim>();

        public CtrVrm() { }

        public CtrVrm(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            Entries.Clear();

            // if it starts with 0x20 magic value, it's an endless stream of tims, however, actual game is hardcoded for 2 pages max
            if (br.ReadInt32() == 0x20)
            {
                for (int i = 0; i < 2; i++)
                {
                    int size = br.ReadInt32(); //data size
                    // read pos after size, dummy
                    int pos = (int)br.BaseStream.Position;
                    Entries.Add(Tim.FromReader(br));

                    if (br.BaseStream.Position - pos != size)
                        Helpers.PanicError(this, $"VRAM: tim size mismatch. Expected: {size}, actual: {br.BaseStream.Position - pos}");
                }
            }
            else //it's supposed to be just a single tim file
            {
                br.Jump(0);
                Entries.Add(Tim.FromReader(br));
            }
        }

        public static CtrVrm FromReader(BinaryReaderEx br) => new CtrVrm(br);

        /// <summary>
        /// Loads ND .VRM file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
        public Tim GetFullVram()
        {
            // create a new TIM image to represent the entire VRAM
            var buffer = new Tim(RegionsList[VramRect.Full], BitDepth.Bit16);

            Helpers.PanicIf(Entries.Count > 2, this, PanicType.Warning, "Original game only ever lods 2 TIMs. You have {Entries.Count}.");

            // draw all pages to the canvas
            // CTR only uses 2 pages in general, but we can handle any amount of pages
            foreach (var tim in Entries)
            {
                buffer.DrawTim(tim);

                foreach (var reg in RegionsList)
                {
                    if (reg.Value == tim.region)
                    {
                        Helpers.PanicDebug(this, $"VRAM file got {reg.Key}.");
                        break;
                    }
                }
            }




            return buffer;
        }

        /// <summary>
        /// Construct a CtrVrm using a full VRAM image, included regions are defined by include flags.
        /// </summary>
        /// <param name="vram"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static CtrVrm FromFullVram(Tim vram, VramRect include)
        {
            // TODO -- check if this is correct
            if (vram.datasize != FullSize)
            {
                Helpers.PanicError("CtrVrm", "Passed TIM is not a full VRAM image.");
                return new CtrVrm();
            }

            var ctr = new CtrVrm();

            foreach (var reg in RegionsList)
                if (include.HasFlag(reg.Key))
                    ctr.Entries.Add(vram.GetTrueColorTexture(reg.Value));

            return ctr;
        }

        /// <summary>
        /// Write all existing 
        /// </summary>
        /// <param name="filename"></param>
        public void Write(string filename)
        {
            using (var bw = new BinaryWriterEx(File.Create(filename)))
            {
                bw.Write((uint)0x20);

                foreach (var tim in Entries)
                {
                    bw.Write(tim.Filesize);
                    tim.Write(bw);
                }

                bw.Write((int)0);

                // check if its required, it seems like create truncates, but openwrite doesnt.
                // bw.Truncate();
            }
        }

        public override string ToString()
        {
            string result = "";

            foreach (var tim in Entries)
                result += tim.ToString() + "\r\n";

            return result;
        }
    }
}