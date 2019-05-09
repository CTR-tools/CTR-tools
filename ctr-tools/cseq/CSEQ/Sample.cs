using System;
using System.Data;
using System.IO;

namespace cseq
{
    public enum InstType
    {
        Long,
        Short
    }

    public class Sample
    {
        public InstType instType;

        public byte magic1;
        public byte velocity;
        public short always0;
        public short basepitch;
        public short sampleID;

        public short unknownFF80;
        public byte reverb;     //maybe reverb is 2 bytes? mostly 193
        public byte reverb2;    //unknown value, mostly 31

        public Sample()
        {
        }

        static Sample Get(BinaryReaderEx br, InstType it)
        {
            Sample s = new Sample();
            if (s.Read(br, it)) return s;
            return null;
        }

        public static Sample GetLong(BinaryReaderEx br)
        {
            return Get(br, InstType.Long);
        }

        public static Sample GetShort(BinaryReaderEx br)
        {
            return Get(br, InstType.Short);
        }


        public bool Read(BinaryReaderEx br, InstType it)
        {
            instType = it;

            if (it == InstType.Long)
            {
                magic1 = br.ReadByte();
                velocity = br.ReadByte();
                always0 = br.ReadInt16();
                basepitch = br.ReadInt16();
                sampleID = br.ReadInt16();
                unknownFF80 = br.ReadInt16();
                reverb = br.ReadByte();
                reverb2 = br.ReadByte();

                return true;
            }
            else if (it == InstType.Short)
            {
                magic1 = br.ReadByte();
                velocity = br.ReadByte();
                basepitch = br.ReadInt16();
                sampleID = br.ReadInt16();
                always0 = br.ReadInt16();

                return true;
            }
            else
            {
                Log.WriteLine("unknown instrument type at " + br.HexPos());
                return false;
            }
        }

        public override string ToString()
        {
            switch (instType)
            {
                case InstType.Long: 
                    return String.Format(
                        "{0}: 0x01 - {1}\tvel - {2}\t0x00 - {3}\tpitch - {4}\tID - {5}\t0xFF80 - {6}\treverb - {7}\t0x00_2 - {8}",
                        instType.ToString(), magic1, velocity, always0, basepitch, sampleID.ToString("X4"), unknownFF80.ToString("X4"), reverb, reverb2
                        );

                case InstType.Short:
                    return String.Format(
                        "{0}: 0x01 - {1}\tvel - {2}\tpitch - {3}\tID - {4}\t0x00 - {5}",
                        instType.ToString(), magic1, velocity, basepitch, sampleID.ToString("X4"), always0
                        );

                default: return "unknown instrument type";
            }
        }

        public void ToDataRow(DataTable dt)
        {
            DataRow dr = dt.NewRow();

            dr["instType"] = instType.ToString();
            dr["magic1"] = magic1;
            dr["velocity"] = velocity;
            dr["basepitch"] = basepitch;
            dr["sampleID"] = sampleID.ToString("X4");
            dr["always0"] = always0;

            if (instType == InstType.Long)
            {
                dr["unknown_80FF"] = unknownFF80.ToString("X4");
                dr["reverb"] = reverb;
                dr["reverb2"] = reverb2;
            }

            dt.Rows.Add(dr);
        }


        public void WriteBytes(BinaryWriter bw)
        {
            if (instType == InstType.Long)
            {
                bw.Write((byte)magic1);
                bw.Write((byte)velocity);
                bw.Write((short)always0);
                bw.Write((short)basepitch);
                bw.Write((short)sampleID);
                bw.Write((short)unknownFF80);
                bw.Write((byte)reverb);
                bw.Write((byte)reverb2);
            }
            else if (instType == InstType.Short)
            {
                bw.Write((byte)magic1);
                bw.Write((byte)velocity);
                bw.Write((short)basepitch);
                bw.Write((short)sampleID);
                bw.Write((short)always0);
            }
        }

    }


}
