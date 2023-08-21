using CTRFramework.Shared;
using CTRFramework.Vram;
using System.Collections.Generic;
using System.Diagnostics;
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


        public CtrTex(BinaryReaderEx br, PsxPtr ptr, VisNodeFlags flags) => Read(br, ptr, flags);

        int mode = 0;

        /// <summary>
        /// Rebuilds final montage result of the hi lod texture. Never appears as an instance in the actual game.
        /// </summary>
        /// <param name="vram"></param>
        /// <param name="qb"></param>
        /// <returns>Bitmap instance.</returns>
        public Bitmap GetHiBitmap(Tim vram, QuadBlock qb, Dictionary<string, Bitmap> cache = null)
        {

            //check cache
            if (cache != null)
                if (cache.ContainsKey(lod2.Tag))
                    return cache[lod2.Tag];

            //maybe nothing to process at all?
            if (hi.Count == 0) return null;

            var sw = new Stopwatch();
            sw.Start();

            int width = 0;
            int height = 0;

            //detect the subdiv mode, 4x4, 4x2 or 4x1 based on quadblock vistree flags
            int numhtex = 4;
            int numvtex = 4;

            if (qb.visNodeFlags.HasFlag(VisNodeFlags.Subdiv4x1)) numvtex = 1;
            if (qb.visNodeFlags.HasFlag(VisNodeFlags.Subdiv4x2)) numvtex = 2;

            //detect the max width and height of a single tile
            //rest will be upscaled to this value, creating blurry image, thats the drawback
            for (int i = 0; i < numvtex * numhtex; i++)
            {
                if (hi[i] != null)
                {
                    if (hi[i].stretch * hi[i].Width > width) width = hi[i].stretch * hi[i].Width;
                    if (hi[i].Height > height) height = hi[i].Height;
                }
            }

            //prepare graphic objects
            var bmp = new Bitmap(width * numhtex, height * numvtex);
            var gr = Graphics.FromImage(bmp);

            //set up GDI rendering params
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.CompositingMode = CompositingMode.SourceOver;

            var attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            //loop through all montageing pieces
            for (int i = 0; i < numvtex * 4; i++)
            {
                //a generous null check
                if (hi[i] == null) continue;

                //get texture
                var b = vram.GetTexture(hi[i]);

                //detect tranform mode based on UV order
                var rotatefliptype = hi[i].DetectRotation();

                //convert ctrframework rotatefliptype to GDI rotatefliptype. should optimize this somehow...
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

                //perform flip/rotate of the original bitmap
                b.RotateFlip(proper);

                //draw transformed bitmap on the final canvas
                gr.DrawImage(b, new Rectangle((i % 4) * width, (i / 4) * height, width, height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, attributes);
            }

            sw.Stop();

            Helpers.Panic(this, PanicType.Info, $"texture done in: {sw.Elapsed.TotalMilliseconds}ms");

            if (cache != null)
                cache.Add(lod2.Tag, bmp);
            else
                Helpers.Panic(this, PanicType.Assume, "WTF cache null");

            return bmp;
        }

        public void Read(BinaryReaderEx br, PsxPtr ptr, VisNodeFlags flags)
        {
            int pos = (int)br.Position;

            Helpers.PanicIf(ptr.ExtraBits == HiddenBits.Bit1, this, PanicType.Assume, "!!! ctrtex ptr got extra bits");

            int currentFrame = 0;

            //this hidden bit defines whether it's animated or not
            if (ptr.ExtraBits == HiddenBits.Bit0)
            {
                isAnimated = true;

                //this is pointer to group3
                uint texpos = br.ReadUInt32();

                int numFrames = br.ReadInt16();
                //this can be used to offset the animation frame, used in cove waterfall
                currentFrame = br.ReadInt16();

                //always 0 i suppose
                int test = br.ReadInt32();

                Helpers.PanicIf(test != 0, this, PanicType.Assume, $"!!! test: {test}");

                //array of pointers to each frame texlayout
                uint[] ptrs = br.ReadArrayUInt32(numFrames);

                //reading stuff
                foreach (uint ptrAnimFrame in ptrs)
                {
                    Helpers.Panic(this, PanicType.Warning, $"{ptrAnimFrame.ToString("X8")}");

                    br.Jump(ptrAnimFrame);

                    //read 4 anim lods. check why initial 4 textures are empty. maybe some extra data?
                    animframes.Add(TextureLayout.FromReader(br));
                    animframes.Add(TextureLayout.FromReader(br));
                    animframes.Add(TextureLayout.FromReader(br));
                    animframes.Add(TextureLayout.FromReader(br));
                }

                //now jump to group3
                br.Jump(texpos);
            }

            //Read group3
            lod0 = TextureLayout.FromReader(br);
            lod1 = TextureLayout.FromReader(br);
            lod2 = TextureLayout.FromReader(br);

            if (isAnimated)
                //duh
                lod2 = animframes[currentFrame * 4 + 7];

            //Console.WriteLine(br.Position.ToString("X8"));
            //Console.ReadKey();

            Helpers.Panic(this, PanicType.Debug, lod0.Tag);
            Helpers.Panic(this, PanicType.Debug, lod1.Tag);
            Helpers.Panic(this, PanicType.Debug, lod2.Tag);

            if (CtrScene.ReadHiTex)
            {
                ptrHi = br.ReadUInt32();

                Helpers.Panic(this, PanicType.Debug, "hiptr: " + ptrHi.ToString("X8"));

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