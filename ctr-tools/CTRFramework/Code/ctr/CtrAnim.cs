using CTRFramework.Shared;
using System.Collections.Generic;

namespace CTRFramework
{
    public class CtrAnim
    {
        public string name;
        public short numFrames;
        public short frameSize;
        public int someOffset;//??

        List<byte[]> frames = new List<byte[]>();

        public CtrAnim(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            // br.BaseStream.Position.ToString("X8");
            // Console.WriteLine(name + " woah");
            // Console.ReadKey();
            numFrames = br.ReadInt16(); //negative value defines amount of render frames in 60fps (duplicated anim frames)
            frameSize = br.ReadInt16();
            someOffset = br.ReadInt32();

            //for (int i = 0; i < (numFrames > 0 ? numFrames : -numFrames; i++)
            //   frames.Add(br.ReadBytes(frameSize));
        }

        public static CtrAnim FromReader(BinaryReaderEx br)
        {
            return new CtrAnim(br);
        }
    }
}
