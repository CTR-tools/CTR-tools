using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRFramework
{
    class CTRAnim
    {
        string name;
        short numFrames;
        short frameSize;
        int someOffset;//??

        List<byte[]> frames = new List<byte[]>();

        public CTRAnim(BinaryReader br)
        {
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Replace("\0", "");
            numFrames = br.ReadInt16();
            frameSize = br.ReadInt16();
            someOffset = br.ReadInt32();

            Console.WriteLine("anim -> " + name + " " + numFrames + " frames " + frameSize + " framesize");

            byte[] data = br.ReadBytes(frameSize);
            frames.Add(data);
        }


        public void Stuff(BinaryReader br)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numFrames; i++)
            {
                sb.Append(String.Format("g frame{0}\r\n", i.ToString("000")));
                sb.Append(String.Format("o frame{0}\r\n", i.ToString("000")));

                byte[] data = br.ReadBytes(frameSize);
                frames.Add(data);


                MemoryStream ms = new MemoryStream(data);
                BinaryReader brz = new BinaryReader(ms);

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
