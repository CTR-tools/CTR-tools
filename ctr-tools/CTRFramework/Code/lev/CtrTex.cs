using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class CtrTex
    {
        public TextureLayout lod0 = new TextureLayout();
        public TextureLayout lod1 = new TextureLayout();
        public TextureLayout lod2 = new TextureLayout();

        public TextureLayout[] hi = new TextureLayout[16];
        public List<TextureLayout> animframes = new List<TextureLayout>(); //this actually has several lods too

        public uint ptrHi;
        public bool isAnimated = false;


        public CtrTex(BinaryReaderEx br, PsxPtr ptr)
        {
            Read(br, ptr);
        }

        public void Read(BinaryReaderEx br, PsxPtr ptr)
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

                        for (int i = 0; i < 16; i++)
                            hi[i] = TextureLayout.FromReader(br);
                    }
            }

        }
    }
}