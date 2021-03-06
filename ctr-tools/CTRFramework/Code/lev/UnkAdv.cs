using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class UnkAdv
    {
        public List<Pose> smth = new List<Pose>();

        public UnkAdv(BinaryReaderEx br, int cnt)
        {
            int ttl = 0;

            for (int i = 0; i < cnt; i++)
            {
                int c = br.ReadInt32();
                ttl += c;
                br.Seek(4);

                Console.WriteLine(c);
                Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            }

            Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            //Console.ReadKey();

            for (int i = 0; i < ttl; i++)
            {
                smth.Add(new Pose(br));
            }
        }
    }
}