using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework.Shared
{
    public class Pose : IReadWrite
    {
        public static Pose Zero => new Pose(Vector3.Zero, Vector3.Zero);

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Pose()
        {
        }

        public static Pose FromReader(BinaryReaderEx br)
        {
            return new Pose(br);
        }

        public Pose(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Move(Vector3 move)
        {
            Position += move;
        }

        public void Rotate(Vector3 rot)
        {
            Rotation += rot;
        }

        public Pose(Vector3 pos, Vector3 ang)
        {
            Position = pos;
            Rotation = ang;
        }

        public void Read(BinaryReaderEx br)
        {
            Position = br.ReadVector3s(1 / 100f);
            Rotation = br.ReadVector3s(1 / 4096f);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3s(Position, 1 / 100f);
            bw.WriteVector3s(Rotation, 1 / 4096f);
        }

        public override string ToString() => $"Pos: {Position} Rot: {Rotation}";
    }
}