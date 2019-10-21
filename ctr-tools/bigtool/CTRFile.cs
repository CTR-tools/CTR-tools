using System.IO;

namespace bigtool
{
    class CTRFile
    {
        public string name;
        public uint size;
        public uint offset;
        public uint padded_size;
        public byte[] data;

        public CTRFile(string path)
        {
            name = Path.GetFileName(path);
            data = new byte[0];
            size = 0;

            if (File.Exists(path))
            {
                data = File.ReadAllBytes(path);
                size = (uint)data.Length;
            }

            CalcPadding();
        }

        public void CalcPadding()
        {
            padded_size = size;
            while (padded_size % 2048 != 0) padded_size++;
        }

        public override string ToString()
        {
            return name + " " + ((size / 1024) + 1) + "kb ";
        }
    }
}
