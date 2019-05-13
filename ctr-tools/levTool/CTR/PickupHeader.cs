using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace CTRtools
{

    public class PickupHeader
    {

        private string name;
        private uint modelOffset;

        //maybe scale
        Vector4s scale;

        public uint null1;

        public uint unk1;

        private Vector3s position;
        private Vector3s angle;

        //public PosAng posang;

        //event type?
        private int evt;



        [CategoryAttribute("General"), DescriptionAttribute("Name of the model.")]  
        public string Name {
            get { return name; }
            set { name = value; }
        }

        [CategoryAttribute("General"), DescriptionAttribute("Mesh offset.")]
        public uint ModelOffset
        {
            get { return modelOffset; }
            set { modelOffset = value; }
        }

        [CategoryAttribute("Spacing"), DescriptionAttribute("Scale of the model."), TypeConverter(typeof(ExpandableObjectConverter))]
        public Vector4s Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        [CategoryAttribute("Spacing"), DescriptionAttribute("Angles of the model."), TypeConverter(typeof(ExpandableObjectConverter))]
        public Vector3s Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        [CategoryAttribute("Spacing"), DescriptionAttribute("Position of the model."), TypeConverter(typeof(ExpandableObjectConverter))]
        public Vector3s Position
        {
            get { return position; }
            set { position = value; }
        }


        [CategoryAttribute("Settings"), DescriptionAttribute("Assigned event.")]
        public CTREvent Event
        {
            get { return (CTREvent)evt; }
            set { evt = (int)value; }
        }

        public enum CTREvent
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
            modelOffset = br.ReadUInt32();

            scale = new Vector4s(br);

            if (scale.W != 0) Console.WriteLine("!! scale.W != 0 !! W = " + scale.W);

            null1 = br.ReadUInt32();

            if (null1 != 0) Console.WriteLine("!! null != 0 !! null1 = " + null1);

            unk1 = br.ReadUInt32();

            br.BaseStream.Position += 4 * 3;

            position = new Vector3s(br);
            angle = new Vector3s(br);

            evt = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(System.Text.Encoding.ASCII.GetBytes(name));
            for (int i = 0; i < 16-name.Length; i++) bw.Write((byte)0);
            bw.Write(modelOffset);

            scale.Write(bw);

            bw.Write(null1);
            bw.Write(unk1);

            bw.Write((int)0);
            bw.Write((int)0);
            bw.Write((int)0);

            position.Write(bw);
            angle.Write(bw);

            bw.Write(evt);
        }

        public override string ToString()
        {
            return
                name +
                // "\t0x"+offsModel.ToString("X8") + 
                // "\t(" + px + ", " + py +  ", " + pz + ") " +
                // "\t" + null1 +
                "\t" + unk1 +
                "\t" + position.ToString() +
                "\t" + angle.ToString() +
                "\t" + ((CTREvent)evt).ToString()
                ;
        }
    }
}

