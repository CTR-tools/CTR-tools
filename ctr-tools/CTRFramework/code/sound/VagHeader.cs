using System;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework.Sound
{
    public class VagHeader
    {
        public static int DefaultSampleRate = 11025;

        public string magic = "VAGp"; //should be VAGp;
        public int version = 3;
        public int reserved1 = 0;
        public int datasize = 0;
        public int frequency = DefaultSampleRate;
        public byte[] reserved10bytes = new byte[10];
        public byte numChannels = 0;
        public byte reserved2 = 0;
        public string name = ""; //16 bytes

        public VagHeader(byte[] data)
        {
            datasize = data.Length;
        }


        public void Write(BinaryWriterEx bw)
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

        public void WriteBig(BinaryWriterEx bw, int value)
        {
            byte[] x = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++) bw.Write(x[3 - i]);
        }


    }
}
