﻿using CTRFramework.Shared;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

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

            if (cnt >= 4)
            {
                var tropy = Instance<TrialGhost>.FromReader(br, ptrs[4]);
                Console.WriteLine(tropy.ToString());
                tropy.Save(Path.Combine(Meta.BasePath, "tropy.gst"));
                tropy.ToObj(Path.Combine(Meta.BasePath, "tropy.obj"));
            }

            if (cnt >= 5)
            {
                var oxide = Instance<TrialGhost>.FromReader(br, ptrs[5]);
                Console.WriteLine(oxide.ToString());
                oxide.Save(Path.Combine(Meta.BasePath, "oxide.gst"));
                oxide.ToObj(Path.Combine(Meta.BasePath, "oxide.obj"));
            }
        }
    }
}