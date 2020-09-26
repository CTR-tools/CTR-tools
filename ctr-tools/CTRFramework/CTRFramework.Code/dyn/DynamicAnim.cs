using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    class DynamicAnim
    {
        string name;
        byte numFrames;
        byte unk1;
        short frameSize;
        int someOffset;//??

        List<byte[]> frames = new List<byte[]>();

        public DynamicAnim(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            numFrames = br.ReadByte();
            unk1 = br.ReadByte();
            frameSize = br.ReadInt16();
            someOffset = br.ReadInt32();

            for (int i = 0; i < numFrames; i++)
            {
                Console.WriteLine(name + " frame " + i + "/" + numFrames + ": " + br.BaseStream.Position.ToString("X8") + 4);
                //Console.ReadKey();
                frames.Add(br.ReadBytes(frameSize));
            }

            Console.WriteLine("anim -> " + name + " " + numFrames + " frames " + frameSize + " framesize");
        }
    }
}
