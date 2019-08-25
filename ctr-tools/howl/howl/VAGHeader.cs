using System.IO;
using System;

namespace howl
{
    public class VAGHeader
    {
        public string magic = "VAGp"; //should be VAGp;
        public int version = 3;
        public int reserved1 = 0;
        public int datasize = 0;
        public int frequency = 11025;
        public byte[] reserved10bytes = new byte[10];
        public byte numChannels = 0;
        public byte reserved2 = 0;
        public string name = ""; //16 bytes

        public VAGHeader(byte[] data)
        {
            datasize = data.Length;
        }


        public void Write(BinaryWriter bw)
        {
            bw.Write(magic.ToCharArray());
            WriteBig(bw, version);
            WriteBig(bw, reserved1);
            WriteBig(bw, datasize);
            WriteBig(bw, frequency);
            bw.Write(reserved10bytes);
            bw.Write(numChannels);
            bw.Write(reserved2);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("testtesttesttest")); //name should go here
        }

        public void WriteBig(BinaryWriter bw, int value)
        {
            byte[] x = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++) bw.Write(x[3 - i]);
        }


    }
}
