using CTRFramework;
using CTRFramework.Shared;
using CTRtools.Helpers;
using System;
using System.Text;

namespace CTRtools.CSEQ
{
    public class Instrument
    {
        public InstType instType;

        public byte magic1;
        public byte velocity;
        public short always0;
        public short basepitch;  //4096 is considered to be 44100

        public int frequency
        {
            //cents needed?
            get { return (int)(basepitch * 44100.0 / 4096.0); }
        }

        public short sampleID;

        public short unknownFF80;
        public byte reverb;     //maybe reverb is 2 bytes? mostly 193
        public byte reverb2;    //unknown value, mostly 31

        public MetaInst info;


        public Instrument()
        {
        }


        static Instrument Get(BinaryReaderEx br, InstType it)
        {
            Instrument s = new Instrument();
            s.Read(br, it);

            return s;
        }

        public static Instrument GetLong(BinaryReaderEx br)
        {
            return Get(br, InstType.Long);
        }

        public static Instrument GetShort(BinaryReaderEx br)
        {
            return Get(br, InstType.Short);
        }

        public void Read(BinaryReaderEx br, InstType it)
        {
            instType = it;

            switch (it)
            {
                case InstType.Long: ReadLong(br); break;
                case InstType.Short: ReadShort(br); break;
            }
        }

        public string Tag => sampleID.ToString("X4") + "_" + frequency;

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

        public string ToSFZ(string cseqname)
        {
            StringBuilder sb = new StringBuilder();

            string samplepath = cseqname + "/" + "sample_" + sampleID.ToString("X4") + ".wav";

            sb.Append("<group>\r\n");
            sb.Append("//" + instType + "\r\n");
            sb.Append("\tvolume=" + (velocity / 255.0).ToString("0.0##").Replace(",", ".") + "\r\n");

            sb.Append("<region>\r\n");
            sb.Append("\tsample=" + samplepath + "\r\n");
            sb.Append("\ttrigger=attack\r\n");

            if (instType == InstType.Short)
            {
                sb.Append("\tkey=" + 0 + "\r\n");
            }
            else
            {
                sb.Append("\tlokey=0\r\n");
                sb.Append("\thikey=120\r\n");
            }

            sb.Append("\r\n");

            return sb.ToString();
        }


        /*
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
        */

        public void Write(BinaryWriterEx bw)
        {
            switch (instType)
            {
                case InstType.Long: WriteLong(bw); break;
                case InstType.Short: WriteShort(bw); break;
            }
        }

        #region [Private functions]

        private void ReadLong(BinaryReaderEx br)
        {
            magic1 = br.ReadByte();
            velocity = br.ReadByte();
            always0 = br.ReadInt16();
            basepitch = br.ReadInt16();
            sampleID = br.ReadInt16();
            unknownFF80 = br.ReadInt16();
            reverb = br.ReadByte();
            reverb2 = br.ReadByte();
        }

        private void ReadShort(BinaryReaderEx br)
        {
            magic1 = br.ReadByte();
            velocity = br.ReadByte();
            basepitch = br.ReadInt16();
            sampleID = br.ReadInt16();
            always0 = br.ReadInt16();
        }

        private void WriteLong(BinaryWriterEx bw)
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

        private void WriteShort(BinaryWriterEx bw)
        {
            bw.Write((byte)magic1);
            bw.Write((byte)velocity);
            bw.Write((short)basepitch);
            bw.Write((short)sampleID);
            bw.Write((short)always0);
        }

        #endregion

    }
}
