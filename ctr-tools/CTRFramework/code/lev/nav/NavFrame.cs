using CTRFramework.Shared;
using System.Text;

namespace CTRFramework
{
    public class NavFrame : IRead
    {
        public Vector3s position;
        byte[] data;
        //public List<short> data = new List<short>();

        public NavFrame()
        {

        }

        public NavFrame(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            position = new Vector3s(br);

            data = br.ReadBytes(7 * 2);
            /*
            for (int i = 0; i < 7; i++)
                data.Add(br.ReadInt16());
                */
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte s in data)
                sb.Append(s.ToString("X2"));

            sb.Append("\t" + position);

            return sb.ToString();
        }
    }
}