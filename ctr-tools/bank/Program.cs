using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace bank
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hi, i'm ctr bank reader");

            Bank b = new Bank();

            using (BinaryReader br = new BinaryReader(File.OpenRead(args[0])))
            {
                b.Read(br);
            }

            Console.ReadKey();
        }
    }
}
