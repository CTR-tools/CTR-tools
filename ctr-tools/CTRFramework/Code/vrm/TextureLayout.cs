using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Numerics;

namespace CTRFramework.Vram
{
    public enum BlendingMode
    {
        Translucent = 0,
        Additive = 1,
        Subtractive = 2,
        Standard = 3
    }

    public enum BitDepth
    {
        Bit4 = 0,
        Bit8 = 1,
        Bit16 = 2,
        Bit24 = 3
    }

    public class TextureLayout : IReadWrite
    {
        #region properties

        public static readonly int SizeOf = 0x0C;

        public uint offset;

        public List<Vector2> uv = new List<Vector2>() { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) };
        public List<Vector2> normuv = new List<Vector2>();


        public Point Palette;

        public int PalX => Palette.X;
        public int PalY => Palette.Y;

        public Point Page;

        public int PageX => Page.X;
        public int PageY => Page.Y;

        public BlendingMode blendingMode;
        public BitDepth bpp;

        public int stretch
        {
            get
            {
                switch (bpp)
                {
                    case BitDepth.Bit4: return 4;
                    case BitDepth.Bit8: return 2;
                    case BitDepth.Bit24: throw new Exception("no supported");
                    case BitDepth.Bit16:
                    default: return 1;
                }
            }
        }

        public int expectedPalSize
        {
            get
            {
                switch (bpp)
                {
                    case BitDepth.Bit4: return 16;
                    case BitDepth.Bit8: return 256;
                    case BitDepth.Bit24:
                    case BitDepth.Bit16:
                    default: return 0;
                }
            }
        }

        public Point min
        {
            get
            {
                return new Point(
                    (int)Math.Round(Math.Min(Math.Min(uv[0].X, uv[1].X), Math.Min(uv[2].X, uv[3].X))),
                     (int)Math.Round(Math.Min(Math.Min(uv[0].Y, uv[1].Y), Math.Min(uv[2].Y, uv[3].Y)))
                    );
            }
        }

        public Point max
        {
            get
            {
                return new Point(
                     (int)Math.Round(Math.Max(Math.Max(uv[0].X, uv[1].X), Math.Max(uv[2].X, uv[3].X))),
                     (int)Math.Round(Math.Max(Math.Max(uv[0].Y, uv[1].Y), Math.Max(uv[2].Y, uv[3].Y)))
                    );
            }
        }

        public int Width => (max.X - min.X) / stretch + 1;

        public int Height => max.Y - min.Y + 1;

        public int Position => RealY * CtrVrm.region.Width + RealX; //PageY * (CtrVrm.region.Height * CtrVrm.region.Width / 2) + min.Y * CtrVrm.region.Width + PageX * 64 + min.X / stretch;

        public int PalPosition => PalY * CtrVrm.region.Width + PalX * 16;

        public int RealX => PageX * 64 + min.X / stretch;
        public int RealY => PageY * 256 + min.Y;

        public Rectangle Frame => new Rectangle(RealX, RealY, Width, Height);

        private ushort packedPageData => (ushort)(PageX & PageY << 4 & (int)blendingMode << 5 & (int)bpp << 7);

        private string _tag = "";

        //meant to be unique
        public string Tag => _tag == "" ? $"{RealX}_{RealY}_{PalX}_{PalY}_{Width * stretch}_{Height}" : _tag;

        #endregion

        public TextureLayout(BinaryReaderEx br)
        {
            Read(br);
        }

        public TextureLayout()
        {
        }

        public static TextureLayout FromReader(BinaryReaderEx br, bool skipcheck = false)
        {
            int test = br.ReadInt32();

            if (test == 0 && !skipcheck)
            {
                Helpers.Panic("TextureLayout", PanicType.Assume, "test failed");
                return null;
            }

            br.BaseStream.Position -= 4;
            return new TextureLayout(br);
        }

        public void Read(BinaryReaderEx br)
        {
            uv.Clear();

            offset = (uint)br.BaseStream.Position;

            uv.Add(br.ReadVector2b());

            ushort buf = br.ReadUInt16();

            Palette = new Point(buf & 0x3F, buf >> 6);

            uv.Add(br.ReadVector2b());

            buf = br.ReadUInt16();

            Page = new Point(buf & 0xF, (buf >> 4) & 1);

            blendingMode = (BlendingMode)((buf >> 5) & 3);
            bpp = (BitDepth)((buf >> 7) & 3);

            byte rest = (byte)((buf >> 9) & 0x7F);

            if (rest != 0)
                Helpers.Panic(this, PanicType.Assume, $"rest = {rest}");

            uv.Add(br.ReadVector2b());
            uv.Add(br.ReadVector2b());

            NormalizeUV();

            if (br.BaseStream.Position - offset != SizeOf)
                throw new Exception("TextureLayout: size mismatch");
        }

        public void NormalizeUV()
        {
            normuv.Clear();

            foreach (var v in uv)
            {
                Vector2 n = new Vector2(0, 0);
                n.X = (byte)(Helpers.Normalize(min.X, max.X, v.X) * 255);
                n.Y = (byte)(Helpers.Normalize(min.Y, max.Y, v.Y) * 255);
                normuv.Add(n);
            }
        }

        public override string ToString()
        {
            return 
                $"offset: {offset.ToString("X8")}\r\n\t" +
                $"UV: ({uv[0].ToString()}, {uv[1].ToString()}, {uv[2].ToString()}, {uv[3].ToString()})\r\n\t" +
                $"palette: ({PalX}, {PalY})\r\n\t" +
                $"page: ({PageX}, {PageY})\r\n\t" +
                $"bpp: {bpp}\r\n\t" +
                $"blend: {blendingMode}";
        }

        public string Dump()
        {
            return $"{PageX}\t{PageY}\t{min}\t{Width * stretch}\t{Height}\t{PalX}\t{PalY}\t{Tag}";
        }

        //this aint actually ever used for obj export...
        public string ToObj(int numVerts = 4)
        {
            StringBuilder sb = new StringBuilder();

            //this is to avoid negative UV and make it clamp friendly
            int[] inds = new int[4] { 0, 1, 2, 3 };

            if (numVerts == 4)
                inds = new int[4] { 2, 3, 0, 1 };

            for (int i = 0; i < numVerts; i++)
            {
                sb.AppendFormat(
                    "vt {0} {1}\r\n",
                    normuv[inds[i]].X / 255f,
                    normuv[inds[i]].Y / 255f
                );
            }

            /*
            foreach (Vector2b v in normuv)
                sb.AppendFormat(
                    "vt {0} {1}\r\n",

                    Math.Round(v.X * 1.0, 3).ToString(),
                    Math.Round(v.Y * 1.0, 3).ToString()
                );
                */
            sb.AppendFormat("\r\nusemtl {0}\r\n", Tag);

            return sb.ToString();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        { 
            bw.WriteVector2b(uv[0]);
            bw.Write(Palette.X & Palette.Y << 6);
            bw.WriteVector2b(uv[1]);
            bw.Write(packedPageData);
            bw.WriteVector2b(uv[2]);
            bw.WriteVector2b(uv[3]);
        }
    }
}