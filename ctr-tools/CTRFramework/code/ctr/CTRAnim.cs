using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework
{
    class CTRAnim
    {
        string name;
        byte numFrames;
        byte unk1;
        short frameSize;
        int someOffset;//??

        List<byte[]> frames = new List<byte[]>();

        public CTRAnim(BinaryReaderEx br, string s)
        {
            name = br.ReadStringFixed(16);
            numFrames = br.ReadByte();
            unk1 = br.ReadByte();
            frameSize = br.ReadInt16();
            someOffset = br.ReadInt32();

            Console.WriteLine("anim -> " + name + " " + numFrames + " frames " + frameSize + " framesize");

            /*
            if (numFrames > 0)
            {
                byte[] head = br.ReadBytes(0x18);
                byte[] data = br.ReadBytes(frameSize - 0x18);
                frames.Add(data);

                Console.WriteLine(frames[0].Length);
                Console.ReadKey();

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < (frames[0].Length - 3) / 3; i++)
                {
                    sb.AppendFormat("v {0} {1} {2}\r\n",
                        frames[0][i * 3] - 128,
                        frames[0][i * 3 + 1] - 128,
                        frames[0][i * 3 + 2] - 128
                        );
                }

                File.WriteAllText(s + "_" + name + ".obj", sb.ToString());
            }
            */
        }


        public void Stuff(BinaryReaderEx br)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numFrames; i++)
            {
                sb.Append(String.Format("g frame{0}\r\n", i.ToString("000")));
                sb.Append(String.Format("o frame{0}\r\n", i.ToString("000")));

                byte[] data = br.ReadBytes(frameSize);
                frames.Add(data);


                MemoryStream ms = new MemoryStream(data);
                BinaryReaderEx brz = new BinaryReaderEx(ms);

                for (int j = 0; j < frameSize / 3; j++)
                {
                    float x = (brz.ReadByte() / 255.0f - 0.5f) * 2000;
                    float y = (brz.ReadByte() / 255.0f - 0.5f) * 2000;
                    float z = (brz.ReadByte() / 255.0f - 0.5f) * 2000;

                    sb.Append(String.Format("v {0} {1} {2}\r\n", x.ToString("0.0000"), y.ToString("0.0000"), z.ToString("0.0000")));
                }

            }

            File.WriteAllText(String.Format("anim_{0}.obj", name), sb.ToString());
        }
    }
}
