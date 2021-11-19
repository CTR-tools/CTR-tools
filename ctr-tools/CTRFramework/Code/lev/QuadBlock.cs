using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CTRFramework
{
    public struct FaceFlags
    {
        public RotateFlipType rotateFlipType;
        public FaceMode faceMode;

        public byte packedValue => (byte)((int)rotateFlipType | (byte)faceMode << 3);

        public FaceFlags(byte x)
        {
            rotateFlipType = (RotateFlipType)(x & 7);
            faceMode = (FaceMode)(x >> 3 & 3);
        }
    }



    public class QuadBlock : IReadWrite
    {
        public static readonly int SizeOf = 0x5C;
        public long BaseAddress;

        /*
         * 0--4--1
         * | /| /|
         * |/ |/ |
         * 5--6--7
         * | /| /|
         * |/ |/ |
         * 2--8--3
         */

        //0x00

        //9 indices in vertex array, that form 4 quads, see above.
        public short[] ind = new short[9];

        //0x12
        public QuadFlags quadFlags;

        public uint packedFaceData =>
            (uint)(
            drawOrderLow |
            faceFlags[0].packedValue << (8 + 0 * 5) |
            faceFlags[1].packedValue << (8 + 1 * 5) |
            faceFlags[2].packedValue << (8 + 2 * 5) |
            faceFlags[3].packedValue << (8 + 3 * 5) |
            (doubleSided ? 1 : 0) << 8 + 5 * 4);

        //0x14
        //these values are contained in packedFaceData, mask is 8b5b5b5b5b4z where b is bit and z is empty. or is it?
        public byte drawOrderLow;
        public FaceFlags[] faceFlags = new FaceFlags[4];
        public bool doubleSided = false;

        //0x18
        public byte[] drawOrderHigh = new byte[4];

        //0x1C
        public PsxPtr[] ptrTexMid = new PsxPtr[4];    //offsets to mid texture definition

        //0x2C
        public BoundingBox bbox;              //a box that bounds

        //0x38
        public TerrainFlags terrainFlag;
        public byte WeatherIntensity;
        public byte WeatherType;
        public byte TerrainFlagUnknown;     //almost always 0, only found in tiger temple and sewer speedway

        //0x3C
        public short id;
        public byte trackPos;
        public byte midunk;

        //public byte[] midflags = new byte[2];

        public PsxPtr ptrTexLow;            //offset to LOD texture definition
        public PsxPtr ptrAddVis;         //pointes to 4 extra visData structs, to be renamed

        public PsxPtr mosaicPtr1;
        public PsxPtr mosaicPtr2;
        public PsxPtr mosaicPtr3;
        public PsxPtr mosaicPtr4;

        public List<Vector2> faceNormal = new List<Vector2>();    //face normal vector or smth. 4*2 for mid + 2 for low

        //additional data
        public TextureLayout texlow;

        public List<CtrTex> tex = new List<CtrTex>() { null, null, null, null };


        public bool isWater = false;

        public bool unk3set = false;
        public bool unk4set = false;

        public QuadBlock()
        {
        }

        public QuadBlock(BinaryReaderEx br)
        {
            Read(br);
        }


        public void Read(BinaryReaderEx br)
        {
            BaseAddress = br.Position;

            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            quadFlags = (QuadFlags)br.ReadUInt16();

            uint buf = br.ReadUInt32(); //big endian or little??

            drawOrderLow = (byte)(buf & 0xFF);

            for (int i = 0; i < 4; i++)
            {
                byte val = (byte)((buf >> 8 + 5 * i) & 0x1F);
                faceFlags[i] = new FaceFlags(val);
            }

            doubleSided = ((buf >> 28) & 1) > 0;

            byte extradata = (byte)(buf >> 29);

            if (extradata > 0)
                Helpers.Panic(this, PanicType.Assume, $"gotcha! blockflags -> {(extradata).ToString("X2")}");

            if (buf != packedFaceData)
                Helpers.Panic(this, PanicType.Error, $"{buf.ToString("X8")}, {packedFaceData.ToString("X8")}");

            drawOrderHigh = br.ReadBytes(4);

            for (int i = 0; i < 4; i++)
            {
                ptrTexMid[i] = PsxPtr.FromReader(br);

                if (ptrTexMid[i].ExtraBits != HiddenBits.None)
                    Helpers.Panic(this, PanicType.Assume, $"ptrTexMid[{i}] {ptrTexMid.ToString()}");
            }

            bbox = BoundingBox.FromReader(br);

            byte tf = br.ReadByte();

            if (tf > 20)
                Helpers.Panic(this, PanicType.Assume, "unexpected terrain flag value -> " + tf);

            terrainFlag = (TerrainFlags)tf;
            WeatherIntensity = br.ReadByte();
            WeatherType = br.ReadByte();
            TerrainFlagUnknown = br.ReadByte();

            id = br.ReadInt16();
            trackPos = br.ReadByte();
            midunk = br.ReadByte();


            ptrTexLow = PsxPtr.FromReader(br);

            if (ptrTexLow.ExtraBits != HiddenBits.None)
                Helpers.Panic(this, PanicType.Assume, $"ptrTexLow {ptrTexLow.ToString()}");


            ptrAddVis = PsxPtr.FromReader(br);

            if (ptrAddVis.ExtraBits != HiddenBits.None)
                Helpers.Panic(this, PanicType.Assume, $"mosaicStruct {ptrAddVis.ToString()}");


            for (int i = 0; i < 5; i++)
                faceNormal.Add(br.ReadVector2s(1 / 4096f));

            //done reading

            if (br.Position - BaseAddress != SizeOf)
                Helpers.Panic(this, PanicType.Error, "SizeOf mismatch!");



            //read texture layouts
            int texpos = (int)br.Position;

            br.Jump(ptrTexLow);
            texlow = TextureLayout.FromReader(br);


            int cntr = 0;

            Helpers.Panic(this, PanicType.Debug, id.ToString("X8"));

            foreach (var ptr in ptrTexMid)
            {
                if (ptr == PsxPtr.Zero)
                {
                    if (ptrTexLow != PsxPtr.Zero)
                        Helpers.Panic(this, PanicType.Assume, $"Got low tex without mid tex at {br.HexPos()}.");

                    continue;
                }

                br.Jump(ptr);
                tex[cntr] = new CtrTex(br, ptr);

                cntr++;
            }

            if (ptrAddVis != UIntPtr.Zero)
            {
                br.Jump(ptrAddVis);

                mosaicPtr1 = PsxPtr.FromReader(br);
                mosaicPtr2 = PsxPtr.FromReader(br);
                mosaicPtr3 = PsxPtr.FromReader(br);
                mosaicPtr4 = PsxPtr.FromReader(br);
            }

            br.Jump(texpos);
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

        public void ColTest(List<Vertex> vertices)
        {
            /*
            Vector3 A = vertices[ind[0]].coord;
            Vector3 B = vertices[ind[1]].coord;
            Vector3 C = vertices[ind[2]].coord;
            Vector3 D = vertices[ind[3]].coord;

            Console.WriteLine(A);
            Console.WriteLine(B);
            Console.WriteLine(C);
            Console.WriteLine(D);

            Console.WriteLine("actual vals " + unk3[4]);

            Vector3 cross1 = Vector3.Cross(B - A, C - A);
            Vector3 cross2 = Vector3.Cross(D - B, C - B);

            Console.WriteLine("guess1 " + cross1.Length());
            Console.WriteLine("guess2 " + cross2.Length());

            Console.WriteLine(Vector3.Normalize(cross1));
            */

        }

        private List<int[]> FaceIndices = new List<int[]>() {
            new int[] { 0, 4, 5, 6 },
            new int[] { 4, 1, 6, 7 },
            new int[] { 5, 6, 2, 8 },
            new int[] { 6, 7, 8, 3 }
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

        public List<Vertex> GetVertexList(Scene s)
        {
            List<Vertex> buf = new List<Vertex>();

            for (int i = inds.Length - 1; i >= 0; i--)
                buf.Add(s.verts[ind[inds[i] - 1]]);

            for (int i = 0; i < inds.Length / 6; i++)
            {
                buf[i * 6 + 0].uv = new Vector2(0, 1);
                buf[i * 6 + 1].uv = new Vector2(1, 0);
                buf[i * 6 + 2].uv = new Vector2(0, 0);
                buf[i * 6 + 3].uv = new Vector2(0, 1);
                buf[i * 6 + 4].uv = new Vector2(1, 1);
                buf[i * 6 + 5].uv = new Vector2(1, 0);
            }

            return buf;
        }


        //use this later for obj export too
        public List<Vertex> GetVertexListq(List<Vertex> vertexArray, int i)
        {
            try
            {
                List<Vertex> buf = new List<Vertex>();

                if (i == -1)
                {
                    int[] arrind = new int[] { 0, 1, 2, 3 };

                    for (int j = 0; j < 4; j++)
                        buf.Add(vertexArray[ind[arrind[j]]]);

                    for (int j = 0; j < 4; j++)
                    {
                        buf[j].uv = texlow.normuv[j];
                    }

                    if (buf.Count != 4)
                    {
                        Helpers.Panic(this, PanicType.Error, "not a quad! " + buf.Count);
                        //Console.ReadKey();
                    }

                    return buf;
                }
                else
                {
                    int[] arrind;
                    int[] uvinds;

                    uvinds = GetUVIndices2(1, 2, 3, 4);

                   
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
                    


                    if (i > 4 || i < 0)
                    {
                        Helpers.Panic(this, PanicType.Error, "Can't have more than 4 quads in a quad block.");
                        return null;
                    }

                    arrind = FaceIndices[i];

                    for (int j = 0; j < 4; j++)
                    {
                        buf.Add(vertexArray[ind[arrind[j]]]);
                    }

                    /*
                    for (int j = 0; j < 4; j++)
                    {
                        buf[j].color = new Vector4b(
                            (byte)(255 / 4 * j),
                            (byte)(255 / 4 * j),
                            (byte)(255 / 4 * j), 
                            0);
                    }
                    */


                    for (int j = 0; j < 4; j++)
                    {
                        if (tex[i] != null)
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
                            buf[j].uv = new Vector2(0, 0);//new Vector2b((byte)((j & 3) >> 1), (byte)(j & 1));
                        }
                    }

                    if (buf.Count != 4)
                        Helpers.Panic(this, PanicType.Error, "not a quad! " + buf.Count);
                }

                return buf;
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, PanicType.Error, "Can't export quad to MG. Give null.\r\n" + this.id.ToString("X8") + " " + this.BaseAddress.ToString("X8") + "\r\n" + ex.Message);
                //Console.ReadKey();
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

        bool objSaveQuads = true;

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
                            sb.AppendLine(vt.ToObj());
                            sb.AppendLine("vt " + vt.uv.X / 255f + " " + vt.uv.Y / -255f);
                        }

                        sb.AppendLine("\r\nusemtl " + (ptrTexLow != UIntPtr.Zero ? texlow.Tag : "default"));

                        if (objSaveQuads)
                        {
                            sb.Append(OBJ.ASCIIQuad("f", a, b));
                        }
                        else
                        {
                            sb.AppendLine(OBJ.ASCIIFace("f", a, b, 1, 3, 2, 1, 3, 2));
                            sb.AppendLine(OBJ.ASCIIFace("f", a, b, 2, 3, 4, 2, 3, 4));
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

                            //this normally shouldn't be null
                            if (list != null)
                            {

                                foreach (Vertex vt in list)
                                {
                                    sb.AppendLine(vt.ToObj());
                                    sb.AppendLine("vt " + vt.uv.X / 255f + " " + vt.uv.Y / -255f);
                                }

                                sb.AppendLine("\r\nusemtl " + (ptrTexMid[i] != UIntPtr.Zero ? (tex[i] != null ? tex[i].midlods[2].Tag : "default") : "default"));

                                if (objSaveQuads)
                                {
                                    sb.Append(OBJ.ASCIIQuad("f", a, b));
                                }
                                else
                                {
                                    sb.AppendLine(OBJ.ASCIIFace("f", a, b, 1, 3, 2, 1, 3, 2));
                                    sb.AppendLine(OBJ.ASCIIFace("f", a, b, 2, 3, 4, 2, 3, 4));
                                }

                                sb.AppendLine();

                                b += 4;
                                a += 4;
                            }
                            else
                            {
                                Helpers.Panic(this, PanicType.Error, $"something's wrong with quadblock {id} at {BaseAddress.ToString("X8")}, happens in secret2_4p and temple2_4p");
                            }
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


        public void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null)
        {
            long sizeCheck = bw.BaseStream.Position;

            for (int i = 0; i < 9; i++)
                bw.Write(ind[i]);

            bw.Write((ushort)quadFlags);
            //bw.Write(unk1);

            // bw.Write(drawOrderLow);
            bw.Write(packedFaceData);
            bw.Write(drawOrderHigh);

            for (int i = 0; i < 4; i++)
                ptrTexMid[i].Write(bw, patchTable);

            bbox.Write(bw);

            bw.Write((byte)terrainFlag);
            bw.Write(WeatherIntensity);
            bw.Write(WeatherType);
            bw.Write(TerrainFlagUnknown);

            bw.Write(id);

            bw.Write(trackPos);
            bw.Write(midunk);
            //bw.Write(midflags);

            bw.Write(ptrTexLow, patchTable);
            bw.Write(ptrAddVis, patchTable);

            foreach (Vector2 v in faceNormal)
                bw.WriteVector2s(v, 1 / 4096f);

            if (bw.BaseStream.Position - sizeCheck != SizeOf)
                throw new Exception("QuadBlock: size mismatch.");
        }
    }
}