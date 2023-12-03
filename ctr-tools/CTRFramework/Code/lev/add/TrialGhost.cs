using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace CTRFramework
{
    public class TrialGhost : IReadWrite
    {
        public ushort magic;
        public ushort datasize => (ushort)data.Length;
        public Level trackIndex;
        public CharIndex charIndex;

        //guess ghost times here
        public Vector3 vec1;
        public Vector3 vec2;

        public List<ushort> unk = new List<ushort>();

        public byte[] data;

        public TrialGhost()
        {
        }

        public TrialGhost(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            magic = br.ReadUInt16();

            if (magic != 0xFFFC)
                Helpers.Panic(this, PanicType.Error, $"Ghost magic value mismatch: {magic}.");

            ushort size = br.ReadUInt16();
            trackIndex = (Level)br.ReadUInt16();
            charIndex = (CharIndex)br.ReadUInt16();

            vec1 = br.ReadVector3s(Helpers.GteScaleSmall);
            vec2 = br.ReadVector3s(Helpers.GteScaleSmall);

            for (int i = 0; i < 10; i++)
                unk.Add(br.ReadUInt16());

            data = br.ReadBytes(size);
        }

        public static TrialGhost FromReader(BinaryReaderEx br)
        {
            return new TrialGhost(br);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write((ushort)0xFFFC);
            bw.Write(datasize);
            bw.Write((ushort)trackIndex);
            bw.Write((ushort)charIndex);

            bw.WriteVector3s(vec1, Helpers.GteScaleSmall);
            bw.WriteVector3s(vec2, Helpers.GteScaleSmall);

            foreach (var ptr in unk)
                bw.Write(ptr);

            bw.Write(data);
        }

        public void Save(string filename)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(filename)))
            {
                Write(bw);
            }
        }

        public void ToObj(string filename)
        {
            StringBuilder sb = new StringBuilder();

            float x = 0;
            float y = 0;
            float z = 0;

            using (var br = new BinaryReaderEx(new MemoryStream(data)))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    byte packet = br.ReadByte();

                    switch (packet)
                    {
                        case 0x80:
                            x = br.ReadInt16Big() / 100f * 8;
                            y = br.ReadInt16Big() / 100f * 8;
                            z = br.ReadInt16Big() / 100f * 8;
                            br.Seek(4);
                            sb.AppendFormat($"v {x} {y} {z}\r\n");
                            break;
                        case 0x81: br.Seek(2); break;
                        case 0x82: br.Seek(5); break;
                        case 0x83: br.Seek(1); break;
                        case 0x84: break;
                        default:
                            br.Seek(-1);
                            x += br.ReadSByte() / 100f * 8;
                            y += br.ReadSByte() / 100f * 8;
                            z += br.ReadSByte() / 100f * 8;
                            br.Seek(2);
                            sb.AppendFormat($"v {x} {y} {z}\r\n");
                            break;
                    }
                }
            }

            Helpers.WriteToFile(filename, sb.ToString());
        }

        public override string ToString()
        {
            return charIndex + " " + trackIndex;
        }
    }
}

