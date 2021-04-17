using CTRFramework.Shared;
using System;

namespace CTRFramework
{
    public class SceneHeader : IRead
    {
        public uint ptrMeshInfo;    //pointer to MeshInfo
        public uint ptrSkybox;      //pointer to SkyBox
        public uint ptrTexArray;    //leads to a weird array of pointers, every pointer group ends in 2 dwords - 0X0A, 0x00, those pointers lead to some array of 0x30 bytes

        public int numInstances;    //number of model instances in the level (i.e. every single box, fruit, etc.)
        public uint ptrInstances;   //points to the 1st entry of the array of model instances
        public int numModels;       //number of actual models
        public uint ptrModelsPtr;   //pointer to the array of pointers to models. easy in c++, messy in c# 

        public uint unkPtr1;
        public uint unkPtr2;
        public uint ptrInstancesPtr;    //pointer to the array of pointers to model instances.
        public uint unkPtr3;

        public int null1;           //assumed reserved
        public int null2;           //assumed reserved

        public uint cntWater;       //number of vertices treated as water
        public uint ptrWater;       //pointer to array of water entries
        public uint ptrIcons;    //lead to the header for the data below
        public uint ptrNamedTexArray; //leads to some named data (drop, bubble, map-asphalt01) with an array of 0x0C bytes afterwards
        public uint ptrRestartMain;

        public SomeData[] someData; //???
        public Pose[] startGrid;    //array of 8 starting locations

        public uint unkPtr4;
        public uint unkPtr5;
        public uint ptrLowTexArray; //assumed to be a pointer to low textures array, there is no number of entries though
        public Vector4b backColor;  //base background color, used to clear the screen
        public uint bgMode;         //control background drawing mode, 1 color, 2 colors, 4 colors

        public uint ptrBuildStart;  //pointer to string, date, assumed visdata compilation start
        public uint ptrBuildEnd;    //pointer to string, date, assumed visdata compilation end
        public uint ptrBuildType;   //pointer to string, assumed build type

        byte[] skip;    //assumed to be related to particles, contains particle gravity value

        public Vector4b particleColorTop;       //controls bottom color of particles (i.e. snow)
        public Vector4b particleColorBottom;    //controls bottom color of particles (i.e. snow)
        public uint particleRenderMode; //assumed to control how particles are drawn

        public uint cntTrialData; //that's incorrect
        public uint ptrTrialData;
        public uint cntu2;
        public uint ptru2;
        public uint cntSpawnPts;
        public uint ptrSpawnPts;
        public uint cntRestartPts;      //number of restarts points in the level
        public uint ptrRestartPts;      //points to the 1st entry in restart points array

        byte[] skip2;

        public Vector4b[] bgColor;
        public uint skip2_unkPtr;

        public uint cntVcolAnim;
        public uint ptrVcolAnim;

        byte[] skip23;


        public uint ptrAiNav;

        byte[] skip3;


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
            ptrNamedTexArray = br.ReadUInt32();

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
            bw.Write(ptrNamedTexArray);
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
