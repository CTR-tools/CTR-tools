using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CTRFramework
{
    public class CtrAnim
    {
        public string Name;
        public short numFrames => (short)Frames.Count;
        public short frameSize = 0;
        public PsxPtr ptrDeltas = PsxPtr.Zero;   //assumed to be the pointer to deltas (check cb2 specs)
        public bool duplicateFrames = false;

        public bool IsCompressed => ptrDeltas != PsxPtr.Zero;

        public List<CtrFrame> Frames = new List<CtrFrame>();

        public List<CtrDelta> deltas = new List<CtrDelta>();

        public static CtrAnim FromReader(BinaryReaderEx br, int numVerts)
        {
            return new CtrAnim(br, numVerts);
        }

        public CtrAnim(BinaryReaderEx br, int numVerts)
        {
            Read(br, numVerts);
        }

        public void Read(BinaryReaderEx br, int numVerts)
        {
            Name = br.ReadStringFixed(16);

            int numFrames = br.ReadInt16(); //bit15 defines amount of render frames in 60fps (duplicated anim frames)

            if ((numFrames & 0x8000) > 0)
                duplicateFrames = true;

            numFrames &= 0x7fff;

            if (duplicateFrames)
            {
                numFrames /= 2;
                numFrames++;
            }

            frameSize = br.ReadInt16();
            ptrDeltas = PsxPtr.FromReader(br);

            Console.WriteLine($"{Name} [{IsCompressed}]: {ptrDeltas}");

            int ptrFrames = (int)br.BaseStream.Position;

            for (int i = 0; i < numFrames; i++)
            {
                br.Jump(ptrFrames + frameSize * i);
                Console.WriteLine("frame " + i + " " + br.HexPos());
                Frames.Add(CtrFrame.FromReader(br, numVerts));
            }

            if (IsCompressed)
            {
                br.Jump(ptrDeltas);
                for (int i = 0; i < numVerts; i++)
                    deltas.Add(CtrDelta.FromReader(br));
            }
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable)
        {
            if (Name.Length > 16)
                Helpers.Panic(this, PanicType.Warning, $"Name too long, will be truncated: {Name}");

            bw.Write(Name.ToCharArray().Take(16).ToArray());
            bw.Write(numFrames & (duplicateFrames ? 1 : 0) << 15);
            bw.Write(frameSize);
            ptrDeltas.Write(bw);

            throw new NotImplementedException("implement ctrframe write");

            //foreach (var frames in Frames)
            //    bw.Write(frames);
        }
    }
}