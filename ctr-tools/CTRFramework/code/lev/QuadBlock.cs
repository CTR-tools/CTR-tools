using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class QuadBlock : IRead, IWrite
    {
        public static int pagex = -1;
        public static int pagey = -1;

        //raw data
        public short[] ind = new short[9];  //9 indices in vertex array, that form 4 quads.

        /*
         * 0--4--1
         * | /| /|
         * |/ |/ |
         * 5--6--7
         * | /| /|
         * |/ |/ |
         * 2--8--3
         */

        public QuadFlags quadFlags;

        public byte drawOrderLow;
        public byte f1;
        public byte f2;
        public byte f3;
        public byte[] drawOrderHigh = new byte[4];

        public uint[] tex = new uint[4];    //offsets to texture definition

        public BoundingBox bb;              //a box that bounds

        public TerrainFlags terrainFlag;
        public byte WeatherIntensity;
        public byte WeatherType;
        public byte TerrainFlagUnknown;

        public short id;
        public byte[] midflags = new byte[2];

        public int offset1;                 //offset to LOD texture definition
        public int offset2;                 //unknown mostly null
        //leads to 3 textures array

        public ushort[] unk3 = new ushort[10];  //unknown

        //additional data
        public TextureLayout texlow;
        public List<TextureLayout> texmid = new List<TextureLayout>();
        public List<TextureLayout> texhi = new List<TextureLayout>();

        public QuadBlock()
        {
        }

        public QuadBlock(BinaryReader br)
        {
            Read(br);
        }


        public void Read(BinaryReader br)
        {
            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            quadFlags = (QuadFlags)br.ReadUInt16();

            //unk1 = br.ReadBytes(8);

            drawOrderLow = br.ReadByte();
            f1 = br.ReadByte();
            f2 = br.ReadByte();
            f3 = br.ReadByte();
            drawOrderHigh = br.ReadBytes(4);

            for (int i = 0; i < 4; i++)
                tex[i] = br.ReadUInt32();

            bb = new BoundingBox(br);

           // unk2 = br.ReadBytes(4);


            terrainFlag = (TerrainFlags)br.ReadByte();
            WeatherIntensity = br.ReadByte();
            WeatherType = br.ReadByte();
            TerrainFlagUnknown = br.ReadByte();

            id = br.ReadInt16();

            midflags = br.ReadBytes(2);

            offset1 = br.ReadInt32();
            offset2 = br.ReadInt32();

            for (int i = 0; i < 10; i++)
                unk3[i] = br.ReadUInt16();


            //read texture layouts
            int pos = (int)br.BaseStream.Position;

            br.BaseStream.Position = (int)offset1;
            texlow = new TextureLayout(br);


            foreach (uint u in tex)
            {
                if (u != 0)
                {
                   // Console.WriteLine(u.ToString("X8"));
                    br.BaseStream.Position = u;
                    texmid.Add(new TextureLayout(br));
                }
            }

            /* //hires attempt
            if (offset2 > 0)
            {
                br.BaseStream.Position = offset2;

                List<uint> offs = new List<uint>();

                for (int i = 0; i < 4; i++)
                {
                    offs.Add(br.ReadUInt32());
                }

                foreach(uint i in offs)
                {
                    if (i > 0)
                    {
                        br.BaseStream.Position = i;
                        //Console.WriteLine(i.ToString("X8"));
                        texhi.Add(new TextureLayout(br));
                    }
                }

            }
             */
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

        /*
        * 1--5--2
        * | /| /|
        * |/ |/ |
        * 6--7--8
        * | /| /|
        * |/ |/ |
        * 3--9--4 
        */


        public string ToObj(List<Vertex> v, Detail detail, ref int a, ref int b)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("g {0}\r\n", (quadFlags.HasFlag(QuadFlags.InvisibleTriggers) ? "invisible" : "visible")));
            sb.Append(String.Format("o piece_{0}\r\n\r\n", id.ToString("X4")));

            int vcnt;

            switch (detail)
            {
                case Detail.Low: vcnt = 4; break;
                case Detail.High: vcnt = 9; break;
                default: vcnt = 0; break;
            }

            for (int i = 0; i < vcnt; i++)
            {
                sb.AppendLine(v[ind[i]].ToString(false));
            }

            sb.AppendLine();


            switch (detail)
            {
                case Detail.Low:
                    {
                        sb.AppendLine(texlow.ToObj());

                        sb.Append(ASCIIFace("f", a + 1, a + 3, a + 2, b + 1, b + 3, b + 2));
                        sb.Append(ASCIIFace("f", a + 2, a + 3, a + 4, b + 2, b + 3, b + 4));

                        b += 4;

                        break;
                    }

                case Detail.High:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (texmid.Count == 4)
                            {
                                sb.AppendLine(texmid[i].ToObj());
                            }
                            else
                            {
                                sb.AppendLine("usemtl default");
                            }

                            sb.Append(ASCIIFace("f", a + inds[i * 6], a + inds[i * 6 + 1], a + inds[i * 6 + 2], b+1, b+3, b+2)); // 1 3 2
                            sb.Append(ASCIIFace("f", a + inds[i * 6 + 3], a + inds[i * 6 + 4], a + inds[i * 6 + 5], b+2, b+3, b+4)); // 2 3 4

                            sb.AppendLine();

                            if (texmid.Count == 4) b += 4;
                        }

                        break;
                    }
            }

            a += vcnt;

            /*
            if (fmt == "ply")
             * 
            {
                sb.Append(ASCIIFace("3", ind, 5, 4, 0));
                sb.Append(ASCIIFace("3", ind, 4, 5, 6));
                sb.Append(ASCIIFace("3", ind, 6, 1, 4));
                sb.Append(ASCIIFace("3", ind, 1, 6, 7));
                sb.Append(ASCIIFace("3", ind, 2, 6, 5));
                sb.Append(ASCIIFace("3", ind, 6, 2, 8));
                sb.Append(ASCIIFace("3", ind, 8, 7, 6));
                sb.Append(ASCIIFace("3", ind, 7, 8, 3));
            }
            else
            {
                sb.Append("o piece_" + i.ToString("0000") + "\r\n");
                sb.Append("g piece_" + i.ToString("0000") + "\r\n");

                sb.Append(ASCIIFace("f", ind, 5, 4, 0));
                sb.Append(ASCIIFace("f", ind, 4, 5, 6));
                sb.Append(ASCIIFace("f", ind, 6, 1, 4));
                sb.Append(ASCIIFace("f", ind, 1, 6, 7));
                sb.Append(ASCIIFace("f", ind, 2, 6, 5));
                sb.Append(ASCIIFace("f", ind, 6, 2, 8));
                sb.Append(ASCIIFace("f", ind, 8, 7, 6));
                sb.Append(ASCIIFace("f", ind, 7, 8, 3));
            }
            */


            return sb.ToString();
        }


        public string ASCIIFace(string label, int x, int y, int z)
        {
            return label + " " + x + " " + y + " " + z + "\r\n";
        }

        public string ASCIIFace(string label, int x, int y, int z, float xuv, float yuv, float zuv)
        {
            return String.Format(
                "{0} {1}/{2} {3}/{4} {5}/{6}\r\n",
                label, x, xuv, y, yuv, z, zuv);
        }


        public void Write(BinaryWriter bw)
        {
            for (int i = 0; i < 9; i++)
                bw.Write(ind[i]);

            bw.Write((ushort)quadFlags);
            //bw.Write(unk1);

            bw.Write(drawOrderLow);
            bw.Write(f1);
            bw.Write(f2);
            bw.Write(f3);
            bw.Write(drawOrderHigh);

            for (int i = 0; i < 4; i++)
                bw.Write(tex[i]);

            bb.Write(bw);

            bw.Write((byte)terrainFlag);
            bw.Write(WeatherIntensity);
            bw.Write(WeatherType);
            bw.Write(TerrainFlagUnknown);

            bw.Write(id);

            bw.Write(midflags);

            bw.Write(offset1);
            bw.Write(offset2);

            for (int i = 0; i < 10; i++)
                bw.Write(unk3[i]);
        }
    }
}
