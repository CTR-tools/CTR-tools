using CTRFramework.Shared;
using System;
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

                Console.WriteLine(c);
                Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            }

            Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            //Console.ReadKey();

            for (int i = 0; i < numEntries; i++)
                Entries.Add(Pose.FromReader(br));
        }
    }
}