using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class AIPath : IRead
    {
        public ushort unk1;
        public ushort numFrames;
        public NavFrame start;
        public List<NavFrame> frames = new List<NavFrame>();
        public AIPath()
        {

        }

        public AIPath(BinaryReaderEx br)
        {
            Read(br);
        }
        public void Read(BinaryReaderEx br)
        {
            unk1 = br.ReadUInt16();
            numFrames = br.ReadUInt16();
            br.Skip(0x4C - 4); //0x4c = total header size

            start = new NavFrame(br);

            for (int i = 0; i < numFrames; i++)
            {
                frames.Add(new NavFrame(br));
            }
        }

        public string ToObj()
        {
            StringBuilder sb = new StringBuilder();

            foreach (NavFrame f in frames)
                sb.AppendFormat("v {0}\r\n", f.position.ToString(VecFormat.Numbers));

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("frames = " + numFrames);

            sb.AppendLine("start: " + start.ToString());

            foreach (NavFrame nv in frames)
            {
                sb.AppendLine(nv.ToString());
            }

            return sb.ToString();
        }
    }
}
