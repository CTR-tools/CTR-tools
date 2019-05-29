using CTRtools.Helpers;

namespace CTRtools.SFX
{
    public class VAGHeader
    {
        public string magic = "VAGp"; //should be VAGp;
        public int version = 3;
        public int reserved1 = 0;
        public int datasize = 0;
        public int frequency = VAG.DefaultSampleRate;
        public byte[] reserved10bytes = new byte[10];
        public byte numChannels = 0;
        public byte reserved2 = 0;
        public string name = ""; //16 bytes

        public VAGHeader()
        {
        }

        public bool Read(BinaryReaderEx br)
        {
            magic = System.Text.Encoding.ASCII.GetString(br.ReadBytes(4));

            if (magic != "VAGp")
                return false;

            version = br.ReadInt32Big();
            reserved1 = br.ReadInt32();
            datasize = br.ReadInt32Big();
            frequency = br.ReadInt32Big();
            br.Skip(10);
            numChannels = br.ReadByte();
            reserved2 = br.ReadByte();
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Replace("\0", "");

            return true;
        }


        public void Write(BinaryWriterEx bw)
        {
            bw.Write(magic.ToCharArray());
            bw.WriteBig(version);
            bw.WriteBig(reserved1);
            bw.WriteBig(datasize);
            bw.WriteBig(frequency);
            bw.Write(reserved10bytes);
            bw.Write(numChannels);
            bw.Write(reserved2);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("testtesttesttest")); //name should go here
        }

    }
}
