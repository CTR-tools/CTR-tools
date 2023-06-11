using CTRFramework.Shared;
using System.Collections.Generic;

namespace CTRFramework
{
    public class SpawnGroup
    {
        public List<Pose> Entries = new List<Pose>();

        public SpawnGroup(BinaryReaderEx br, int numGroups)
        {
            int numEntries = 0;

            for (int i = 0; i < numGroups; i++)
            {
                int c = br.ReadInt32();
                numEntries += c;
                br.Seek(4);

                Helpers.Panic(this, PanicType.Debug, $"{br.HexPos()} {c}");
            }

            for (int i = 0; i < numEntries; i++)
                Entries.Add(Pose.FromReader(br));
        }
    }
}