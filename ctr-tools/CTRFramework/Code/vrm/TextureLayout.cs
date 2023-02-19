using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

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
        public TextureLayout ParentLayout = null;

        #region properties

        public static readonly int SizeOf = 0x0C;
        public uint offset;

        public Vector2[] uv = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public Vector2[] normuv = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

        public Point Palette;

        public int PalX => Palette.X;
        public int PalY => Palette.Y;

        private ushort packedPalette => (ushort)(Palette.X | Palette.Y << 6);

        public Point Page;

        public int PageX => Page.X;
        public int PageY => Page.Y;

        public BlendingMode blendingMode;
        public BitDepth bpp;

        private ushort packedPageData => (ushort)(PageX | PageY << 4 | (byte)blendingMode << 5 | (byte)bpp << 7);

        public int stretch
        {
            get
            {
                switch (bpp)
                {
                    case BitDepth.Bit4: return 4;
                    case BitDepth.Bit8: return 2;
                    case BitDepth.Bit24: Helpers.Panic(this, PanicType.Error, "24 bits not supported"); return 1;
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

        public Vector2 min
        {
            get
            {
                return new Vector2(
                    (int)Math.Round(Math.Min(Math.Min(uv[0].X, uv[1].X), Math.Min(uv[2].X, uv[3].X))),
                     (int)Math.Round(Math.Min(Math.Min(uv[0].Y, uv[1].Y), Math.Min(uv[2].Y, uv[3].Y)))
                    );
            }
        }

        public Vector2 max
        {
            get
            {
                return new Vector2(
                     (int)Math.Round(Math.Max(Math.Max(uv[0].X, uv[1].X), Math.Max(uv[2].X, uv[3].X))),
                     (int)Math.Round(Math.Max(Math.Max(uv[0].Y, uv[1].Y), Math.Max(uv[2].Y, uv[3].Y)))
                    );
            }
        }

        public int Width => (int)((max.X - min.X) / stretch + 1);

        public int Height => (int)(max.Y - min.Y + 1);

        public int Position => RealY * CtrVrm.region.Width + RealX; //PageY * (CtrVrm.region.Height * CtrVrm.region.Width / 2) + min.Y * CtrVrm.region.Width + PageX * 64 + min.X / stretch;

        public int PalPosition => PalY * CtrVrm.region.Width + PalX * 16;

        public int RealX => (int)(PageX * 64 + min.X / stretch);
        public int RealY => (int)(PageY * 256 + min.Y);

        public Rectangle Frame => new Rectangle(RealX, RealY, Width, Height);

        private string _tag = "";

        //meant to be unique
        public string Tag => _tag == "" ? $"{RealX}_{RealY}_{PalX}_{PalY}_{Width * stretch}_{Height}{(blendingMode != BlendingMode.Standard ? "_" + (byte)blendingMode : "")}" : _tag;
        //public string Tag => _tag == "" ? $"{PalX}_{PalY}_{RealX}_{RealY}_{Width * stretch}_{Height}" : _tag;
        #endregion

        public TextureLayout()
        {
        }

        public TextureLayout(BinaryReaderEx br) => Read(br);

        public static TextureLayout FromReader(BinaryReaderEx br) => new TextureLayout(br);

        public void Read(BinaryReaderEx br)
        {
            //uv.Clear();

            offset = (uint)br.Position;

            uv[0] = br.ReadVector2b();

            ushort buf = br.ReadUInt16();

            Palette = new Point(buf & 0x3F, buf >> 6);

            uv[1] = br.ReadVector2b();

            buf = br.ReadUInt16();

            Page = new Point(buf & 0xF, (buf >> 4) & 1);

            blendingMode = (BlendingMode)((buf >> 5) & 3);
            bpp = (BitDepth)((buf >> 7) & 3);

            byte rest = (byte)((buf >> 9) & 0x7F);

            if (rest != 0)
                Helpers.Panic(this, PanicType.Assume, $"rest = {rest}");

            if (packedPageData != buf)
                Helpers.Panic(this, PanicType.Assume, $"mismatch! {buf} {packedPageData}");

            //        private ushort packedPageData => (ushort)(PageX & PageY << 4 & (byte)blendingMode << 5 & (byte)bpp << 7);

            uv[2] = br.ReadVector2b();
            uv[3] = br.ReadVector2b();

            NormalizeUV();

            if (br.Position - offset != SizeOf)
                Helpers.Panic(this, PanicType.Error, "size mismatch");
        }

        public void NormalizeUV()
        {
            var src = (ParentLayout == null ? this : ParentLayout);

            for (int i = 0; i < 4; i++)
                normuv[i] = new Vector2(
                    (byte)(Helpers.Normalize(src.min.X, src.max.X, uv[i].X) * 255),
                    (byte)(Helpers.Normalize(src.min.Y, src.max.Y, uv[i].Y) * 255)
               );
        }

        public override string ToString()
        {
            return
                $"offset: {offset.ToString("X8")}\r\n\t" +
                $"UV: ({uv[0]}, {uv[1]}, {uv[2]}, {uv[3]})\r\n\t" +
                $"palette: ({PalX}, {PalY})\r\n\t" +
                $"page: ({PageX}, {PageY})\r\n\t" +
                $"bpp: {bpp}\r\n\t" +
                $"blend: {blendingMode}";
        }

        public RotateFlipType DetectRotation()
        {
            return RotateFlipType.None;
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            int offset = (int)bw.Position;

            bw.WriteVector2b(uv[0]);
            bw.Write(packedPalette);
            bw.WriteVector2b(uv[1]);
            bw.Write(packedPageData);
            bw.WriteVector2b(uv[2]);
            bw.WriteVector2b(uv[3]);

            if (bw.Position - offset != SizeOf)
                Helpers.Panic(this, PanicType.Error, "size mismatch");
        }
    }
}