using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CTRFramework
{
    public class NavFrame : IReadWrite
    {
        public static readonly int SizeOf = 0x20;

        public Vector3 position;
        public Vector3s angle;
        public byte unk11;
        public byte unk12;
        public ushort unk2;
        public ushort unk3;
        public byte pos;
        public byte unk5;

        public NavFrame()
        {
        }

        public NavFrame(BinaryReaderEx br) => Read(br);

        public static NavFrame FromReader(BinaryReaderEx br) => new NavFrame(br);

        public void Read(BinaryReaderEx br)
        {
            Helpers.Panic(this, PanicType.Debug, $"frame starts at {br.HexPos()}... [total stream length: {br.BaseStream.Length.ToString("X8")}]...");

            position = br.ReadVector3s(1 / 100f);
            angle = new Vector3s(br);
            unk11 = br.ReadByte();
            unk12 = br.ReadByte();
            unk2 = br.ReadUInt16();
            unk3 = br.ReadUInt16();
            pos = br.ReadByte();
            unk5 = br.ReadByte();

            Helpers.Panic(this, PanicType.Debug, $"frame done, now at {br.HexPos()}");
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.WriteVector3s(position, 1 / 100f);
            angle.Write(bw);
            bw.Write(unk11);
            bw.Write(unk12);
            bw.Write(unk2);
            bw.Write(unk3);
            bw.Write(pos);
            bw.Write(unk5);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(position.ToString());
            sb.Append(", " + angle.ToString());
            sb.Append(", " + unk11);
            sb.Append(", " + unk12);
            sb.Append(", " + unk2);
            sb.Append(", " + unk3);
            sb.Append(", " + pos);
            sb.Append(", " + unk5);

            return sb.ToString();
        }
    }
}