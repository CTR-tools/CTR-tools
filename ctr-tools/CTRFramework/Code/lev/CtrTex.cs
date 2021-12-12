using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CTRFramework
{
    public class CtrTex
    {
        public TextureLayout lod0 = new TextureLayout();
        public TextureLayout lod1 = new TextureLayout();
        public TextureLayout lod2 = new TextureLayout();

        //public TextureLayout[] hi = new TextureLayout[16];
        public List<TextureLayout> hi = new List<TextureLayout>();

        public List<TextureLayout> animframes = new List<TextureLayout>(); //this actually has several lods too

        public uint ptrHi;
        public bool isAnimated = false;


        public CtrTex(BinaryReaderEx br, PsxPtr ptr, VisDataFlags flags)
        {
            Read(br, ptr, flags);
        }

        public Bitmap GetHiBitmap(Tim vram, QuadBlock qb)
        {
            if (hi.Count == 0)
                return null;

            int width = 0;
            int height = 0;

            int numhtex = 4;
            int numvtex = 4;

            if (qb.visDataFlags.HasFlag(VisDataFlags.Subdiv4x1)) numvtex = 1;
            if (qb.visDataFlags.HasFlag(VisDataFlags.Subdiv4x2)) numvtex = 2;

            for (int i = 0; i < numvtex * numhtex; i++)
            {
                if (hi[i] != null)
                {
                    if (hi[i].stretch * hi[0].Width > width) width = hi[0].stretch * hi[0].Width;
                    if (hi[i].Height > height) height = hi[0].Height;
                }
            }

            Bitmap bmp = new Bitmap(width * numhtex, height * numvtex);
            Graphics gr = Graphics.FromImage(bmp);

            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.NearestNeighbor;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.CompositingMode = CompositingMode.SourceCopy;

            var attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            for (int i = 0; i < numvtex * 4; i++)
            {
                if (hi[i] != null)
                {
                    Bitmap b = vram.GetTexture(hi[i]);
                    gr.DrawImage(b, new Rectangle((i % 4) * width, (i / 4) * height, width, height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return bmp;

        }


        public void Read(BinaryReaderEx br, PsxPtr ptr, VisDataFlags flags)
        {
            int pos = (int)br.Position;

            if (ptr.ExtraBits == HiddenBits.Bit1)
            {
                Console.WriteLine("!!!");
                //Console.ReadKey();
            }

            //this apparently defines animated texture, really
            if (ptr.ExtraBits == HiddenBits.Bit0)
            {
                isAnimated = true;

                uint texpos = br.ReadUInt32();
                int numFrames = br.ReadInt16();
                int whatsthat = br.ReadInt16();

                if (whatsthat != 0)
                    Helpers.Panic(this, PanicType.Assume, $"whatsthat is not null! {whatsthat}");

                if (br.ReadUInt32() != 0)
                    Helpers.Panic(this, PanicType.Assume, "not 0!");

                uint[] ptrs = br.ReadArrayUInt32(numFrames);

                foreach (uint ptrAnimFrame in ptrs)
                {
                    br.Jump(ptrAnimFrame);
                    animframes.Add(TextureLayout.FromReader(br));
                }

                br.Jump(texpos);
            }

            lod0 = TextureLayout.FromReader(br);
            lod1 = TextureLayout.FromReader(br);
            lod2 = TextureLayout.FromReader(br);

            //Console.WriteLine(br.Position.ToString("X8"));
            //Console.ReadKey();

            Helpers.Panic(this, PanicType.Debug, lod0.Tag);
            Helpers.Panic(this, PanicType.Debug, lod1.Tag);
            Helpers.Panic(this, PanicType.Debug, lod2.Tag);

            if (CtrScene.ReadHiTex)
            {
                ptrHi = br.ReadUInt32();

                Helpers.Panic(this, PanicType.Debug, "hiptr: " + ptrHi.ToString("X8"));

                if (CtrScene.ReadHiTex)
                    //loosely assume we got a valid pointer
                    if (ptrHi > 0x30000 && ptrHi < 0xB0000)
                    {
                        br.Jump(ptrHi);

                        int toread = 16;

                        if (flags.HasFlag(VisDataFlags.Subdiv4x2)) toread = 8;
                        if (flags.HasFlag(VisDataFlags.Subdiv4x1)) toread = 4;

                        for (int i = 0; i < toread; i++)
                            hi.Add(TextureLayout.FromReader(br));
                    }
            }

        }
    }
}