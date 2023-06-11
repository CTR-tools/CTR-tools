using CTRFramework.Shared;
using System.Collections.Generic;

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

            //um
            br.Seek(6);

            long pos = br.Position;

            if (cnt > 4)
            {
                var tropy = Instance<TrialGhost>.FromReader(br, ptrs[4]);
                Helpers.Panic(this, PanicType.Debug, tropy.ToString());
                //tropy.Save(Helpers.PathCombine(Meta.BasePath, "tropy.gst"));
                //tropy.ToObj(Helpers.PathCombine(Meta.BasePath, "tropy.obj"));
            }

            if (cnt > 5)
            {
                var oxide = Instance<TrialGhost>.FromReader(br, ptrs[5]);
                Helpers.Panic(this, PanicType.Debug, oxide.ToString());
                //oxide.Save(Helpers.PathCombine(Meta.BasePath, "oxide.gst"));
                //oxide.ToObj(Helpers.PathCombine(Meta.BasePath, "oxide.obj"));
            }

            if (cnt > 6)
            {
                var credits = Instance<CreditsText>.FromReader(br, ptrs[6]);
                credits.Save(Helpers.PathCombine(Meta.BasePath, "credits.txt"));
            }

            br.Jump(pos);
        }
    }
}