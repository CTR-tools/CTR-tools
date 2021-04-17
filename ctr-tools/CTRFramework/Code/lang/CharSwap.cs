using System;
using System.IO;
using System.Linq;

namespace CTRFramework.Lang
{
    enum SwapMode
    {
        ToEnglish,
        ToLocal
    }

    class CharSwap
    {
        char[] srcSet;
        char[] dstSet;

        public CharSwap(string filename)
        {
            string[] lines = File.ReadAllLines(filename);

            if (lines.Length < 2)
                throw new Exception("Insufficient text to work with.");

            if (lines[0].Length != lines[1].Length)
                throw new Exception("Char string length mismatch.");

            srcSet = lines[0].ToArray();
            dstSet = lines[1].ToArray();
        }

        public string Parse(string text, SwapMode swapmode)
        {
            char[] target = text.ToArray();

            for (int i = 0; i < target.Length; i++)
            {
                if (swapmode == SwapMode.ToLocal)
                {
                    if (srcSet.Contains(target[i]))
                    {
                        int x = Array.IndexOf(srcSet, target[i]);
                        target[i] = dstSet[x];
                    }
                }
                else
                {
                    if (dstSet.Contains(target[i]))
                    {
                        int x = Array.IndexOf(dstSet, target[i]);
                        target[i] = srcSet[x];
                    }
                }
            }

            return new string(target);
        }
    }
}