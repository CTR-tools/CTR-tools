using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class RespawnPoint : IReadWrite
    {
        public Pose Pose { get; set; } = Pose.Zero;
        public short DistanceToFinish;
        public byte index;
        public byte FFunk; //usually FF, 
        public byte prevIndex;
        public byte FFunk2;

        public RespawnPoint Next;

        public RespawnPoint()
        {
        }

        public static RespawnPoint FromReader(BinaryReaderEx br)
        {
            return new RespawnPoint(br);
        }

        public RespawnPoint(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            Pose.Position = br.ReadVector3s(1 / 100f);
            DistanceToFinish = br.ReadInt16();
            index = br.ReadByte();
            FFunk = br.ReadByte();
            prevIndex = br.ReadByte();
            FFunk2 = br.ReadByte();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3s(Pose.Position, 1 / 100f);
            bw.Write(DistanceToFinish);
            bw.Write(index);
            bw.Write(FFunk);
            bw.Write(prevIndex);
            bw.Write(FFunk2);
        }

        public override string ToString() => $"{Pose} index: {index} linked: {prevIndex}";
    }
}