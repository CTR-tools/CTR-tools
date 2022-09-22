using CTRFramework;
using CTRFramework.Shared;

namespace bash_dat
{
    public class BashSfx : IRead
    {
        public byte[] VB;
        public byte[] VH;
        public List<byte[]> SEQ = new List<byte[]>();

        public BashSfx(BinaryReaderEx br)
        {
            Read(br);
        }

        public static BashSfx FromReader(BinaryReaderEx br)
        {
            return new BashSfx(br);
        }

        public static BashSfx FromFile(string path)
        {
            using (var br = new BinaryReaderEx(File.OpenRead(path)))
            {
                return FromReader(br);
            }
        }

        public void Read(BinaryReaderEx br)
        {
            int numSeq = br.ReadInt32() / 4 - 2;

            if (numSeq > 5)
                throw new Exception($"unlikely a bash sfx file: {numSeq} songs");

            br.Jump(0);

            int ptrVB = br.ReadInt32();
            int ptrVH = br.ReadInt32();
            List<int> ptrSEQ = br.ReadListInt32(numSeq);
            ptrSEQ.Add((int)br.BaseStream.Length);

            br.Jump(ptrVB);
            VB = br.ReadBytes(ptrVH - ptrVB);

            br.Jump(ptrVH);
            VH = br.ReadBytes(ptrSEQ[0] - ptrVB);

            for (int i = 0; i < ptrSEQ.Count - 1; i++)
            {
                int size = ptrSEQ[i + 1] - ptrSEQ[i];

                if (size == 0)
                    break;

                br.Jump(ptrSEQ[i]);
                SEQ.Add(br.ReadBytes(size));
            }
        }

        public void Export(string path)
        {
            Helpers.CheckFolder(path);

            Helpers.WriteToFile(Helpers.PathCombine(path, "data.vb"), VB);
            Helpers.WriteToFile(Helpers.PathCombine(path, "data.vh"), VH);

            for (int i = 0; i < SEQ.Count; i++)
                Helpers.WriteToFile(Helpers.PathCombine(path, $"data_{i}.seq"), SEQ[i]);
        }
    }
}
