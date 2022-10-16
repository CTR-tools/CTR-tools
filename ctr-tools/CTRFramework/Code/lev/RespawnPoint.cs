using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class RespawnPoint : IReadWrite
    {
        public Pose Pose { get; set; } = Pose.Zero;
        public short DistanceToFinish;
        public byte next;
        public byte FFunk; //usually FF, 
        public byte prev;
        public byte FFunk2;

        public RespawnPoint Prev;
        public RespawnPoint Next;

        public RespawnPoint()
        {
        }

        public static RespawnPoint FromReader(BinaryReaderEx br) => new RespawnPoint(br);

        public RespawnPoint(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            Pose.Position = br.ReadVector3s(1 / 100f);
            DistanceToFinish = br.ReadInt16();
            next = br.ReadByte();
            FFunk = br.ReadByte();
            prev = br.ReadByte();
            FFunk2 = br.ReadByte();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3s(Pose.Position, 1 / 100f);
            bw.Write(DistanceToFinish);
            bw.Write(next);
            bw.Write(FFunk);
            bw.Write(prev);
            bw.Write(FFunk2);
        }

        public override string ToString() => $"{Pose} index: {prev} linked: {next}";
    }
}