using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class Texture
    {
        uint[] ptr_tex_hi = new uint[4];

        public uint ptr1;
        public uint ptr2;
        public uint ptr3;
        public uint ptr4;

        public Texture(int x, BinaryReaderEx br)
        {
            long back = br.BaseStream.Position;

            if (x != 0)
            {
                br.Jump(x);

                ptr1 = br.ReadUInt32();
                ptr2 = br.ReadUInt32();
                ptr3 = br.ReadUInt32();
                ptr4 = br.ReadUInt32();
            }

            br.BaseStream.Position = back;
        }
    }
}
