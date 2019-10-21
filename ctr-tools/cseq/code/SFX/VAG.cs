using CTRFramework.Shared;
using System.IO;

namespace CTRtools.SFX
{
    public class VAG
    {
        public static int DefaultSampleRate = 22050;

        public VAGHeader header = new VAGHeader();

        public byte[] WaveData
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                header.datasize = value.Length;
            }
        }

        private byte[] data;


        public VAG()
        {
        }

        public VAG(VAGHeader h, byte[] d)
        {
            header = h;
            data = d;
        }

        public VAG(byte[] d)
        {
            data = d;
        }

        public VAG(string s)
        {
            //load vag from file here
        }

        public void Save(string s)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(s)))
            {
                if (header != null)
                    header.Write(bw);

                if (data != null)
                    bw.Write(data);
            }
        }
    }
}