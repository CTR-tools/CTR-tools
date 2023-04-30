using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public class Nav : IReadWrite
    {
        public List<uint> ptrs = new List<uint>();
        public List<BotPath> paths = new List<BotPath>();

        public Nav()
        {
        }

        public Nav(BinaryReaderEx br) => Read(br);

        public static Nav FromReader(BinaryReaderEx br) => new Nav(br);

        public void Read(BinaryReaderEx br)
        {
            ptrs = br.ReadListUInt32(3);

            for (int i = 0; i < 3; i++)
            {
                if (ptrs[i] != 0)
                {
                    br.Jump(ptrs[i]);
                    paths.Add(new BotPath(br));
                }
            }
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            foreach (uint p in ptrs)
                bw.Write(p);

            for (int i = 0; i < ptrs.Count; i++)
            {
                bw.Jump(ptrs[i] + 4);
                paths[i].Write(bw, patchTable);
            }
        }

        public string ToObj(ref int startindex)
        {
            StringBuilder sb = new StringBuilder();

            startindex += 1;

            if (ptrs[0] != 0)
            {
                sb.AppendLine("o BotPathEasy");
                sb.AppendLine(paths[0].ToObj(ref startindex));
            }

            if (ptrs[1] != 0)
            {
                sb.AppendLine("o BotPathMedium");
                sb.AppendLine(paths[1].ToObj(ref startindex));
            }

            if (ptrs[2] != 0)
            {
                sb.AppendLine("o BotPathHard");
                sb.AppendLine(paths[2].ToObj(ref startindex));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (uint u in ptrs)
                sb.AppendLine(u.ToString("X8"));

            foreach (BotPath p in paths)
                sb.AppendLine(p.ToString());

            return sb.ToString();
        }
    }
}
