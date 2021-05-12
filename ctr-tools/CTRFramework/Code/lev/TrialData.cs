using CTRFramework.Shared;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class TrialData : IRead
    {
        public int cntArrays;
        public List<uint> ptrs = new List<uint>();

        public TrialData()
        {

        }

        public TrialData(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            int cnt = br.ReadInt32();

            for (int i = 0; i < cnt; i++)
                ptrs.Add(br.ReadUInt32());

            br.Seek(6);

            if (cnt > 4)
            {
                br.Jump(ptrs[4]);

                br.Seek(1);

                StringBuilder sb = new StringBuilder();
                do
                {
                    Vertex v = new Vertex();
                    v.coord = br.ReadVector3sPadded(1 / 100f);

                    br.Seek(2);

                    sb.AppendFormat("v {0}\r\n", v.coord.ToString());
                }
                while (br.BaseStream.Position < ptrs[5]);

                //Helpers.WriteToFile(".\\test.obj", sb.ToString());
            }
        }
    }
}