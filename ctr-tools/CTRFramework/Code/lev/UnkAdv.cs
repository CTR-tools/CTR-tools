using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class UnkAdv
    {
        public List<PosAng> smth = new List<PosAng>();

        public UnkAdv(BinaryReaderEx br, int cnt)
        {
            int ttl = 0;

            for (int i = 0; i < cnt; i++)
            {
                int c = br.ReadInt32();
                ttl += c;
                br.Skip(4);

                Console.WriteLine(c);
                Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            }

            Console.WriteLine(br.BaseStream.Position.ToString("X8"));
            //Console.ReadKey();

            for (int i = 0; i < ttl; i++)
            {
                smth.Add(new PosAng(br));
            }
        }
    }
}