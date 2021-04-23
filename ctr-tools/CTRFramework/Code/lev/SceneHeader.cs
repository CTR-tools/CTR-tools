using CTRFramework.Shared;
using System;

namespace CTRFramework
{
    public class SceneHeader : IRead
    {
        public uint ptrMeshInfo;    //0x0 - pointer to MeshInfo
        public uint ptrSkybox;      //0x4 - pointer to SkyBox
        public uint ptrTexArray;    //0x8 - leads to a weird array of pointers, every pointer group ends in 2 dwords - 0X0A, 0x00, those pointers lead to some array of 0x30 bytes

        public int numInstances;    //0xC - number of model instances in the level (i.e. every single box, fruit, etc.)
        public uint ptrInstances;   //0x10 - points to the 1st entry of the array of model instances
        public int numModels;       //0x14 - number of actual models
        public uint ptrModelsPtr;   //0x18 - pointer to the array of pointers to models. easy in c++, messy in c# 

        public uint unkPtr1;        //0x1C
        public uint unkPtr2;        //0x20
        public uint ptrInstancesPtr;    //0x24 - pointer to the array of pointers to model instances.
        public uint unkPtr3;        //0x28

        public int null1;           //0x2C - assumed reserved
        public int null2;           //0x30 - assumed reserved

        public uint cntWater;       //0x34 - number of vertices treated as water
        public uint ptrWater;       //0x38 - pointer to array of water entries
        public uint ptrIcons;       //0x3C - lead to the icon pack header
        public uint ptrIconsArray;  //0x40 - leads to the icon pack data
        public uint ptrRestartMain; //0x44 - looks like a restart point, but doesn't affect anything? maybe like play area bbox?

        public SomeData[] someData; //0x48 - ??? 36 bytes
        public Pose[] startGrid;    //0x6C - array of 8 starting locations (96 bytes = (6 * 2) * 8)

        public uint unkPtr4;        //0xCC
        public uint unkPtr5;        //0xD0
        public uint ptrLowTexArray; //0xD4 - assumed to be a pointer to low textures array, there is no number of entries though
        public Vector4b backColor;  //0xD8 - base background color, used to clear the screen
        public uint bgMode;         //0xDC - control background drawing mode, 1 color, 2 colors, 4 colors

        public uint ptrBuildStart;  //0xE0 - pointer to string, date, assumed visdata compilation start
        public uint ptrBuildEnd;    //0xE4 - pointer to string, date, assumed visdata compilation end
        public uint ptrBuildType;   //0xE8 - pointer to string, assumed build type

        byte[] skip;                //0xEC - (7*8 = 56 bytes) assumed to be related to particles, contains particle gravity value

        public Vector4b particleColorTop;       //0x124 - controls bottom color of particles (i.e. snow)
        public Vector4b particleColorBottom;    //0x128 - controls bottom color of particles (i.e. snow)
        public uint particleRenderMode;         //0x12C - assumed to control how particles are drawn

        public uint cntTrialData;   //0x130 - that's incorrect
        public uint ptrTrialData;   //0x134 - pointer to additional data referred to as "trialdata" for now
        public uint cntu2;          //0x138 - 
        public uint ptru2;          //0x13C
        public uint cntSpawnPts;    //0x140
        public uint ptrSpawnPts;    //0x144
        public uint cntRestartPts;  //0x148 - number of restart points in the level
        public uint ptrRestartPts;  //0x14C - points to the 1st entry in restart points array

        byte[] skip2;               //0x150 - 16 bytes

        public Vector4b[] bgColor;  //0x160 - 4 * 4 = 16 bytes for 4 background colors.
        public uint skip2_unkPtr;   //0x170 - 

        public uint cntVcolAnim;    //0x174 - number of animated vertices data
        public uint ptrVcolAnim;    //0x178 - pointer to animated vertices data

        byte[] skip23;              //0x17C - 12 bytes

        public uint ptrAiNav;       //0x188 - pointer to bot path data

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
            ptrMeshInfo = br.ReadUInt32();
            ptrSkybox = br.ReadUInt32();
            ptrTexArray = br.ReadUInt32();

            numInstances = br.ReadInt32();
            ptrInstances = br.ReadUInt32();
            numModels = br.ReadInt32();
            ptrModelsPtr = br.ReadUInt32();

            unkPtr1 = br.ReadUInt32();
            unkPtr2 = br.ReadUInt32();
            ptrInstancesPtr = br.ReadUInt32();
            unkPtr3 = br.ReadUInt32();

            null1 = br.ReadInt32();
            null2 = br.ReadInt32();

            if (null1 != 0 || null2 != 0)
                Helpers.Panic(this, PanicType.Assume, "WARNING header.null1 = " + null1 + "; header.null2 = " + null2);

