using CTRFramework.Shared;
using System;
using System.ComponentModel;

namespace CTRFramework
{
    public class PickupHeader
    {

        [CategoryAttribute("General"), DescriptionAttribute("Name of the model.")]
        public string Name
        {
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


        public string ModelName;

        public PickupHeader(BinaryReaderEx br)
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

            int x = (int)br.BaseStream.Position;

            br.Jump(modelOffset);

            ModelName = br.ReadStringFixed(16);

            br.BaseStream.Position = x;
        }

        public void Write(BinaryWriterEx bw)
        {
            bw.Write(System.Text.Encoding.ASCII.GetBytes(name));
            for (int i = 0; i < 16 - name.Length; i++) bw.Write((byte)0);
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

