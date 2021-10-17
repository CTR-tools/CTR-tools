using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class SceneHeader : IRead
    {
        public static readonly int SizeOf = 0x1B0;

        public UIntPtr ptrMeshInfo;     //0x0 - pointer to MeshInfo
        public UIntPtr ptrSkybox;       //0x4 - pointer to SkyBox
        public UIntPtr ptrAnimTex;      //0x8 - pointer to animated textures array

        public int numInstances;        //0xC - number of model instances in the level (i.e. every single box, fruit, etc.)
        public UIntPtr ptrInstances;    //0x10 - points to the 1st entry of the array of model instances
        public int numModels;           //0x14 - number of actual models
        public UIntPtr ptrModelsPtr;    //0x18 - pointer to the array of pointers to models. easy in c++, messy in c# 

        public uint unkPtr1;            //0x1C
        public uint unkPtr2;            //0x20
        public UIntPtr ptrInstancesPtr;    //0x24 - pointer to the array of pointers to model instances.
        public uint unkPtr3;            //0x28

        public int null1;           //0x2C - assumed reserved
        public int null2;           //0x30 - assumed reserved

        public uint numWater;       //0x34 - number of vertices treated as water
        public UIntPtr ptrWater;       //0x38 - pointer to array of water entries
        public UIntPtr ptrIcons;       //0x3C - lead to the icon pack header
        public UIntPtr ptrIconsArray;  //0x40 - leads to the icon pack data
        public UIntPtr ptrRestartMain; //0x44 - looks like a restart point, but doesn't affect anything? maybe like play area bbox?

        public Gradient[] glowGradients; //0x48 - renders glowing effect in papu's pyramid
        public Pose[] startGrid;    //0x6C - array of 8 starting locations (96 bytes = (6 * 2) * 8)

        public uint unkPtr4;        //0xCC
        public uint unkPtr5;        //0xD0
        public UIntPtr ptrLowTexArray; //0xD4 - assumed to be a pointer to low textures array, there is no number of entries though
        public Vector4b backColor;  //0xD8 - base background color, used to clear the screen
        public uint bgMode;         //0xDC - control background drawing mode, 1 color, 2 colors, 4 colors

        public UIntPtr ptrBuildStart;  //0xE0 - pointer to string, date, assumed visdata compilation start
        public UIntPtr ptrBuildEnd;    //0xE4 - pointer to string, date, assumed visdata compilation end
        public UIntPtr ptrBuildType;   //0xE8 - pointer to string, assumed build type

        byte[] skip;                //0xEC - (7*8 = 56 bytes) assumed to be related to particles, contains particle gravity value

        public Vector4b particleColorTop;       //0x124 - controls bottom color of particles (i.e. snow)
        public Vector4b particleColorBottom;    //0x128 - controls bottom color of particles (i.e. snow)
        public uint particleRenderMode;         //0x12C - assumed to control how particles are drawn

        public uint cntTrialData;   //0x130 - that's incorrect
        public UIntPtr ptrTrialData;   //0x134 - pointer to additional data referred to as "trialdata" for now
        public uint cntu2;          //0x138 - 
        public uint ptru2;          //0x13C
        public uint numSpawnPts;    //0x140
        public UIntPtr ptrSpawnPts;    //0x144
        public uint numRestartPts;  //0x148 - number of restart points in the level
        public UIntPtr ptrRestartPts;  //0x14C - points to the 1st entry in restart points array

        byte[] skip2;               //0x150 - 16 bytes

        public Vector4b[] bgColor;  //0x160 - 4 * 4 = 16 bytes for 4 background colors.
        public uint skip2_unkPtr;   //0x170 - 

        public uint numVcolAnim;    //0x174 - number of animated vertices data
        public UIntPtr ptrVcolAnim;    //0x178 - pointer to animated vertices data

        byte[] skip23;              //0x17C - 12 bytes

        public UIntPtr ptrAiNav;       //0x188 - pointer to bot path data

        byte[] skip3;               //0x18C - 36 bytes


        public SceneHeader()
        {
        }

        public SceneHeader(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            int dataStart = (int)br.BaseStream.Position;

            ptrMeshInfo = br.ReadUIntPtr();
            ptrSkybox = br.ReadUIntPtr();
            ptrAnimTex = br.ReadUIntPtr();

            numInstances = br.ReadInt32();
            ptrInstances = br.ReadUIntPtr();
            numModels = br.ReadInt32();
            ptrModelsPtr = br.ReadUIntPtr();

            unkPtr1 = br.ReadUInt32();
            unkPtr2 = br.ReadUInt32();
            ptrInstancesPtr = br.ReadUIntPtr();
            unkPtr3 = br.ReadUInt32();

            null1 = br.ReadInt32();
            null2 = br.ReadInt32();

            if (null1 != 0 || null2 != 0)
                Helpers.Panic(this, PanicType.Assume, "WARNING header.null1 = " + null1 + "; header.null2 = " + null2);

            numWater = br.ReadUInt32();
            ptrWater = br.ReadUIntPtr();
            ptrIcons = br.ReadUIntPtr();
            ptrIconsArray = br.ReadUIntPtr();

            ptrRestartMain = br.ReadUIntPtr();

            glowGradients = new Gradient[3];

            for (int i = 0; i < 3; i++)
                glowGradients[i] = Gradient.FromReader(br);

            startGrid = new Pose[8];

            for (int i = 0; i < 8; i++)
                startGrid[i] = Pose.FromReader(br);

            unkPtr4 = br.ReadUInt32();
            unkPtr5 = br.ReadUInt32();
            ptrLowTexArray = br.ReadUIntPtr();
            backColor = new Vector4b(br);

            bgMode = br.ReadUInt32();
            ptrBuildStart = br.ReadUIntPtr();
            ptrBuildEnd = br.ReadUIntPtr();
            ptrBuildType = br.ReadUIntPtr();

            skip = br.ReadBytes(0x38);

            particleColorTop = new Vector4b(br);
            particleColorBottom = new Vector4b(br);
            particleRenderMode = br.ReadUInt32();

            cntTrialData = br.ReadUInt32();
            ptrTrialData = br.ReadUIntPtr();
            cntu2 = br.ReadUInt32();
            ptru2 = br.ReadUInt32();

            numSpawnPts = br.ReadUInt32();
            ptrSpawnPts = br.ReadUIntPtr();

            numRestartPts = br.ReadUInt32();
            ptrRestartPts = br.ReadUIntPtr();

            skip2 = br.ReadBytes(16);

            bgColor = new Vector4b[4];

            for (int i = 0; i < 4; i++)
                bgColor[i] = new Vector4b(br);

            skip2_unkPtr = br.ReadUInt32();
            numVcolAnim = br.ReadUInt32();
            ptrVcolAnim = br.ReadUIntPtr();

            skip23 = br.ReadBytes(12);

            ptrAiNav = br.ReadUIntPtr();

            skip3 = br.ReadBytes(0x24);

            long dataEnd = br.BaseStream.Position;

            if (dataEnd - dataStart != SizeOf)
                throw new Exception("SceneHeader: size mismatch");



            if (ptrBuildStart != UIntPtr.Zero)
            {
                br.Jump(ptrBuildStart);
                compilationBegins = Helpers.ParseDate(br.ReadStringNT());
                Console.WriteLine(compilationBegins);
            }

            if (ptrBuildEnd != UIntPtr.Zero)
            {
                br.Jump(ptrBuildEnd);
                compilationEnds = Helpers.ParseDate(br.ReadStringNT());
                Console.WriteLine(compilationEnds);
            }

            if (ptrBuildType != UIntPtr.Zero)
            {
                br.Jump(ptrBuildType);
                Console.WriteLine(br.ReadStringNT());
            }

            br.Jump(dataEnd);
        }

        public DateTime compilationBegins;
        public DateTime compilationEnds;


        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            long dataStart = bw.BaseStream.Position;

            bw.Write(ptrMeshInfo, patchTable);
            bw.Write(ptrSkybox, patchTable);
            bw.Write(ptrAnimTex, patchTable);

            bw.Write(numInstances);
            bw.Write(ptrInstances, patchTable);
            bw.Write(numModels);
            bw.Write(ptrModelsPtr, patchTable);

            bw.Write(unkPtr1);
            bw.Write(unkPtr2);
            bw.Write(ptrInstancesPtr, patchTable);
            bw.Write(unkPtr3);

            bw.Write(null1);
            bw.Write(null2);

            bw.Write(numWater);
            bw.Write(ptrWater, patchTable);
            bw.Write(ptrIcons, patchTable);
            bw.Write(ptrIconsArray, patchTable);
            bw.Write(ptrRestartMain, patchTable);

            for (int i = 0; i < glowGradients.Length; i++)
                glowGradients[i].Write(bw);

            for (int i = 0; i < startGrid.Length; i++)
                startGrid[i].Write(bw);

            bw.Write(unkPtr4);
            bw.Write(unkPtr5);
            bw.Write(ptrLowTexArray, patchTable);
            backColor.Write(bw);
            bw.Write(bgMode);

            bw.Write(ptrBuildStart, patchTable);
            bw.Write(ptrBuildEnd, patchTable);
            bw.Write(ptrBuildType, patchTable);

            bw.Write(skip);

            particleColorTop.Write(bw);
            particleColorBottom.Write(bw);
            bw.Write(particleRenderMode);

            bw.Write(cntTrialData);
            bw.Write(ptrTrialData, patchTable);
            bw.Write(cntu2);
            bw.Write(ptru2);
            bw.Write(numSpawnPts);
            bw.Write(ptrSpawnPts, patchTable);
            bw.Write(numRestartPts);
            bw.Write(ptrRestartPts, patchTable);

            bw.Write(skip2);

            for (int i = 0; i < bgColor.Length; i++)
                bgColor[i].Write(bw);

            bw.Write(skip2_unkPtr);

            bw.Write(numVcolAnim);
            bw.Write(ptrVcolAnim, patchTable);

            bw.Write(skip23);


            bw.Write(ptrAiNav, patchTable);

            bw.Write(skip3);

            long dataEnd = bw.BaseStream.Position;

            if (dataEnd - dataStart != SizeOf)
                throw new Exception("SceneHeader: size mismatch");
        }
    }
}
