using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public class SceneHeader : IReadWrite
    {
        public static readonly int SizeOf = 0x1B0;

        public PsxPtr ptrMeshInfo;      //0x00 - pointer to MeshInfo
        public PsxPtr ptrSkybox;        //0x04 - pointer to SkyBox
        public PsxPtr ptrAnimTex;       //0x08 - pointer to animated textures array

        public int numInstances;        //0x0C - number of model instances in the level (i.e. every single box, fruit, etc.)
        public PsxPtr ptrInstances;     //0x10 - points to the 1st entry of the array of model instances
        public int numModels;           //0x14 - number of actual models
        public PsxPtr ptrModelsPtr;     //0x18 - pointer to the array of pointers to models. easy in c++, messy in c# 

        public uint unkPtr1;            //0x1C - unknown, extra visdata region
        public uint unkPtr2;            //0x20 - unknown, extra visdata region
        public PsxPtr ptrInstancesPtr;  //0x24 - pointer to the array of pointers to model instances.
        public uint unkPtr3;            //0x28 - unknown, extra visdata region

        public int null1;               //0x2C - assumed reserved
        public int null2;               //0x30 - assumed reserved

        public uint numWater;           //0x34 - number of vertices treated as water
        public PsxPtr ptrWater;         //0x38 - pointer to array of water entries
        public PsxPtr ptrIcons;         //0x3C - lead to the icon pack header
        public PsxPtr ptrIconsArray;    //0x40 - leads to the icon pack data
        public PsxPtr ptrRestartMain;   //0x44 - looks like a restart point, but doesn't affect anything? maybe like play area bbox?

        public Gradient[] glowGradients;    //0x48 - used for additional skybox gradient (like papu's pyramid) (24 bytes = 3 * (2 + 2 + 4))
        public Pose[] startGrid;        //0x6C - array of 8 starting locations (96 bytes = (6 * 2) * 8)

        public uint unkPtr4;            //0xCC - unknown, extra visdata region
        public uint unkPtr5;            //0xD0 - unknown, extra visdata region

        public PsxPtr ptrLowTexArray;   //0xD4 - assumed to be a pointer to low textures array, there is no number of entries though

        public Vector4b backColor;      //0xD8 - base background color, used to clear the screen
        public uint bgMode;             //0xDC - control background drawing mode, 1 color, 2 colors, 4 colors, gradient mode

        public PsxPtr ptrBuildStart;    //0xE0 - pointer to string, date, assumed visdata compilation start
        public PsxPtr ptrBuildEnd;      //0xE4 - pointer to string, date, assumed visdata compilation end
        public PsxPtr ptrBuildType;     //0xE8 - pointer to string, assumed build type

        byte[] skip;                    //0xEC - (7*8 = 56 bytes) assumed to be related to particles, contains particle gravity value

        public Vector4b particleColorTop;       //0x124 - controls bottom color of particles (i.e. snow)
        public Vector4b particleColorBottom;    //0x128 - controls bottom color of particles (i.e. snow)
        public uint particleRenderMode;         //0x12C - assumed to control how particles are drawn

        public uint cntTrialData;       //0x130 - that's incorrect
        public PsxPtr ptrTrialData;     //0x134 - pointer to additional data referred to as "trialdata" for now

        public uint cntu2;              //0x138
        public PsxPtr ptru2;            //0x13C

        public uint numSpawnPts;        //0x140
        public PsxPtr ptrSpawnPts;      //0x144

        public uint numRestartPts;      //0x148 - number of restart points in the level
        public PsxPtr ptrRestartPts;    //0x14C - pointer to the array of restart points (used for cameras, mask grabs, warp orbs) 

        byte[] skip2;                   //0x150 - 16 bytes

        public Vector4b[] bgColor;      //0x160 - 4 background colors used in 2/4 background color modes (16 bytes = 4 * 4)
        public uint skip2_unkPtr;       //0x170

        public uint numVcolAnim;        //0x174 - number of animated vertices data
        public PsxPtr ptrVcolAnim;      //0x178 - pointer to animated vertices data

        byte[] skip23;                  //0x17C - 12 bytes

        public PsxPtr ptrAiNav;         //0x188 - pointer to bot path data

        byte[] skip3;                   //0x18C - 36 bytes


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

            ptrMeshInfo = PsxPtr.FromReader(br);
            ptrSkybox = PsxPtr.FromReader(br);
            ptrAnimTex = PsxPtr.FromReader(br);

            numInstances = br.ReadInt32();
            ptrInstances = PsxPtr.FromReader(br);
            numModels = br.ReadInt32();
            ptrModelsPtr = PsxPtr.FromReader(br);

            unkPtr1 = br.ReadUInt32();
            unkPtr2 = br.ReadUInt32();
            ptrInstancesPtr = PsxPtr.FromReader(br);
            unkPtr3 = br.ReadUInt32();

            null1 = br.ReadInt32();
            null2 = br.ReadInt32();

            if (null1 != 0 || null2 != 0)
                Helpers.Panic(this, PanicType.Assume, "WARNING header.null1 = " + null1 + "; header.null2 = " + null2);

            numWater = br.ReadUInt32();
            ptrWater = PsxPtr.FromReader(br);
            ptrIcons = PsxPtr.FromReader(br);
            ptrIconsArray = PsxPtr.FromReader(br);

            ptrRestartMain = PsxPtr.FromReader(br);

            glowGradients = new Gradient[3];

            for (int i = 0; i < 3; i++)
                glowGradients[i] = Gradient.FromReader(br);

            startGrid = new Pose[8];

            for (int i = 0; i < 8; i++)
                startGrid[i] = Pose.FromReader(br);

            unkPtr4 = br.ReadUInt32();
            unkPtr5 = br.ReadUInt32();
            ptrLowTexArray = PsxPtr.FromReader(br);
            backColor = new Vector4b(br);

            bgMode = br.ReadUInt32();
            ptrBuildStart = PsxPtr.FromReader(br);
            ptrBuildEnd = PsxPtr.FromReader(br);
            ptrBuildType = PsxPtr.FromReader(br);

            skip = br.ReadBytes(0x38);

            particleColorTop = new Vector4b(br);
            particleColorBottom = new Vector4b(br);
            particleRenderMode = br.ReadUInt32();

            cntTrialData = br.ReadUInt32();
            ptrTrialData = PsxPtr.FromReader(br);
            cntu2 = br.ReadUInt32();
            ptru2 = PsxPtr.FromReader(br);

            numSpawnPts = br.ReadUInt32();
            ptrSpawnPts = PsxPtr.FromReader(br);

            numRestartPts = br.ReadUInt32();
            ptrRestartPts = PsxPtr.FromReader(br);

            skip2 = br.ReadBytes(16);

            bgColor = new Vector4b[4];

            for (int i = 0; i < 4; i++)
                bgColor[i] = new Vector4b(br);

            skip2_unkPtr = br.ReadUInt32();
            numVcolAnim = br.ReadUInt32();
            ptrVcolAnim = PsxPtr.FromReader(br);

            skip23 = br.ReadBytes(12);

            ptrAiNav = PsxPtr.FromReader(br);

            skip3 = br.ReadBytes(0x24);

            long dataEnd = br.BaseStream.Position;

            if (dataEnd - dataStart != SizeOf)
                throw new Exception("SceneHeader: size mismatch");



            if (ptrBuildStart != PsxPtr.Zero)
            {
                br.Jump(ptrBuildStart);
                compilationBegins = Helpers.ParseDate(br.ReadStringNT());
                Console.WriteLine(compilationBegins);
            }

            if (ptrBuildEnd != PsxPtr.Zero)
            {
                br.Jump(ptrBuildEnd);
                compilationEnds = Helpers.ParseDate(br.ReadStringNT());
                Console.WriteLine(compilationEnds);
            }

            if (ptrBuildType != PsxPtr.Zero)
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

            ptrMeshInfo.Write(bw, patchTable);
            ptrSkybox.Write(bw, patchTable);
            ptrAnimTex.Write(bw, patchTable);

            bw.Write(numInstances);
            ptrInstances.Write(bw, patchTable);
            bw.Write(numModels);
            ptrModelsPtr.Write(bw, patchTable);

            bw.Write(unkPtr1);
            bw.Write(unkPtr2);
            ptrInstancesPtr.Write(bw, patchTable);
            bw.Write(unkPtr3);

            bw.Write(null1);
            bw.Write(null2);

            bw.Write(numWater);
            ptrWater.Write(bw, patchTable);
            ptrIcons.Write(bw, patchTable);
            ptrIconsArray.Write(bw, patchTable);
            ptrRestartMain.Write(bw, patchTable);

            for (int i = 0; i < glowGradients.Length; i++)
                glowGradients[i].Write(bw);

            for (int i = 0; i < startGrid.Length; i++)
                startGrid[i].Write(bw);

            bw.Write(unkPtr4);
            bw.Write(unkPtr5);
            ptrLowTexArray.Write(bw, patchTable);
            backColor.Write(bw);
            bw.Write(bgMode);

            ptrBuildStart.Write(bw, patchTable);
            ptrBuildEnd.Write(bw, patchTable);
            ptrBuildType.Write(bw, patchTable);

            bw.Write(skip);

            particleColorTop.Write(bw);
            particleColorBottom.Write(bw);
            bw.Write(particleRenderMode);

            bw.Write(cntTrialData);
            ptrTrialData.Write(bw, patchTable);
            bw.Write(cntu2);
            ptru2.Write(bw, patchTable);
            bw.Write(numSpawnPts);
            ptrSpawnPts.Write(bw, patchTable);
            bw.Write(numRestartPts);
            ptrRestartPts.Write(bw, patchTable);

            bw.Write(skip2);

            for (int i = 0; i < bgColor.Length; i++)
                bgColor[i].Write(bw);

            bw.Write(skip2_unkPtr);

            bw.Write(numVcolAnim);
            ptrVcolAnim.Write(bw, patchTable);

            bw.Write(skip23);


            ptrAiNav.Write(bw, patchTable);

            bw.Write(skip3);

            long dataEnd = bw.BaseStream.Position;

            if (dataEnd - dataStart != SizeOf)
                throw new Exception("SceneHeader: size mismatch");
        }
    }
}
