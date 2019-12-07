using CTRFramework;
using CTRFramework.Shared;
using System.IO;
using System.Text;

namespace levTool
{
    class Mcs : IRead
    {


        public Mcs()
        {
        }
        public Mcs(BinaryReaderEx br)
        {
            Read(br);
        }
        public void Read(BinaryReaderEx br)
        {
            br.Jump(256 + 2);
            int datasize = br.ReadInt16();

            br.Skip(2 + 2 + 4 + 4 + 4 + 5 * 4);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < datasize / 6; i++)
            {
                Vector3s v = new Vector3s(br);

                sb.AppendFormat("v {0}\r\n", v.ToString(CTRFramework.VecFormat.Numbers));
            }

            File.WriteAllText("testghost.obj", sb.ToString());
        }
    }
}
