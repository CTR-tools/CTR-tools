using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public enum Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    //a collection of 3 navpaths for each difficulty level
    public class NavData : IReadWrite
    {
        public List<uint> ptrs = new List<uint>();
        public List<NavPath> paths = new List<NavPath>();

        public NavData()
        {
        }

        public NavData(BinaryReaderEx br) => Read(br);

        public static NavData FromReader(BinaryReaderEx br) => new NavData(br);

        public void Read(BinaryReaderEx br)
        {
            ptrs = br.ReadListUInt32(3);

            for (int i = 0; i < 3; i++)
            {
                if (ptrs[i] != 0)
                {
                    br.Jump(ptrs[i]);
                    paths.Add(NavPath.FromReader(br));
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
            var sb = new StringBuilder();

            startindex += 1;

            if (ptrs[0] != 0)
            {
                sb.AppendLine("o BotPath_Easy");
                sb.AppendLine(paths[0].ToObj(ref startindex));
            }

            if (ptrs[1] != 0)
            {
                sb.AppendLine("o BotPath_Medium");
                sb.AppendLine(paths[1].ToObj(ref startindex));
            }

            if (ptrs[2] != 0)
            {
                sb.AppendLine("o BotPath_Hard");
                sb.AppendLine(paths[2].ToObj(ref startindex));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (uint u in ptrs)
                sb.AppendLine(u.ToString("X8"));

            foreach (NavPath p in paths)
                sb.AppendLine(p.ToString());

            return sb.ToString();
        }
    }
}
