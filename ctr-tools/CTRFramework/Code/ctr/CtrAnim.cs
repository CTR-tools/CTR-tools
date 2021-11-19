using CTRFramework.Shared;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CTRFramework
{
    public class CtrAnim : IReadWrite
    {
        public string Name;
        public short numFrames => (short)frames.Count;
        public short frameSize;
        public int ptrUnk;//??
        public bool duplicateFrames = false;

        List<byte[]> frames = new List<byte[]>();

        public static CtrAnim FromReader(BinaryReaderEx br)
        {
            return new CtrAnim(br);
        }

        public CtrAnim(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            Name = br.ReadStringFixed(16);

            int numFrames = br.ReadInt16(); //bit15 defines amount of render frames in 60fps (duplicated anim frames)

            if ((numFrames & 0x8000) > 0)
                duplicateFrames = true;

            numFrames &= 0x7fff;

            frameSize = br.ReadInt16();
            ptrUnk = br.ReadInt32();

            for (int i = 0; i < numFrames; i++)
                frames.Add(br.ReadBytes(frameSize));
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable)
        {
            if (Name.Length > 16)
                Helpers.Panic(this, PanicType.Warning, $"Name too long, will be truncated: {Name}");

            bw.Write(Name.ToCharArray().Take(16).ToArray());
            bw.Write(numFrames & (duplicateFrames ? 1 : 0 ) << 15);
            bw.Write(frameSize);
            bw.Write(ptrUnk);

            foreach (var frames in frames)
                bw.Write(frames);
        }
    }
}