using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CTRFramework
{
    public class PickupHeader : IReadWrite
    {
        #region Component model
        public string Name
        {
            get => name;
            set => name = value;
        }

        public PsxPtr ModelOffset
        {
            get => ptrModel;
            set => ptrModel = value;
        }

        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public Pose Pose
        {
            get => pose;
            set => pose = value;
        }

        public CtrThreadID ThreadID
        {
            get => threadID;
            set => threadID = value;
        }
        #endregion

        private string name;
        private PsxPtr ptrModel;
        Vector3 scale;
        public uint null1;
        public uint unk1;
        private Pose pose;
        private CtrThreadID threadID = CtrThreadID.None;

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
            ptrModel = PsxPtr.FromReader(br);
            scale = br.ReadVector3sPadded(1 / 4096f / 16f);
            null1 = br.ReadUInt32();

            if (null1 != 0)
                Helpers.Panic(this, PanicType.Assume, $"!! null1 != 0 --> {null1}");

            if (ptrModel.ExtraBits > 0)
                Helpers.Panic(this, PanicType.Assume, $"{name}: ptrModel extrabits = {ptrModel.ExtraBits}");

            unk1 = br.ReadUInt32();

            br.Seek(4 * 3);

            pose = Pose.FromReader(br);

            threadID = (CtrThreadID)br.ReadInt32();

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

            bw.Write((int)threadID);
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
                "\t" + threadID.ToString()
                ;
        }
    }
}

