using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    //a list of navframes
    public class NavPath : IReadWrite
    {
        public ushort version;
        public ushort NumFrames => (ushort)Frames.Count;
        public byte[] data;
        public NavFrame start;

        public List<NavFrame> Frames = new List<NavFrame>();

        public NavPath()
        {
        }

        public static NavPath FromReader(BinaryReaderEx br) => new NavPath(br);

        public NavPath(BinaryReaderEx br) => Read(br);

        public void Read(BinaryReaderEx br)
        {
            version = br.ReadUInt16();

            Helpers.Panic(this, PanicType.Info, $"Path version: 0x{version.ToString("X8")}");

            int numFrames = br.ReadUInt16();

            switch (version)
            {
                case 0xECFD:
                    data = br.ReadBytes(4 * 18); //0x4c = total header size
                    start = new NavFrame(br);
                    break;
                case 0xFEFD:
                    data = br.ReadBytes(4);
                    break;
                default:
                    Helpers.Panic(this, PanicType.Warning, "Unknown bot path version.");
                    break;
            }

            for (int i = 0; i < numFrames; i++)
                Frames.Add(NavFrame.FromReader(br));
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(version);
            bw.Write(NumFrames);

            switch (version)
            {
                case 0xECFD:
                    bw.Write(data);
                    start.Write(bw);
                    break;
                case 0xFEFD:
                    bw.Write(data);
                    break;
                default:
                    Helpers.Panic(this, PanicType.Warning, "Unknown bot path version.");
                    break;
            }

            foreach (var frame in Frames)
                frame.Write(bw);
        }

        public string ToObj(ref int startindex)
        {
            if (NumFrames == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var frame in Frames)
                sb.AppendLine($"v {frame.position.X} {frame.position.Y} {frame.position.Z}");

            sb.Append("\r\nl ");

            for (int i = 0; i < NumFrames; i++)
                sb.Append($"{startindex + i} ");

            sb.AppendLine($"{startindex}");

            startindex += NumFrames;

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("frames = " + NumFrames);

            sb.AppendLine("start: " + start.ToString());

            foreach (NavFrame nv in Frames)
            {
                sb.AppendLine(nv.ToString());
            }

            return sb.ToString();
        }
    }
}
