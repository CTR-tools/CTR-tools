using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class QuadBlock : IRead
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

        public byte[] unk1 = new byte[10];  //assumed flags
        public uint[] tex = new uint[4];    //offsets to texture definition

        public BoundingBox bb;              //a box that bounds
        public byte[] unk2 = new byte[4];   //unknown

        public short id;
        public byte[] midflags = new byte[2];

        public int offset1;                 //offset to LOD texture definition
        public int offset2;                 //unknown mostly null

        public ushort[] unk3 = new ushort[10];  //unknown

        //additional data
        TextureLayout lod_tex;
        List<TextureLayout> ctrtex = new List<TextureLayout>();

        public QuadBlock(BinaryReader br)
        {
            Read(br);
        }

        public void ExportTexture(CTRVRAM vrm)
        {

            /*
            byte[] buf = vrm.tim.GetData(ttx.X0, ttx.Y0,
                Max(ttx.X0, ttx.X1, ttx.X2) - Min(ttx.X0, ttx.X1, ttx.X2),
                Max(ttx.Y0, ttx.Y1, ttx.Y2) - Min(ttx.Y0, ttx.Y1, ttx.Y2)
                );
            
            
            for (int i = 0; i < ttx.Y2 - ttx.Y0; i++)
            {
                for (int j = 0; j < ttx.Y2 - ttx.Y0; j++)
                {
                    Console.Write(buf[i * ttx.Y2 - ttx.Y0 + j]);
                }
                Console.WriteLine();
            }
             */
        }
        /*
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (uint t in tex)
            {
                sb.Append(t.ToString() + "\r\n");
            }

            foreach (CTRTex c in ctrtex)
            {
                sb.Append(c.ToString() + "\r\n");
            }


            return sb.ToString();
        }
         * */



        public void Read(BinaryReader br)
        {
            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            unk1 = br.ReadBytes(10);

            for (int i = 0; i < 4; i++)
                tex[i] = br.ReadUInt32();

            bb = new BoundingBox(br);

            unk2 = br.ReadBytes(4);

            id = br.ReadInt16();

            midflags = br.ReadBytes(2);

            offset1 = br.ReadInt32();
            offset2 = br.ReadInt32();

            for (int i = 0; i < 10; i++)
                unk3[i] = br.ReadUInt16();


            int pos = (int)br.BaseStream.Position;

            br.BaseStream.Position = (int)offset1;

            lod_tex = new TextureLayout(br);
            //Console.Write(lod_tex.ToString());

            
            foreach (uint u in tex)
            {
                if (u > 0)
                {
                   // Console.WriteLine(u.ToString("X8"));
                    br.BaseStream.Position = (int)u;
                    ctrtex.Add(new TextureLayout(br));
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


        public string ToObj(List<Vertex> v, Detail detail)
        {
            StringBuilder sb = new StringBuilder();

            bool x = (unk1[2] > 0);// & (byte)Flags2.InvisibleTriggers) > 0;
            //bool x = false;

            //if (!x)
            {
                sb.Append(String.Format("g piece_{0}\r\n", id.ToString("X4")));
                sb.Append(String.Format("o piece_{0}\r\n\r\n", id.ToString("X4")));

                for (int i = 0; i < ind.Length; i++)
                {
                    sb.AppendLine(v[ind[i]].ToString(x));
                }

                sb.AppendLine();


                switch (detail)
                {
                    case Detail.Low:
                        {
                            sb.Append(String.Format("usemtl texpage_{0}_{1}\r\n", lod_tex.PageX, lod_tex.PageY));

                            sb.AppendLine(lod_tex.ToObj());
                            sb.AppendLine();

                            sb.Append(ASCIIFace("f", -9 + 2, -9 + 1, -9 + 0, -4 + 2, -4 + 1, -4 + 0));
                            sb.Append(ASCIIFace("f", -9 + 1, -9 + 2, -9 + 3, -4 + 1, -4 + 2, -4 + 3));
                            break;
                        }

                    case Detail.High:
                        {
                            foreach (TextureLayout tl in ctrtex)
                            {
                                sb.AppendLine(tl.ToObj());

                            }

                            sb.AppendLine();



                            if (ctrtex.Count == 4)
                            {
                                if (pagex != ctrtex[0].PageX || pagey != ctrtex[0].PageY)
                                {
                                    sb.Append(String.Format("usemtl texpage_{0}_{1}\r\n", ctrtex[0].PageX, ctrtex[0].PageY));
                                    pagex = ctrtex[0].PageX;
                                    pagey = ctrtex[0].PageY;
                                }
                            }

                            sb.Append(ASCIIFace("f", -9 + 5, -9 + 4, -9 + 0, -16 + 0, -16+3, -16 + 1));
                            sb.Append(ASCIIFace("f", -9 + 4, -9 + 5, -9 + 6, -16 + 3, -16+0, -16 + 2));

                            if (ctrtex.Count == 4)
                            {
                                if (pagex != ctrtex[1].PageX || pagey != ctrtex[1].PageY)
                                {
                                    sb.Append(String.Format("usemtl texpage_{0}_{1}\r\n", ctrtex[1].PageX, ctrtex[1].PageY));
                                    pagex = ctrtex[1].PageX;
                                    pagey = ctrtex[1].PageY;
                                }
                            }

                            sb.Append(ASCIIFace("f", -9 + 6, -9 + 1, -9 + 4, -12 + 0, -12 + 3, -12 + 1));
                            sb.Append(ASCIIFace("f", -9 + 1, -9 + 6, -9 + 7, -12 + 3, -12 + 0, -12 + 2));

                            if (ctrtex.Count == 4)
                            {
                                if (pagex != ctrtex[2].PageX || pagey != ctrtex[2].PageY)
                                {
                                    sb.Append(String.Format("usemtl texpage_{0}_{1}\r\n", ctrtex[2].PageX, ctrtex[2].PageY));
                                    pagex = ctrtex[2].PageX;
                                    pagey = ctrtex[2].PageY;
                                }
                            }
                            sb.Append(ASCIIFace("f", -9 + 2, -9 + 6, -9 + 5, -8 + 0, -8 + 3, -8 + 1));
                            sb.Append(ASCIIFace("f", -9 + 6, -9 + 2, -9 + 8, -8 + 3, -8 + 0, -8 + 2));

                            if (ctrtex.Count == 4)
                            {
                                if (pagex != ctrtex[3].PageX || pagey != ctrtex[3].PageY)
                                {
                                    sb.Append(String.Format("usemtl texpage_{0}_{1}\r\n", ctrtex[0].PageX, ctrtex[0].PageY));
                                    pagex = ctrtex[3].PageX;
                                    pagey = ctrtex[3].PageY;
                                }
                            }
                            sb.Append(ASCIIFace("f", -9 + 8, -9 + 7, -9 + 6, -4 + 0, -4 + 3, -4 + 1));
                            sb.Append(ASCIIFace("f", -9 + 7, -9 + 8, -9 + 3, -4 + 3, -4 + 0, -4 + 2));
                            
                            break;
                        }
                }

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

            }
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

    }
}
