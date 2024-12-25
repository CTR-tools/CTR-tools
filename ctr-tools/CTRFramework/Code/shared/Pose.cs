using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace CTRFramework.Shared
{

    public class Pose : IReadWrite
    {
        public static Pose Zero => new Pose(Vector3.Zero, Vector3.Zero);

        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;

        [CategoryAttribute("Position")]
        [Description("Object position along the X axis")]
        public float X
        {
            get { return Position.X; }
            set { Position = new Vector3(value, Position.Y, Position.Z); }
        }

        [CategoryAttribute("Position")]
        [Description("Object rotation along the Y axis (the vertical one)")]
        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector3(Position.X, value, Position.Z); }
        }

        [CategoryAttribute("Position")]
        [Description("Object rotation along the Z axis")]
        public float Z
        {
            get { return Position.Z; }
            set { Position = new Vector3(Position.X, Position.Y, value); }
        }

        [CategoryAttribute("Rotation")]
        [Description("Object rotation along the X axis")]
        public float Pitch
        {
            get { return Rotation.X; }
            set { Position = new Vector3(value, Position.Y, Position.Z); }
        }
        [CategoryAttribute("Rotation")]
        [Description("Object rotation along the Y axis")]
        public float Yaw
        {
            get { return Rotation.Y; }
            set { Position = new Vector3(Position.X, value, Position.Z); }
        }
        [CategoryAttribute("Rotation")]
        [Description("Object rotation along the Z axis")]
        public float Roll
        {
            get { return Rotation.Z; }
            set { Position = new Vector3(Position.X, Position.Y, value); }
        }


        public Pose()
        {
        }

        public static Pose FromReader(BinaryReaderEx br) => new Pose(br);

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
            Position = br.ReadVector3s(Helpers.GteScaleSmall);
            Rotation = br.ReadVector3s(Helpers.GteScaleLarge);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3s(Position, Helpers.GteScaleSmall);
            bw.WriteVector3s(Rotation, Helpers.GteScaleLarge);
        }

        public override string ToString() => $"Pos: {Position} Rot: {Rotation}";
    }
}