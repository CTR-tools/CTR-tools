using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CTRtools
{
    public class PickupHeader
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


        public PosAng posang;

        //event type?
        public int evt;


        enum CTREvent
        {
            Nothing = -1,
            SingleFruit = 2,
            CrateNitro = 6,
            CrateFruit = 7,
            CrateWeapon = 8,
            StateBurned = 18,
            StateEaten = 19,
            CrateTNT = 0x27,
            StateSquished = 33,
            StateSquishedBall = 34,
            StateRotatedArmadillo = 36,
            StateKilledBlades = 37,
            Pipe = 0x57,
            Vent = 0x59,
            Crystal = 0x60,
            pass_seal = 76,
            StateSquishedBarrel = 78,
            StateTurleJump = 81,
            StateRotatedSpider = 82,
            StateBurnedInAir = 84,
            labs_drum = 85,
            StateCastleSign = 91, //what?
            WarpPad = 108,
            Teeth = 112,  //trigger secret passage script?
            SaveScreen = 114, 

            GaragePin = 115,
            GaragePapu = 116,
            GarageRoo = 117,
            GarageJoe = 118,
            GarageOxide = 119,

            DoorUnknown = 122,

            LetterC = 147,
            LetterT = 148,
            LetterR = 149,
            FinishLap = 166, //check
           
            HubDoor = 225,
            CrateRelic1 = 0x5C,
            CrateRelic2 = 0x64,
            CrateRelic3 = 0x65

    }



        public PickupHeader(BinaryReader br)
        {
            name = System.Text.Encoding.ASCII.GetString(br.ReadBytes(16)).Replace("\0", "");
            offsModel = br.ReadUInt32();
            px = br.ReadInt16();
            py = br.ReadInt16();
            pz = br.ReadInt16();

            p0 = br.ReadInt16();
            null1 = br.ReadUInt32();

            unk1 = br.ReadUInt32();

            br.BaseStream.Position += 4 * 3;

            posang = new PosAng(new Vector3s(br), new Vector3s(br));

            evt = br.ReadInt32();
        }

        public override string ToString()
        {
            return
                name +
                // "\t0x"+offsModel.ToString("X8") + 
                // "\t(" + px + ", " + py +  ", " + pz + ") " +
                // "\t" + null1 +
                "\t" + unk1 +
                "\t" + posang.ToString() +
                "\t" + ((CTREvent)evt).ToString()
                ;
        }
    }
}

