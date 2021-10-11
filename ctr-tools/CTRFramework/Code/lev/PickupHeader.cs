using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace CTRFramework
{
    public class PickupHeader : IRead, IWrite
    {
        #region Component model
        [CategoryAttribute("General"), DescriptionAttribute("Name of the instance.")]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [CategoryAttribute("General"), DescriptionAttribute("Pointer to model.")]
        public UIntPtr ModelOffset
        {
            get => ptrModel;
            set => ptrModel = value;
        }

        [CategoryAttribute("Spacing"), DescriptionAttribute("Scale of the instance."), TypeConverter(typeof(ExpandableObjectConverter))]
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        [CategoryAttribute("Spacing"), DescriptionAttribute("Pose of the instance."), TypeConverter(typeof(ExpandableObjectConverter))]
        public Pose Pose
        {
            get => pose;
            set => pose = value;
        }

        [CategoryAttribute("Settings"), DescriptionAttribute("Assigned event.")]
        public CTREvent Event
        {
            get => (CTREvent)evt;
            set => evt = (int)value;
        }
        #endregion

        private string name;
        private UIntPtr ptrModel;
        Vector3 scale;
        public uint null1;
        public uint unk1;
        private Pose pose;

        //event type?
        private int evt;


        public string ModelName;

        public PickupHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public static PickupHeader FromReader(BinaryReaderEx br)
        {
            return new PickupHeader(br);
        }

        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            ptrModel = br.ReadUIntPtr();
            scale = br.ReadVector3sPadded(1 / 4096f / 16f);
            null1 = br.ReadUInt32();

            if (null1 != 0)
                Helpers.Panic(this, PanicType.Assume, $"!! null1 != 0 --> {null1}");

            unk1 = br.ReadUInt32();

            br.BaseStream.Position += 4 * 3;

            pose = Pose.FromReader(br);

            evt = br.ReadInt32();

            ModelName = br.ReadFixedStringPtr(ptrModel, 16);
        }

        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            bw.Write(System.Text.Encoding.ASCII.GetBytes(name));
            for (int i = 0; i < 16 - name.Length; i++) bw.Write((byte)0);

            bw.Write(ptrModel, patchTable);

            bw.WriteVector3sPadded(scale, 1 / 4096f / 16f);

            bw.Write(null1);
            bw.Write(unk1);

            bw.Write((int)0);
            bw.Write((int)0);
            bw.Write((int)0);

            pose.Write(bw);

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
                "\t" + pose.ToString() +
                "\t" + ((CTREvent)evt).ToString()
                ;
        }
    }
}

