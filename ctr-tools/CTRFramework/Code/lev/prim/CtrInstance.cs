using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CTRFramework
{
    public class CtrInstance : IReadWrite
    {
        //instance should hold a pointer to model class, they're separate now

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

        public CtrInstance(BinaryReaderEx br) => Read(br);

        public static CtrInstance FromReader(BinaryReaderEx br) => new CtrInstance(br);

        public void Read(BinaryReaderEx br)
        {
            name = br.ReadStringFixed(16);
            ptrModel = PsxPtr.FromReader(br);
            scale = br.ReadVector3sPadded(Helpers.GteScaleSmall, true);

            //duh
            if (name.Contains("plant") || name.Contains("seal") || name.Contains("spider")) scale *= 2;

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
            //duh
            if (name.Contains("plant") || name.Contains("seal") || name.Contains("spider")) scale /= 2;

            int pos = (int)bw.BaseStream.Position;
            bw.Write(name.ToCharArray().Take(16).ToArray());
            bw.Jump(pos + 16);
            bw.Write(ptrModel, patchTable);

            bw.WriteVector3s(scale, 1 / 4096f / 16f, VectorPadding.Yes);

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

