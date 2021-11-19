using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class CtrTex
    {
        public int numHighTex = 0;

        public TextureLayout[] midlods = new TextureLayout[3];
        public TextureLayout[] hi = new TextureLayout[16];
        public List<TextureLayout> animframes = new List<TextureLayout>(); //this actually has several lods too

        public uint ptrHi;
        public bool isAnimated = false;


        public CtrTex(BinaryReaderEx br, PsxPtr ptr, int numhi)
        {
            numHighTex = numhi;
            Read(br, ptr);
        }

        public void Read(BinaryReaderEx br, PsxPtr ptr)
        {
            int pos = (int)br.BaseStream.Position;

            if (ptr.ExtraBits == HiddenBits.Bit1)
            {
                Console.WriteLine("!!!");
                Console.ReadKey();
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

            for (int i = 0; i < 3; i++)
                midlods[i] = TextureLayout.FromReader(br);

            //Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            //Console.ReadKey();


            Helpers.Panic(this, PanicType.Debug, midlods[2].Tag);
            Helpers.Panic(this, PanicType.Debug, "" + numHighTex);


            if (numHighTex > 0 & Scene.ReadHiTex)
            {
                ptrHi = br.ReadUInt32();

                Helpers.Panic(this, PanicType.Debug, "hiptr: " + ptrHi.ToString("X8"));

                if (Scene.ReadHiTex)
                    //loosely assume we got a valid pointer
                    if (ptrHi > 0x30000 && ptrHi < 0xB0000)
                    {
                        br.Jump(ptrHi);

                        for (int i = 0; i < numHighTex; i++)
                            hi[i] = TextureLayout.FromReader(br);
                    }
            }

        }
    }
}