using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CTRFramework
{
    public class NavFrame : IReadWrite
    {
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

        public NavFrame(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            position = br.ReadVector3s(1 / 100f);
            angle = new Vector3s(br);
            unk11 = br.ReadByte();
            unk12 = br.ReadByte();
            unk2 = br.ReadUInt16();
            unk3 = br.ReadUInt16();
            pos = br.ReadByte();
            unk5 = br.ReadByte();
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
            StringBuilder sb = new StringBuilder();

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