            cntWater = br.ReadUInt32();
            ptrWater = br.ReadUInt32();
            ptrIcons = br.ReadUInt32();
            ptrIconsArray = br.ReadUInt32();

            ptrRestartMain = br.ReadUInt32();

            someData = new SomeData[3];

            for (int i = 0; i < 3; i++)
            {
                SomeData sd = new SomeData();
                sd.Read(br);
                someData[i] = sd;
            }

            startGrid = new Pose[8];

            for (int i = 0; i < 8; i++)
            {
                Pose pos = new Pose(br);
                startGrid[i] = pos;
                //Console.WriteLine(startGrid[i].ToString());
            }

            unkPtr4 = br.ReadUInt32();
            unkPtr5 = br.ReadUInt32();
            ptrLowTexArray = br.ReadUInt32();
            backColor = new Vector4b(br);

            bgMode = br.ReadUInt32();
            ptrBuildStart = br.ReadUInt32();
            ptrBuildEnd = br.ReadUInt32();
            ptrBuildType = br.ReadUInt32();

            skip = br.ReadBytes(0x38);

            particleColorTop = new Vector4b(br);
            particleColorBottom = new Vector4b(br);
            particleRenderMode = br.ReadUInt32();

            cntTrialData = br.ReadUInt32();
            ptrTrialData = br.ReadUInt32();
            cntu2 = br.ReadUInt32();
            ptru2 = br.ReadUInt32();

            cntSpawnPts = br.ReadUInt32();
            ptrSpawnPts = br.ReadUInt32();

            cntRestartPts = br.ReadUInt32();
            ptrRestartPts = br.ReadUInt32();

            //skip2 = br.ReadBytes(0x38);

            skip2 = br.ReadBytes(16);

            bgColor = new Vector4b[4];

            for (int i = 0; i < 4; i++)
                bgColor[i] = new Vector4b(br);

            skip2_unkPtr = br.ReadUInt32();
            cntVcolAnim = br.ReadUInt32();
            ptrVcolAnim = br.ReadUInt32();

            skip23 = br.ReadBytes(12);

            ptrAiNav = br.ReadUInt32();

            skip3 = br.ReadBytes(0x24);

            long posx = br.BaseStream.Position;

            if (ptrBuildStart != 0)
            {
                br.Jump(ptrBuildStart);
                compilationBegins = Helpers.ParseDate(br.ReadStringNT());
            }

            if (ptrBuildEnd != 0)
            {
                br.Jump(ptrBuildEnd);
                compilationEnds = Helpers.ParseDate(br.ReadStringNT());
            }

            if (ptrBuildType != 0)
            {
                br.Jump(ptrBuildType);
                Console.WriteLine(br.ReadStringNT());
            }

            br.Jump(posx);
            //Console.ReadKey();
        }

        public DateTime compilationBegins;
        public DateTime compilationEnds;


        public void Write(BinaryWriterEx bw)
        {
            bw.Write(ptrMeshInfo);
            bw.Write(ptrSkybox);
            bw.Write(ptrTexArray);

            bw.Write(numInstances);
            bw.Write(ptrInstances);
            bw.Write(numModels);
            bw.Write(ptrModelsPtr);

            bw.Write(unkPtr1);
            bw.Write(unkPtr2);
            bw.Write(ptrInstancesPtr);
            bw.Write(unkPtr3);

            bw.Write(null1);
            bw.Write(null2);

            bw.Write(cntWater);
            bw.Write(ptrWater);
            bw.Write(ptrIcons);
            bw.Write(ptrIconsArray);
            bw.Write(ptrRestartMain);

            for (int i = 0; i < someData.Length; i++)
                someData[i].Write(bw);

            for (int i = 0; i < startGrid.Length; i++)
                startGrid[i].Write(bw);

            bw.Write(unkPtr4);
            bw.Write(unkPtr5);
            bw.Write(ptrLowTexArray);
            backColor.Write(bw);
            bw.Write(bgMode);

            bw.Write(ptrBuildStart);
            bw.Write(ptrBuildEnd);
            bw.Write(ptrBuildType);

            bw.Write(skip);

            bw.Write(cntTrialData);
            bw.Write(ptrTrialData);
            bw.Write(cntu2);
            bw.Write(ptru2);
            bw.Write(cntSpawnPts);
            bw.Write(ptrSpawnPts);
            bw.Write(cntRestartPts);
            bw.Write(ptrRestartPts);

            bw.Write(skip2);

            for (int i = 0; i < bgColor.Length; i++)
                bgColor[i].Write(bw);

            bw.Write(skip2_unkPtr);

            bw.Write(cntVcolAnim);
            bw.Write(ptrVcolAnim);

            bw.Write(skip23);


            bw.Write(ptrAiNav);

            bw.Write(skip3);
        }
    }
}
