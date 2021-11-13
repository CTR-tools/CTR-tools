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

            if (cnt >= 4)
            {
                br.Jump(ptrs[3]);

                br.Seek(1);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < 291; i++)
                {
                    Vertex v = new Vertex();
                    v.Position = br.ReadVector3s(1 / 100f);

                    sb.AppendFormat($"v {v.Position.X} {v.Position.Y} {v.Position.Z}\r\n");
                }

                //Helpers.WriteToFile(".\\test.obj", sb.ToString());
            }
        }
    }
}