using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace cseq
{
    class RAWVABConverter
    {
        public RAWVABConverter(byte[] data, string fn)
        {
            using (BinaryReaderEx vag = new BinaryReaderEx(new MemoryStream(data)))
            {
                using (BinaryWriterEx pcm = new BinaryWriterEx(File.OpenWrite(fn + ".pcm")))
                {

                List<PointF> f = new List<PointF>();

                f.Add(new PointF(0.0f, 0.0f));
                f.Add(new PointF(60.0f / 64.0f, 0.0f));
                f.Add(new PointF(115.0f / 64.0f, -52.0f / 64.0f));
                f.Add(new PointF(98.0f / 64.0f, -55.0f / 64.0f));
                f.Add(new PointF(122.0f / 64.0f, -60.0f / 64.0f));


                double[] samples = new double[28];
                //string fname;
                //char *p; //??
                int predict_nr, shift_factor, flags;
                uint i;
                uint d, s;
                double s_1 = 0.0; //was static, why
                double s_2 = 0.0; //was static, why

                //fseek( vag, 64, SEEK_SET );


                while (true)
                {
                    predict_nr = vag.ReadByte();
                    shift_factor = predict_nr & 0xf;
                    predict_nr >>= 4;
                    flags = vag.ReadByte();                          // flags

                    if (flags == 7)
                        break;

                    for (i = 0; i < 28; i += 2)
                    {
                        d = vag.ReadByte();
                        s = (d & 0xf) << 12;
                        if ((s & 0x8000) != 0)
                            s |= 0xffff0000;
                        samples[i] = (double)(s >> shift_factor);
                        s = (d & 0xf0) << 8;
                        if ((s & 0x8000) != 0)
                            s |= 0xffff0000;
                        samples[i + 1] = (double)(s >> shift_factor);
                    }

                    for (i = 0; i < 28; i++)
                    {
                        samples[i] = samples[i] + s_1 * f[predict_nr].X + s_2 * f[predict_nr].Y;
                        s_2 = s_1;
                        s_1 = samples[i];
                        d = (uint)(samples[i] + 0.5);
                        pcm.Write(d & 0xff);
                        pcm.Write(d >> 8);
                    }
                }
            }
            }
        }
    }
}