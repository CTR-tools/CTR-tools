using CTRFramework.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CTRFramework.Audio
{
    public class XaStackedFrameCollection : List<XaStackedFrame>
    {
        public static XaStackedFrameCollection FromFile(string filename)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(filename)))
            {
                return FromReader(br);
            }
        }

        public static XaStackedFrameCollection FromReader(BinaryReaderEx br)
        {
            var list = new XaStackedFrameCollection();

            while (br.CanRead)
            {
                list.Add(XaStackedFrame.FromReader(br));
            }

            return list;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Count; i++)
                sb.AppendLine($"{i.ToString("0000")}: {this[i]}");

            return sb.ToString();
        }
    }
}