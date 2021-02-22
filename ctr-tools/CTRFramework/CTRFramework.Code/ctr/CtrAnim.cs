using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    class CtrAnim
    {
        string name;
        short numFrames;
        short frameSize;
        int someOffset;//??

        List<byte[]> frames = new List<byte[]>();

        public CtrAnim(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            numFrames = br.ReadInt16(); //negative value defines amount of render frames in 60fps (duplicated anim frames)
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
