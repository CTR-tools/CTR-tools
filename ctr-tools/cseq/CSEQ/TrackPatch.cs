using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cseq
{

    public class BytePair
    {
        public byte A;
        public byte B;

        public byte Swap(byte x)
        {
            if (x == A) return B;
            return x;
        }

        public BytePair(byte a, byte b)
        {
            A = a;
            B = b;
        }
    }

    public class TrackPatch
    {

        string name;
        List<BytePair> instSwaps = new List<BytePair>();
        List<BytePair> drumSwaps = new List<BytePair>();

        //string val = "arena|1=23;2=30;3=18;0=33|255=255";
        string val = "test|0=0|0=0";

        public TrackPatch()
        {
            string[] kek = val.Split('|');

            name = kek[0];

           // System.Windows.Forms.MessageBox.Show(kek[0] + "\r\n" + kek[1] + "\r\n" + kek[2]);

            string[] inst = kek[1].Split(';');

            foreach (string s in inst)
            {
                string[] x = s.Split('=');
               // System.Windows.Forms.MessageBox.Show(x[0] + " <---> " + x[1]);

                    byte b1 = Byte.Parse(x[0]);
                    byte b2 = Byte.Parse(x[1]);

                    BytePair bp = new BytePair(b1, b2);

                    instSwaps.Add(bp);

            }

            string[] drums = kek[2].Split(';');

            foreach (string s in drums)
            {
                string[] x = s.Split('=');
                drumSwaps.Add(new BytePair(Byte.Parse(x[0]), Byte.Parse(x[1])));
            }

        }

        public byte SwapDrum(byte x)
        {
            foreach (BytePair ip in drumSwaps)
            {
                if (x != ip.Swap(x)) 
                    return ip.Swap(x);
            }

            return x;
        }


        public byte SwapPatch(byte x)
        {
            foreach (BytePair ip in instSwaps)
            {
                if (x != ip.Swap(x)) 
                    return ip.Swap(x);
            }

            return x;
        }



    }
}
