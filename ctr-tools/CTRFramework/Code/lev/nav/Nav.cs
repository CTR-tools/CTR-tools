using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class Nav : IReadWrite
    {
        public List<uint> ptrs = new List<uint>();
        public List<AIPath> paths = new List<AIPath>();

        public Nav()
        {

        }

        public Nav(BinaryReaderEx br)
        {
            Read(br);
        }
        public void Read(BinaryReaderEx br)
        {
            ptrs = br.ReadListUInt32(3);

            for (int i = 0; i < 3; i++)
            {
                if (ptrs[i] != 0)
                    paths.Add(new AIPath(br));
            }
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            foreach (uint p in ptrs)
                bw.Write(p);

            foreach (AIPath ai in paths)
                ai.Write(bw);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (uint u in ptrs)
                sb.AppendLine(u.ToString("X8"));

            foreach (AIPath p in paths)
                sb.AppendLine(p.ToString());

            return sb.ToString();
        }
    }
}
