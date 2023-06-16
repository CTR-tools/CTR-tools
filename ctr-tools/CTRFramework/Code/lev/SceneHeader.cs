using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    [Flags]
    public enum SceneFlags
    {
        GradientSky = 1 << 0,
        KillPlane = 1 << 1,
        AnimVerts = 1 << 2
    }

    public class SceneHeader : IReadWrite
    {
        public static readonly int SizeOf = 0x1B0;

        public PsxPtr ptrMeshInfo = PsxPtr.Zero;      //0x00 - pointer to MeshInfo
        public PsxPtr ptrSkybox = PsxPtr.Zero;        //0x04 - pointer to SkyBox
        public PsxPtr ptrAnimTex = PsxPtr.Zero;       //0x08 - pointer to animated textures array

        public int numInstances;        //0x0C - number of model instances in the level (i.e. every single box, fruit, etc.)
        public PsxPtr ptrInstances = PsxPtr.Zero;     //0x10 - points to the 1st entry of the array of model instances
        public int numModels;           //0x14 - number of actual models
        public PsxPtr ptrModelsPtr = PsxPtr.Zero;     //0x18 - pointer to the array of pointers to models. easy in c++, messy in c# 

        public uint unkPtr1;            //0x1C - unknown, extra visdata region
        public uint unkPtr2;            //0x20 - unknown, extra visdata region
        public PsxPtr ptrInstancesPtr = PsxPtr.Zero;  //0x24 - pointer to the array of pointers to model instances.
        public uint unkPtr3;            //0x28 - unknown, extra visdata region

        public int null1;               //0x2C - assumed reserved
        public int null2;               //0x30 - assumed reserved

        public uint numWater;           //0x34 - number of vertices treated as water
        public PsxPtr ptrWater = PsxPtr.Zero;         //0x38 - pointer to array of water entries
        public PsxPtr ptrIcons = PsxPtr.Zero;         //0x3C - lead to the icon pack header
        public PsxPtr ptrIconsArray = PsxPtr.Zero;    //0x40 - leads to the icon pack data
        public PsxPtr ptrEnviroMap = PsxPtr.Zero;     //0x44 - pointer to environment map, used by water rendering

        public Gradient[] glowGradients = new Gradient[3];    //0x48 - used for additional skybox gradient (like papu's pyramid) (24 bytes = 3 * (2 + 2 + 4))
        public Pose[] startGrid;        //0x6C - array of 8 starting locations (96 bytes = (6 * 2) * 8)

        public uint unkPtr4;            //0xCC - unknown, extra visdata region
        public uint unkPtr5;            //0xD0 - unknown, extra visdata region

        public PsxPtr ptrLowTexArray = PsxPtr.Zero;   //0xD4 - assumed to be a pointer to low textures array, there is no number of entries though

        public Vector4b backColor = new Vector4b(0,0,0,0);      //0xD8 - base background color, used to clear the screen
        public SceneFlags sceneFlags;   //0xDC - this actually toggles some render stuff, bit0 - gradient sky, bit1 - ???, bit2 - toggles between water and animated vertices?

        public PsxPtr ptrBuildStart = PsxPtr.Zero;    //0xE0 - pointer to string, date, assumed visdata compilation start
        public PsxPtr ptrBuildEnd = PsxPtr.Zero;      //0xE4 - pointer to string, date, assumed visdata compilation end
        public PsxPtr ptrBuildType = PsxPtr.Zero;     //0xE8 - pointer to string, assumed build type

        byte[] skip;                    //0xEC - (7*8 = 56 bytes) assumed to be related to particles, contains particle gravity value

        public Vector4b particleColorTop = new Vector4b(0, 0, 0, 0);       //0x124 - controls bottom color of particles (i.e. snow)
        public Vector4b particleColorBottom = new Vector4b(0, 0, 0, 0);    //0x128 - controls bottom color of particles (i.e. snow)
        public uint particleRenderMode;         //0x12C - assumed to control how particles are drawn

        public uint cntTrialData;       //0x130 - that's incorrect
        public PsxPtr ptrTrialData = PsxPtr.Zero;     //0x134 - pointer to additional data referred to as "trialdata" for now

        public uint cntu2;              //0x138
        public PsxPtr ptru2 = PsxPtr.Zero;            //0x13C

        public uint numSpawnGroups;     //0x140 - number of spawn point groups, mainly used in adventure mode, also used for hazard paths
        public PsxPtr ptrSpawnGroups = PsxPtr.Zero;   //0x144 - pointer to the spawn point groups struct

        public uint numRespawnPts;      //0x148 - number of restart points in the level
        public PsxPtr ptrRespawnPts = PsxPtr.Zero;    //0x14C - pointer to the array of restart points (used for cameras, mask grabs, warp orbs) 

        byte[] skip2;                   //0x150 - 16 bytes

        public Vector4b bgColorTop = new Vector4b(0, 0, 0, 0);     //0x160 - top background color, last byte - usage bool, if == 0 - dont fill, same for colors below
        public Vector4b bgColorBottom = new Vector4b(0, 0, 0, 0);  //0x164 - bottom background color
        public Vector4b gradColorTop = new Vector4b(0, 0, 0, 0);   //0x168 - some color used in sky gradient rendering, kinda replaces top color if grad is used
        public Vector4b gradColorBottom = new Vector4b(0, 0, 0, 0);//0x16C - not sure, but maybe

        public uint skip2_unkPtr;       //0x170

        public uint numVcolAnim;        //0x174 - number of animated vertices data
        public PsxPtr ptrVcolAnim = PsxPtr.Zero;      //0x178 - pointer to animated vertices data

        public ushort numStars;         //0x17C - amount of stars to generate
        public ushort unkStarsBool;     //0x17E - some stars related bool
        public ushort unkStarsFlags;    //0x180 - some stars flags
        public ushort starsDepth;       //0x182 - defines OT position to draw for correct depth

        public ushort unkAfterStars;    //0x184 - ever not null?
        public ushort waterLevel;       //0x186 - defines split height for reflections

        public PsxPtr ptrNavData = PsxPtr.Zero;       //0x188 - pointer to bot path data

        byte[] skip3;                   //0x18C - 36 bytes


        public SceneHeader()
        {
        }

        public SceneHeader(BinaryReaderEx br) => Read(br);

        public static SceneHeader FromReader(BinaryReaderEx br) => new SceneHeader(br);

        public void Read(BinaryReaderEx br)
        {
            int dataStart = (int)br.Position;

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

            ptrEnviroMap = PsxPtr.FromReader(br);

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

            sceneFlags = (SceneFlags)br.ReadUInt32();
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

            numSpawnGroups = br.ReadUInt32();
            ptrSpawnGroups = PsxPtr.FromReader(br);

            numRespawnPts = br.ReadUInt32();
            ptrRespawnPts = PsxPtr.FromReader(br);

            skip2 = br.ReadBytes(4 * 4);

            bgColorTop = new Vector4b(br);
            bgColorBottom = new Vector4b(br);
            gradColorTop = new Vector4b(br);
            gradColorBottom = new Vector4b(br);

            skip2_unkPtr = br.ReadUInt32();
            numVcolAnim = br.ReadUInt32();
            ptrVcolAnim = PsxPtr.FromReader(br);

            numStars = br.ReadUInt16();
            unkStarsBool = br.ReadUInt16();
            unkStarsFlags = br.ReadUInt16();
            starsDepth = br.ReadUInt16();

            unkAfterStars = br.ReadUInt16();
            waterLevel = br.ReadUInt16();

            ptrNavData = PsxPtr.FromReader(br);

            skip3 = br.ReadBytes(0x24);


            long dataEnd = br.Position;

            if (dataEnd - dataStart != SizeOf)
                throw new Exception("SceneHeader: size mismatch");
     
            if (ptrBuildStart != PsxPtr.Zero)
            {
                br.Jump(ptrBuildStart);
                compilationBegins = Helpers.ParseDate(br.ReadStringNT());
                Helpers.Panic(this, PanicType.Info, compilationBegins.ToString());
            }

            if (ptrBuildEnd != PsxPtr.Zero)
            {
                br.Jump(ptrBuildEnd);
                compilationEnds = Helpers.ParseDate(br.ReadStringNT());
                Helpers.Panic(this, PanicType.Info, compilationEnds.ToString());
            }

            if (ptrBuildType != PsxPtr.Zero)
            {
                br.Jump(ptrBuildType);
                Helpers.Panic(this, PanicType.Info, br.ReadStringNT());
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
            ptrEnviroMap.Write(bw, patchTable);

            for (int i = 0; i < glowGradients.Length; i++)
                glowGradients[i].Write(bw);

            for (int i = 0; i < startGrid.Length; i++)
                startGrid[i].Write(bw);

            bw.Write(unkPtr4);
            bw.Write(unkPtr5);
            ptrLowTexArray.Write(bw, patchTable);
            backColor.Write(bw);
            bw.Write((uint)sceneFlags);

            ptrBuildStart.Write(bw, patchTable);
            ptrBuildEnd.Write(bw, patchTable);
            ptrBuildType.Write(bw, patchTable);


            //bw.Write(skip);
            bw.Seek(0x38); //this must coincide with the skip amount in read!

            particleColorTop.Write(bw);
            particleColorBottom.Write(bw);
            bw.Write(particleRenderMode);

            bw.Write(cntTrialData);
            ptrTrialData.Write(bw, patchTable);
            bw.Write(cntu2);
            ptru2.Write(bw, patchTable);
            bw.Write(numSpawnGroups);
            ptrSpawnGroups.Write(bw, patchTable);
            bw.Write(numRespawnPts);
            ptrRespawnPts.Write(bw, patchTable);


            //bw.Write(skip2);
            bw.Seek(4*4); //this must coincide with the skip amount in read!

            bgColorTop.Write(bw);
            bgColorBottom.Write(bw);
            gradColorTop.Write(bw);
            gradColorBottom.Write(bw);

            bw.Write(skip2_unkPtr);

            bw.Write(numVcolAnim);
            ptrVcolAnim.Write(bw, patchTable);

            bw.Write(numStars);
            bw.Write(unkStarsBool);
            bw.Write(unkStarsFlags);
            bw.Write(starsDepth);

            bw.Write(unkAfterStars);
            bw.Write(waterLevel);

            ptrNavData.Write(bw, patchTable);

            //bw.Write(skip3);
            bw.Seek(0x24); //this must coincide with the skip amount in read!

            long dataEnd = bw.BaseStream.Position;

            if (dataEnd - dataStart != SizeOf)
                throw new Exception("SceneHeader: size mismatch");
        }
    }
}
