using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class RespawnPoint : IReadWrite
    {
        public static readonly int SizeOf = 0x0C;

        public Pose Pose { get; set; } = Pose.Zero;
        public short DistanceToFinish;

        // likely FF is "no link"
        public byte next;
        public byte left;
        public byte prev;
        public byte right;


        public RespawnPoint Prev;
        public RespawnPoint Next;

        public RespawnPoint()
        {
        }

        public static RespawnPoint FromReader(BinaryReaderEx br) => new RespawnPoint(br);

        public RespawnPoint(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            Pose.Position = br.ReadVector3s(Helpers.GteScaleSmall);
            DistanceToFinish = br.ReadInt16();
            next = br.ReadByte();
            left = br.ReadByte();
            prev = br.ReadByte();
            right = br.ReadByte();
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3s(Pose.Position, Helpers.GteScaleSmall);
            bw.Write(DistanceToFinish);
            bw.Write(next);
            bw.Write(left);
            bw.Write(prev);
            bw.Write(right);
        }

        public override string ToString() => $"{Pose} index: {prev} linked: {next}";
    }
}