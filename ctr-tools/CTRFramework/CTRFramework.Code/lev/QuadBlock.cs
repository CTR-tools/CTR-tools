using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework
{
    public struct FaceFlags
    {
        public RotateFlipType rotateFlipType;
        public FaceMode faceMode;

        public FaceFlags(byte x)
        {
            rotateFlipType = (RotateFlipType)(x & 7);
            faceMode = (FaceMode)(x >> 3 & 3);
        }
    }



    public class QuadBlock : IReadWrite
    {
        /*
         * 0--4--1
         * | /| /|
         * |/ |/ |
         * 5--6--7
         * | /| /|
         * |/ |/ |
         * 2--8--3
         */

        public long pos;

        //9 indices in vertex array, that form 4 quads, see above.
        public short[] ind = new short[9];
        public QuadFlags quadFlags;

        public uint bitvalue; //important! big endian!

        //these values are contained in bitvalue, mask is 8b5b5b5b5b4z where b is bit and z is empty. or is it?
        public byte drawOrderLow;
        public FaceFlags[] faceFlags = new FaceFlags[4];
        public uint extradata;

        public byte[] drawOrderHigh = new byte[4];

        public uint[] ptrTexMid = new uint[4];    //offsets to mid texture definition

        public BoundingBox bb;              //a box that bounds

        public TerrainFlags terrainFlag;
        public byte WeatherIntensity;
        public byte WeatherType;
        public byte TerrainFlagUnknown;

        public short id;
        public byte trackPos;
        public byte midunk;

        //public byte[] midflags = new byte[2];

        public uint ptrTexLow;                 //offset to LOD texture definition
        public uint mosaicStruct;

        public uint mosaicPtr1;
        public uint mosaicPtr2;
        public uint mosaicPtr3;
        public uint mosaicPtr4;

        public List<Vector2s> unk3 = new List<Vector2s>();  //unknown

        //additional data
        public TextureLayout texlow;

        public List<CtrTex> tex = new List<CtrTex>();


        public bool isWater = false;

        public QuadBlock()
        {
        }

        public QuadBlock(BinaryReaderEx br)
        {
            Read(br);
        }


        public void Read(BinaryReaderEx br)
        {
            pos = br.BaseStream.Position;

            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            quadFlags = (QuadFlags)br.ReadUInt16();

            bitvalue = br.ReadUInt32Big();
            {
                drawOrderLow = (byte)(bitvalue & 0xFF);

                for (int i = 0; i < 4; i++)
                {
                    byte val = (byte)((bitvalue >> 8 + 5 * i) & 0x1F);
                    faceFlags[i] = new FaceFlags(val);
                }

                //extradata = (byte)(bitvalue & 0xF0000000 >> 28);
                extradata = (bitvalue & 0xFFF);
            }

            drawOrderHigh = br.ReadBytes(4);

            for (int i = 0; i < 4; i++)
            {
                ptrTexMid[i] = br.ReadUInt32();

                if (Helpers.TestPointer(ptrTexMid[i]) != 0)
                {
                    Console.WriteLine("mid " + Helpers.TestPointer(ptrTexMid[i]).ToString("x2"));
                   // Console.ReadKey();
                }
            }



            bb = new BoundingBox(br);

            byte tf = br.ReadByte();

            if (tf > 20)
                Helpers.Panic(this, "unexpected terrain flag value -> " + tf);

            terrainFlag = (TerrainFlags)tf;
            WeatherIntensity = br.ReadByte();
            WeatherType = br.ReadByte();
            TerrainFlagUnknown = br.ReadByte();

            id = br.ReadInt16();

            trackPos = br.ReadByte();
            midunk = br.ReadByte();

            //midflags = br.ReadBytes(2);

            ptrTexLow = br.ReadUInt32();

            if (Helpers.TestPointer(ptrTexLow) != 0)
            {
                Console.WriteLine("ptrTexLow " + Helpers.TestPointer(ptrTexLow).ToString("x2"));
                //Console.ReadKey();
            }

            mosaicStruct = br.ReadUInt32();

            if (Helpers.TestPointer(mosaicStruct) != 0)
            {
                Console.WriteLine("offset2 " + Helpers.TestPointer(mosaicStruct).ToString("x2"));
                //Console.ReadKey();
            }

            for (int i = 0; i < 5; i++)
                unk3.Add(new Vector2s(br));



            /*
            //this is some value per tirangle
            foreach(var val in unk3)
            {
                Console.WriteLine(val.X / 4096f + " " + val.Y / 4096f);
            }
            */

            //struct done

            //read texture layouts
            int texpos = (int)br.BaseStream.Position;

            br.Jump(ptrTexLow);
            texlow = TextureLayout.FromStream(br);


            foreach (uint u in ptrTexMid)
            {
                if (u != 0)
                {
                    br.Jump(u);
                    tex.Add(new CtrTex(br));
                }
                else
                {
                    if (ptrTexLow != 0) Console.WriteLine("!");
                }
            }

            if (mosaicStruct != 0)
            {
                br.BaseStream.Position = mosaicStruct;

                mosaicPtr1 = br.ReadUInt32();
                mosaicPtr2 = br.ReadUInt32();
                mosaicPtr3 = br.ReadUInt32();
                mosaicPtr4 = br.ReadUInt32();
            }

            br.BaseStream.Position = texpos;
        }

        public void RecalcBB(List<Vertex> vert)
        {
            BoundingBox bb_new = new BoundingBox();

            foreach (int i in ind)
            {
                if (vert[i].coord.X < bb_new.Min.X) bb_new.Min.X = vert[i].coord.X;
                if (vert[i].coord.X > bb_new.Max.X) bb_new.Max.X = vert[i].coord.X;

                if (vert[i].coord.Y < bb_new.Min.Y) bb_new.Min.Y = vert[i].coord.Y;
                if (vert[i].coord.Y > bb_new.Max.Y) bb_new.Max.Y = vert[i].coord.Y;

                if (vert[i].coord.Z < bb_new.Min.Z) bb_new.Min.Z = vert[i].coord.Z;
                if (vert[i].coord.Z > bb_new.Max.Z) bb_new.Max.Z = vert[i].coord.Z;
            }
        }

        //magic array of indices, each line contains 2 quads
        int[] inds = new int[]
        {
            6, 5, 1,
            6, 7, 5,
            7, 2, 5,
            7, 8, 2,
            3, 7, 6,
            3, 9, 7,
            9, 8, 7,
            9, 4, 8
        };

        /*
        //magic array of indices, each line contains 2 quads
        int[] inds = new int[]
        {
            1, 6, 5, 5, 6, 7,
            5, 7, 2, 2, 7, 8,
            6, 3, 7, 7, 3, 9,
            7, 9, 8, 8, 9, 4
        };
        */

        public List<CTRFramework.Vertex> GetVertexList(Scene s)
        {
            List<CTRFramework.Vertex> buf = new List<CTRFramework.Vertex>();

            for (int i = inds.Length - 1; i >= 0; i--)
                buf.Add(s.verts[ind[inds[i] - 1]]);

            for (int i = 0; i < inds.Length / 6; i++)
            {
                buf[i * 6 + 0].uv = new Vector2b(0, 1);
                buf[i * 6 + 1].uv = new Vector2b(1, 0);
                buf[i * 6 + 2].uv = new Vector2b(0, 0);
                buf[i * 6 + 3].uv = new Vector2b(0, 1);
                buf[i * 6 + 4].uv = new Vector2b(1, 1);
                buf[i * 6 + 5].uv = new Vector2b(1, 0);
            }

            return buf;
        }


        //use this later for obj export too
        public List<CTRFramework.Vertex> GetVertexListq(List<Vertex> v, int i)
        {
            try
            {
                List<CTRFramework.Vertex> buf = new List<CTRFramework.Vertex>();

                if (i == -1)
                {
                    int[] arrind = new int[] { 0, 1, 2, 3 };

                    for (int j = 0; j < 4; j++)
                        buf.Add(v[ind[arrind[j]]]);

                    for (int j = 0; j < 4; j++)
                    {
                        buf[j].uv = texlow.normuv[j];
                    }

                    if (buf.Count != 4)
                    {
                        Helpers.Panic(this, "not a quad! " + buf.Count);
                        Console.ReadKey();
                    }

                    return buf;
                }
                else
                {
                    int[] arrind;
                    int[] uvinds;

                    switch (faceFlags[i].rotateFlipType)
                    {
                        case RotateFlipType.None: uvinds = GetUVIndices2(1, 2, 3, 4); break;
                        case RotateFlipType.Rotate90: uvinds = GetUVIndices2(3, 1, 4, 2); break;
                        case RotateFlipType.Rotate180: uvinds = GetUVIndices2(4, 3, 2, 1); break;
                        case RotateFlipType.Rotate270: uvinds = GetUVIndices2(2, 4, 1, 3); break;
                        case RotateFlipType.Flip: uvinds = GetUVIndices2(2, 1, 4, 3); break;
                        case RotateFlipType.FlipRotate90: uvinds = GetUVIndices2(4, 2, 3, 1); break;
                        case RotateFlipType.FlipRotate180: uvinds = GetUVIndices2(3, 4, 1, 2); break;
                        case RotateFlipType.FlipRotate270: uvinds = GetUVIndices2(1, 3, 2, 4); break;
                        default: throw new Exception("Impossible rotatefliptype.");
                    }


                    switch (faceFlags[i].faceMode)
                    {
                        case FaceMode.SingleUV1:
                            {
                                uvinds = new int[] { uvinds[2], uvinds[0], uvinds[3], uvinds[1] };
                                //uvinds = new int[] { uvinds[0], uvinds[0], uvinds[0], uvinds[0] };
                                break;
                            }

                        case FaceMode.SingleUV2:
                            {
                                uvinds = new int[] { uvinds[1], uvinds[2], uvinds[3], uvinds[0] };
                                //uvinds = new int[] { uvinds[0], uvinds[0], uvinds[0], uvinds[0] };
                                break;
                            }
                    }


                    switch (i)
                    {
                        case 0: arrind = new int[4] { 0, 4, 5, 6 }; break;
                        case 1: arrind = new int[4] { 4, 1, 6, 7 }; break;
                        case 2: arrind = new int[4] { 5, 6, 2, 8 }; break;
                        case 3: arrind = new int[4] { 6, 7, 8, 3 }; break;
                        default: throw new Exception("Can't have more than 4 quads in a quad block.");
                    }

                    for (int j = 0; j < 4; j++)
                        buf.Add(v[ind[arrind[j]]]);


                    for (int j = 0; j < 4; j++)
                    {
                        if (tex.Count > 0)
                        {
                            if (!tex[i].isAnimated)
                            {
                                buf[j].uv = tex[i].midlods[2].normuv[uvinds[j] - 1];
                            }
                            else
                            {
                                buf[j].uv = tex[i].animframes[1].normuv[uvinds[j] - 1];
                            }
                        }
                        else
                        {
                            buf[j].uv = new Vector2b((byte)((j & 3) >> 1), (byte)(j & 1));
                        }
                    }

                    if (buf.Count != 4)
                    {
                        Helpers.Panic(this, "not a quad! " + buf.Count);
                        Console.ReadKey();
                    }
                }

                return buf;
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, "Can't export quad to MG. Give null.\r\n" + i + "\r\n" + ex.Message);
                return null;
            }
        }

        public int[] GetUVIndices2(int x, int y, int z, int w)
        {
            return new int[]
            {
                x, y, z, w
            };
        }

        bool objSaveQuads = false;

        public string ToObj(List<Vertex> v, Detail detail, ref int a, ref int b)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("g {0}\r\n", (quadFlags.HasFlag(QuadFlags.InvisibleTriggers) ? "invisible" : "visible"));
            sb.AppendFormat("o piece_{0}\r\n\r\n", id.ToString("X4"));

            switch (detail)
            {
                case Detail.Low:
                    {
                        List<Vertex> list = GetVertexListq(v, -1);

                        foreach (Vertex vt in list)
                        {
                            sb.AppendLine(vt.ToString());
                            sb.AppendLine("vt " + vt.uv.X / 255f + " " + vt.uv.Y / -255f);
                        }

                        sb.AppendLine("\r\nusemtl " + (ptrTexLow != 0 ? texlow.Tag() : "default"));

                        if (objSaveQuads)
                        {
                            sb.Append(OBJ.ASCIIQuad("f", a, b));
                        }
                        else
                        {
                            sb.Append(OBJ.ASCIIFace("f", a, b, 1, 3, 2, 1, 3, 2));
                            sb.Append(OBJ.ASCIIFace("f", a, b, 2, 3, 4, 2, 3, 4));
                        }

                        a += 4;
                        b += 4;

                        break;
                    }
                case Detail.Med:
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            List<Vertex> list = GetVertexListq(v, i);

                            foreach (Vertex vt in list)
                            {
                                sb.AppendLine(vt.ToString());
                                sb.AppendLine("vt " + vt.uv.X / 255f + " " + vt.uv.Y / -255f);
                            }

                            sb.AppendLine("\r\nusemtl " + (ptrTexMid[i] != 0 ? tex[i].midlods[2].Tag() : "default"));

                            if (objSaveQuads)
                            {
                                sb.Append(OBJ.ASCIIQuad("f", a, b));
                            }
                            else
                            {
                                sb.Append(OBJ.ASCIIFace("f", a, b, 1, 3, 2, 1, 3, 2));
                                sb.Append(OBJ.ASCIIFace("f", a, b, 2, 3, 4, 2, 3, 4));
                            }

                            sb.AppendLine();

                            b += 4;
                            a += 4;
                        }

                        break;
                    }
            }

            return sb.ToString();
        }

        /*
        public string ToObj(List<Vertex> v, Detail detail, ref int a, ref int b)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("g {0}\r\n", (quadFlags.HasFlag(QuadFlags.InvisibleTriggers) ? "invisible" : "visible"));
            sb.AppendFormat("o piece_{0}\r\n\r\n", id.ToString("X4"));

            int vcnt;

            switch (detail)
            {
                case Detail.Low: vcnt = 4; break;
                case Detail.Med: vcnt = 9; break;
                default: vcnt = 0; break;
            }

            for (int i = 0; i < vcnt; i++)
            {
                sb.AppendLine(v[ind[i]].ToString());
            }

            sb.AppendLine();




            //if (!quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
            {

                switch (detail)
                {
                    case Detail.Low:
                        {
                            sb.AppendLine(texlow.ToObj());

                            sb.Append(OBJ.ASCIIFace("f", a, b, 1, 3, 2, 1, 3, 2));
                            sb.Append(OBJ.ASCIIFace("f", a, b, 2, 3, 4, 2, 3, 4));

                            b += 4;

                            break;
                        }

                    case Detail.Med:
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (tex.Count == 4)
                                {
                                    sb.AppendLine(tex[i].midlods[2].ToObj());
                                }
                                else
                                {
                                    sb.AppendLine("usemtl default");
                                }

                                if (quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
                                    sb.AppendLine("usemtl default");

                                switch (faceFlags[i].rotateFlipType)
                                {
                                    case RotateFlipType.None: uvinds = GetUVIndices(1, 2, 3, 4); break;
                                    case RotateFlipType.Rotate90: uvinds = GetUVIndices(3, 1, 4, 2); break;
                                    case RotateFlipType.Rotate180: uvinds = GetUVIndices(4, 3, 2, 1); break;
                                    case RotateFlipType.Rotate270: uvinds = GetUVIndices(2, 4, 1, 3); break;
                                    case RotateFlipType.Flip: uvinds = GetUVIndices(2, 1, 4, 3); break;
                                    case RotateFlipType.FlipRotate90: uvinds = GetUVIndices(4, 2, 3, 1); break;
                                    case RotateFlipType.FlipRotate180: uvinds = GetUVIndices(3, 4, 1, 2); break;
                                    case RotateFlipType.FlipRotate270: uvinds = GetUVIndices(1, 3, 2, 4); break;
                                }

                                switch (faceFlags[i].faceMode)
                                {
                                    case FaceMode.Normal:
                                        {
                                            sb.Append(OBJ.ASCIIFace("f", a, b, inds[i * 6], inds[i * 6 + 1], inds[i * 6 + 2], uvinds[0], uvinds[1], uvinds[2])); // 1 3 2 | 0 2 1
                                            sb.Append(OBJ.ASCIIFace("f", a, b, inds[i * 6 + 3], inds[i * 6 + 4], inds[i * 6 + 5], uvinds[3], uvinds[4], uvinds[5])); // 2 3 4 | 1 2 3
                                            break;
                                        }
                                    case FaceMode.SingleUV1:
                                        {
                                            sb.Append(OBJ.ASCIIFace("f", a, b, inds[i * 6], inds[i * 6 + 1], inds[i * 6 + 2], uvinds[1], uvinds[2], uvinds[0])); // 1 3 2 | 0 2 1
                                            break;
                                        }

                                    case FaceMode.SingleUV2:
                                        {
                                            sb.Append(OBJ.ASCIIFace("f", a, b, inds[i * 6], inds[i * 6 + 1], inds[i * 6 + 2], uvinds[2], uvinds[0], uvinds[1]));
                                            break;
                                        }
                                    case FaceMode.Unknown:
                                        {
                                            //should never happen i guess
                                            Helpers.Panic(this, "FaceMode: both flags are set!");
                                            Console.ReadKey();
                                            break;
                                        }
                                }

                                sb.AppendLine();

                                if (tex.Count == 4) b += 4;
                            }

                            break;
                        }
                }

            }
                
                a += vcnt;
            
            return sb.ToString();
        }
        */


        public void Write(BinaryWriterEx bw)
        {
            for (int i = 0; i < 9; i++)
                bw.Write(ind[i]);

            bw.Write((ushort)quadFlags);
            //bw.Write(unk1);

            // bw.Write(drawOrderLow);
            bw.Write(bitvalue);
            bw.Write(drawOrderHigh);

            for (int i = 0; i < 4; i++)
                bw.Write(ptrTexMid[i]);

            bb.Write(bw);

            bw.Write((byte)terrainFlag);
            bw.Write(WeatherIntensity);
            bw.Write(WeatherType);
            bw.Write(TerrainFlagUnknown);

            bw.Write(id);

            bw.Write(trackPos);
            bw.Write(midunk);
            //bw.Write(midflags);

            bw.Write(ptrTexLow);
            bw.Write(mosaicStruct);

            foreach (Vector2s v in unk3)
                v.Write(bw);
        }
    }
}
