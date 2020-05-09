using System;

namespace CTRFramework
{
    class OBJ
    {
        public static string ASCIIFace(string label, int totalv, int x, int y, int z)
        {
            return
                String.Format(
                    "{0} {1} {2} {3}\r\n",
                    label,
                    totalv + x, totalv + y, totalv + z
                    );
        }

        public static string ASCIIFace(string label, int totalv, int totalvt, int x, int y, int z, float xuv, float yuv, float zuv)
        {
            return String.Format(
                "{0} {1}/{2} {3}/{4} {5}/{6}\r\n",
                label,
                totalv + x, totalvt + xuv,
                totalv + y, totalvt + yuv,
                totalv + z, totalvt + zuv
                );
        }
    }
}