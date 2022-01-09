using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework.Sound
{
    public class VagFrame : IReadWrite
    {
        public byte predict_nr = 0;
        public byte shift_factor = 0;
        public byte flags = 0;

        public byte[] data = new byte[14];

        public bool IsEmpty
        {
            get
            {
                if (predict_nr != 0) return false;
                if (shift_factor != 0) return false;
                if (flags != 0) return false;

                foreach (byte b in data)
                    if (b > 0)
                        return false;

                return true;
            }
        }

        public VagFrame()
        {
        }

        public VagFrame(BinaryReaderEx br)
        {
            Read(br);
        }

        public static VagFrame FromReader(BinaryReaderEx br)
        {
            return new VagFrame(br);
        }

        /// <summary>
        /// Reads Vag frame from stream using binary reader.
        /// </summary>
        /// <param name="br">BinaryReaderEx object.</param>
        public void Read(BinaryReaderEx br)
        {
            predict_nr = br.ReadByte();
            shift_factor = (byte)(predict_nr & 0xf);
            predict_nr >>= 4;
            flags = br.ReadByte();
            data = br.ReadBytes(14);
        }

        private static double[,] f = {
                        { 0.0, 0.0 },
                        { 60.0 / 64.0,  0.0 },
                        { 115.0 / 64.0, -52.0 / 64.0 },
                        { 98.0 / 64.0, -55.0 / 64.0 },
                        { 122.0 / 64.0, -60.0 / 64.0 }
        };

        /// <summary>
        /// Converts compressed VAG frame data to raw PCM data.
        /// Converted from C++ source: https://github.com/ColdSauce/psxsdk/blob/master/tools/vag2wav.c
        /// </summary>
        /// <returns>Array of bytes.</returns>
        public byte[] GetRawData(ref double s_1, ref double s_2)
        {
            if (flags == 7)
                return new byte[0];

            int d, s;
            double[] samples = new double[28];

            for (int i = 0; i < 28; i += 2)
            {
                d = data[i / 2];
                s = (d & 0xf) << 12;

                if ((s & 0x8000) > 0)
                    s = (int)(s | 0xffff0000);

                samples[i] = (double)(s >> shift_factor);

                s = (d & 0xf0) << 8;

                if ((s & 0x8000) > 0)
                    s = (int)(s | 0xffff0000);

                samples[i + 1] = (double)(s >> shift_factor);
            }

            List<byte> rawdata = new List<byte>();

            for (int i = 0; i < 28; i++)
            {
                samples[i] = samples[i] + s_1 * f[predict_nr, 0] + s_2 * f[predict_nr, 1];
                s_2 = s_1;
                s_1 = samples[i];
                d = (short)Math.Round(samples[i] + 0.5f);

                rawdata.Add((byte)((short)d & 0xFF));
                rawdata.Add((byte)(((short)d >> 8) & 0xFF));
            }

            return rawdata.ToArray();
        }

        /// <summary>
        /// Writes VAG frame data to stream using binary writer.
        /// </summary>
        /// <param name="bw">Binary writer object.</param>
        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            if (data.Length != 14)
                throw new Exception("Wrong VAG frame data length.");

            bw.Write((byte)((predict_nr << 4) | shift_factor));
            bw.Write(flags);
            bw.Write(data);
        }
    }
}