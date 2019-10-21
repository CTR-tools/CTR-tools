using CTRFramework.Shared;
using System;

namespace CTRFramework
{
    class FaceGroup
    {
        int start;

        public bool valid = false;

        short u1;
        short u2;
        int nil;
        int[] list = new int[10];

        public FaceGroup(BinaryReaderEx br, int control)
        {
            start = br.ReadInt32();

            if (start != control)
            {
                valid = true;
                u1 = br.ReadInt16();
                u2 = br.ReadInt16();
                nil = br.ReadInt32();

                for (int i = 0; i < 10; i++)
                {
                    list[i] = br.ReadInt32();
                }

                if (nil != 0) Console.WriteLine("nil != 0!");
                if (start != list[0]) Console.WriteLine("start doesn't match!");
            }
        }

        public override string ToString()
        {
            string r = "FaceGroup: " + start.ToString("x8") + " " + u1 + " " + u2 + " " + nil + "[";
            foreach (int x in list)
                r += (x - start) + ", ";

            r += "]";

            return r;
        }
    }
}
