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


        public CtrTex(BinaryReaderEx br, PsxPtr ptr, VisNodeFlags flags)
        {
            Read(br, ptr, flags);
        }

        int mode = 0;


        public Bitmap GetHiBitmap(Tim vram, QuadBlock qb)
        {
            if (hi.Count == 0)
                return null;

            int width = 0;
            int height = 0;

            int numhtex = 4;
            int numvtex = 4;

            if (qb.visDataFlags.HasFlag(VisNodeFlags.Subdiv4x1)) numvtex = 1;
            if (qb.visDataFlags.HasFlag(VisNodeFlags.Subdiv4x2)) numvtex = 2;

            for (int i = 0; i < numvtex * numhtex; i++)
            {
                if (hi[i] != null)
                {
                    if (hi[i].stretch * hi[i].Width > width) width = hi[i].stretch * hi[i].Width;
                    if (hi[i].Height > height) height = hi[i].Height;
                }
            }

            var bmp = new Bitmap(width * numhtex, height * numvtex);
            var gr = Graphics.FromImage(bmp);

            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //gr.CompositingMode = CompositingMode.SourceCopy;
            gr.CompositingMode = CompositingMode.SourceOver;

            var attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            for (int i = 0; i < numvtex * 4; i++)
            {
                if (hi[i] != null)
                {
                    var rotatefliptype = hi[i].DetectRotation();

                    var b = vram.GetTexture(hi[i]);

                    var proper = System.Drawing.RotateFlipType.RotateNoneFlipNone;

                    switch (rotatefliptype)
                    {
                        case RotateFlipType.None: proper = System.Drawing.RotateFlipType.RotateNoneFlipNone; break;
                        case RotateFlipType.Rotate90: proper = System.Drawing.RotateFlipType.Rotate90FlipNone; break;
                        case RotateFlipType.Rotate180: proper = System.Drawing.RotateFlipType.Rotate180FlipNone; break;
                        case RotateFlipType.Rotate270: proper = System.Drawing.RotateFlipType.Rotate270FlipNone; break;
                        case RotateFlipType.Flip: proper = System.Drawing.RotateFlipType.RotateNoneFlipY; break;
                        case RotateFlipType.FlipRotate90: proper = System.Drawing.RotateFlipType.Rotate90FlipY; break;
                        case RotateFlipType.FlipRotate180: proper = System.Drawing.RotateFlipType.Rotate180FlipY; break;
                        case RotateFlipType.FlipRotate270: proper = System.Drawing.RotateFlipType.Rotate270FlipY; break;
                        default: proper = System.Drawing.RotateFlipType.RotateNoneFlipNone; break;
                    }

                    b.RotateFlip(proper);

                    gr.DrawImage(b, new Rectangle((i % 4) * width, (i / 4) * height, width, height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, attributes);
                    //gr.DrawString($"{rotatefliptype}", font, Brushes.Yellow, (i % 4) * width + 1, (i / 4) * height + 1);
                }
            }

            /*
            //gr.CompositingMode = CompositingMode.SourceOver;
            for (int i = 0; i < numvtex * 4; i++)
            {
                if (hi[i] != null)
                {
                    if (hi[i].uv[0].X == hi[i].min.X && hi[i].uv[0].Y == hi[i].min.Y)
                        mode = 1;

                    //gr.DrawString($"{mode}", font, Brushes.Yellow, (i % 4) * width + 1, (i / 4) * height + 1);
                    mode = 0;
                }
            }
            */

            return bmp;
        }

        Font font = new Font("Courier New", 7);

        public void Read(BinaryReaderEx br, PsxPtr ptr, VisNodeFlags flags)
        {
            int pos = (int)br.Position;

            Helpers.PanicIf(ptr.ExtraBits == HiddenBits.Bit1, this, PanicType.Assume, "!!! ctrtex ptr got extra bits");

            //this apparently defines animated texture, really
            if (ptr.ExtraBits == HiddenBits.Bit0)
            {
                isAnimated = true;

                uint texpos = br.ReadUInt32();
                int numFrames = br.ReadInt16();
                int whatsthat = br.ReadInt16();

                Helpers.PanicIf(whatsthat != 0, this, PanicType.Assume, $"whatsthat is not null! {whatsthat}");
                Helpers.PanicIf(br.ReadUInt32() != 0, this, PanicType.Assume, "not 0!");

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

                        if (flags.HasFlag(VisNodeFlags.Subdiv4x2)) toread = 8;
                        if (flags.HasFlag(VisNodeFlags.Subdiv4x1)) toread = 4;

                        for (int i = 0; i < toread; i++)
                            hi.Add(TextureLayout.FromReader(br));
                    }
            }
        }
    }
}