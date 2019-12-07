using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
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

    public class QuadBlock : IRead, IWrite
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

        //9 indices in vertex array, that form 4 quads, see above.
        public short[] ind = new short[9];
        public QuadFlags quadFlags;

        public uint bitvalue; //important! big endian!

        //these values are contained in bitvalue, mask is 8b5b5b5b5b4z where b is bit and z is empty
        public byte drawOrderLow;
        public FaceFlags[] faceFlags = new FaceFlags[4];
        public byte extradata;

        public byte[] drawOrderHigh = new byte[4];

        public uint[] ptrTexMid = new uint[4];    //offsets to mid texture definition

        public BoundingBox bb;              //a box that bounds

        public TerrainFlags terrainFlag;
        public byte WeatherIntensity;
        public byte WeatherType;
        public byte TerrainFlagUnknown;

        public short id;
        public byte[] midflags = new byte[2];

        public int ptrTexLow;                 //offset to LOD texture definition
        public int offset2;

        public ushort[] unk3 = new ushort[10];  //unknown

        //additional data
        public TextureLayout texlow;
        public List<TextureLayout> texmid = new List<TextureLayout>();
        public List<TextureLayout> texmid2 = new List<TextureLayout>();
        public List<TextureLayout> texmid3 = new List<TextureLayout>();
        public List<TextureLayout> texhi = new List<TextureLayout>();

        public Texture texture;

        public QuadBlock()
        {
        }

        public QuadBlock(BinaryReaderEx br)
        {
            Read(br);
        }


        public void Read(BinaryReaderEx br)
        {
            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            quadFlags = (QuadFlags)br.ReadUInt16();

            bitvalue = br.ReadUInt32Big();
            {
                drawOrderLow = (byte)(bitvalue & 0xFF);

                for (int i = 0; i < 4; i++)
                {
                    byte val = (byte)(bitvalue >> 8 + 5 * i & 0x1F);
                    faceFlags[i] = new FaceFlags(val);
                }

                extradata = (byte)(bitvalue & 0xF0000000 >> 28);
            }

            drawOrderHigh = br.ReadBytes(4);

            for (int i = 0; i < 4; i++)
                ptrTexMid[i] = br.ReadUInt32();

            bb = new BoundingBox(br);

            terrainFlag = (TerrainFlags)br.ReadByte();
            WeatherIntensity = br.ReadByte();
            WeatherType = br.ReadByte();
            TerrainFlagUnknown = br.ReadByte();

            id = br.ReadInt16();

            midflags = br.ReadBytes(2);

            ptrTexLow = br.ReadInt32();
            offset2 = br.ReadInt32();

            for (int i = 0; i < 10; i++)
                unk3[i] = br.ReadUInt16();


            //read texture layouts
            int pos = (int)br.BaseStream.Position;

            br.Jump(ptrTexLow);
            texlow = new TextureLayout(br);

            foreach (uint u in ptrTexMid)
            {
                if (u != 0)
                {
                    br.Jump(u);
                    texmid.Add(new TextureLayout(br));
                    texmid2.Add(new TextureLayout(br));
                    texmid3.Add(new TextureLayout(br));
                }
            }

            br.BaseStream.Position = pos;
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
            1, 6, 5, 5, 6, 7,
            5, 7, 2, 2, 7, 8,
            6, 3, 7, 7, 3, 9,
            7, 9, 8, 8, 9, 4
        };


        public int[] GetUVIndices(int x, int y, int z, int w)
        {
            return new int[]
            {
                x, z, y,
                y, z, w
            };
        }

        int[] uvinds = new int[] {
            1, 3, 2,
            2, 3, 4
        };

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
                sb.AppendLine(v[ind[i]].ToString(false));
            }

            sb.AppendLine();


            if (!quadFlags.HasFlag(QuadFlags.InvisibleTriggers))
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
                                if (texmid3.Count == 4)
                                {
                                    sb.AppendLine(texmid3[i].ToObj());
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
                                            Console.WriteLine("both flags are set!");
                                            Console.ReadKey();
                                            break;
                                        }
                                }

                                sb.AppendLine();

                                if (texmid.Count == 4) b += 4;
                            }

                            break;
                        }
                }

            }

            a += vcnt;

            return sb.ToString();
        }



        public void Write(BinaryWriter bw)
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

            bw.Write(midflags);

            bw.Write(ptrTexLow);
            bw.Write(offset2);

            for (int i = 0; i < 10; i++)
                bw.Write(unk3[i]);
        }
    }
}
