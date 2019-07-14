using System;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class PickupHeader : IRead
    {
        public string name;
        public uint offsModel;

        //maybe scale
        public short px;
        public short py;
        public short pz;
        public short p0;

        public uint null1;

        public uint unk1;

        PosAng posang;

        /*
        //most likely position
        public short ax;
        public short ay;
        public short az;

        //most likely angle
        public short bx;
        public short by;
        public short bz;
        */

        //event type?
        public int unk2;

        public PickupHeader(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16));
            offsModel = br.ReadUInt32();
            px = br.ReadInt16();
            py = br.ReadInt16();
            pz = br.ReadInt16();

            p0 = br.ReadInt16();
            null1 = br.ReadUInt32();

            unk1 = br.ReadUInt32();

            br.BaseStream.Position += 4 * 3;

            posang = new PosAng(br);

            unk2 = br.ReadInt32();

            Console.WriteLine(posang.ToString());
        }

        public override string ToString()
        {
            return
                name +
                // "\t0x"+offsModel.ToString("X8") + 
                // "\t(" + px + ", " + py +  ", " + pz + ") " +
                // "\t" + null1 +
                //"\t" + unk1 +
                //"\t(" + ax + ", " + ay + ", " + az + ") " +
                //"\t(" + bx + ", " + by + ", " + bz + ") " +
                "\t" + unk2
                ;
        }
    }
}

