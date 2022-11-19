using CTRFramework.Shared;
using CTRFramework.Vram;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CTRFramework
{
    public enum RotateFlipType
    {
        None = 0,
        Rotate90 = 1,
        Rotate180 = 2,
        Rotate270 = 3,
        FlipRotate270 = 4,
        FlipRotate180 = 5,
        FlipRotate90 = 6,
        Flip = 7
    }

    public enum FaceMode
    {
        Normal = 0,
        SingleUV1 = 1,
        SingleUV2 = 2,
        Unknown = 3
    }

    public class FaceFlags
    {
        public RotateFlipType Rotation = RotateFlipType.None;
        public FaceMode faceMode;

        public byte packedValue => (byte)((int)Rotation | (byte)faceMode << 3);

        public FaceFlags(byte x)
        {
            Rotation = (RotateFlipType)(x & 7);
            faceMode = (FaceMode)((x >> 3) & 3);
        }
    }



    public class QuadBlock : IReadWrite
    {
        public static readonly int SizeOf = 0x5C;
        public long BaseAddress;

        //this is copied from bsp tree
        public VisDataFlags visDataFlags = VisDataFlags.None;
        public List<CtrQuad> MidQuads = new List<CtrQuad>();

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

        public short[,] ind2 = new short[3, 3];

        //0x12
        public QuadFlags quadFlags;

        public uint packedFaceData =>
            (uint)(
            drawOrderLow |
            faceFlags[0].packedValue << (8 + 0 * 5) |
            faceFlags[1].packedValue << (8 + 1 * 5) |
            faceFlags[2].packedValue << (8 + 2 * 5) |
            faceFlags[3].packedValue << (8 + 3 * 5) |
            (doubleSided ? 1 : 0) << 31);

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

        public QuadBlock()
        {
        }

        public QuadBlock(BinaryReaderEx br)
        {
            Read(br);
        }

        public void GenerateCtrQuads(List<Vertex> vertices)
        {
            for (int i = 0; i < 4; i++)
            {
                var quad = new CtrQuad()
                {
                    FaceMode = faceFlags[i].faceMode,
                    RotateFlipType = faceFlags[i].Rotation,
                    DrawingOrder = drawOrderHigh[i],
                    FaceNormal = faceNormal[i],
                    Texture = tex[i],
                    Vertices = new Vertex[4] {
                        vertices[ind[FaceIndices[i][0]]],
                        vertices[ind[FaceIndices[i][1]]],
                        vertices[ind[FaceIndices[i][2]]],
                        vertices[ind[FaceIndices[i][3]]]
                    }
                };

                MidQuads.Add(quad);
            }
        }


        public void Read(BinaryReaderEx br)
        {
            BaseAddress = br.Position;

            for (int i = 0; i < 9; i++)
                ind[i] = br.ReadInt16();

            ind2[0, 0] = ind[0];
            ind2[1, 0] = ind[4];
            ind2[2, 0] = ind[1];

            ind2[0, 1] = ind[5];
            ind2[1, 1] = ind[6];
            ind2[2, 1] = ind[7];

            ind2[0, 2] = ind[2];
            ind2[1, 2] = ind[8];
            ind2[2, 2] = ind[3];

            quadFlags = (QuadFlags)br.ReadUInt16();

            uint buf = br.ReadUInt32(); //big endian or little??

            drawOrderLow = (byte)(buf & 0xFF);

            for (int i = 0; i < 4; i++)
            {
                byte val = (byte)((buf >> 8 + 5 * i) & 0x1F);
                faceFlags[i] = new FaceFlags(val);
            }

            doubleSided = ((buf >> 31) & 1) > 0;

            byte extradata = (byte)(buf >> 28);

            if (extradata > 0 && extradata != 8)
                Helpers.Panic(this, PanicType.Assume, $"gotcha! blockflags -> {(extradata).ToString("X2")}");

            if (buf != packedFaceData)
                Helpers.Panic(this, PanicType.Error, $"{buf.ToString("X8")}, {packedFaceData.ToString("X8")}");

            drawOrderHigh = br.ReadBytes(4);

            for (int i = 0; i < 4; i++)
            {
                ptrTexMid[i] = PsxPtr.FromReader(br);

                if (ptrTexMid[i].ExtraBits != HiddenBits.None)
                    Helpers.Panic(this, PanicType.Assume, $"ptrTexMid[{i}] {ptrTexMid[i].ToString()}");
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
                tex[cntr] = new CtrTex(br, ptr, visDataFlags);

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

        /// <summary>
        /// Rotates quad UVs 
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="rotations"></param>
        public void QuadTexRotate(List<Vertex> buf, int rotations = 1)
        {
            if (buf is null)
                return;

            for (int i = 0; i < rotations % 4; i++)
            {
                SwapUV(buf[0], buf[1]);
                SwapUV(buf[0], buf[2]);
                SwapUV(buf[2], buf[3]);
            }
        }

        /// <summary>
        /// Rotates single face UVs.
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="rotations"></param>
        public void QuadTexRotateTri(List<Vertex> buf, int rotations = 1)
        {
            if (buf is null)
                return;

            for (int i = 0; i < rotations % 3; i++)
            {
                SwapUV(buf[0], buf[1]);
                SwapUV(buf[1], buf[2]);
            }
        }

        /// <summary>
        /// Flips quad UVs
        /// </summary>
        /// <param name="buf"></param>
        public void QuadTexFlip(List<Vertex> buf)
        {
            if (buf is null)
                return;

            SwapUV(buf[0], buf[1]);
            SwapUV(buf[2], buf[3]);
        }

        /// <summary>
        /// Swaps UVs of 2 given vertices
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void SwapUV(Vertex v1, Vertex v2)
        {
            var v = v1.uv;
            v1.uv = v2.uv;
            v2.uv = v;
        }

        /// <summary>
        /// Returns 4 vertices of a quad. -1 for low, 0 1 2 3 for individual med quads.
        /// </summary>
        /// <param name="vertexArray"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public List<Vertex> GetVertexListq(List<Vertex> vertexArray, int i)
        {
            try
            {
                List<Vertex> buf = new List<Vertex>();

                if (i == -1)
                {
                    buf.Add(vertexArray[ind2[0, 0]]);
                    buf.Add(vertexArray[ind2[2, 0]]);
                    buf.Add(vertexArray[ind2[0, 2]]);
                    buf.Add(vertexArray[ind2[2, 2]]);

                    for (int j = 0; j < buf.Count; j++)
                        buf[j].uv = texlow.normuv[j];

                    if (buf.Count != 4)
                    {
                        Helpers.Panic(this, PanicType.Error, "not a quad! " + buf.Count);
                    }

                    return buf;
                }
                else
                {
                    if (i > 4 || i < 0)
                    {
                        Helpers.Panic(this, PanicType.Error, "Can't have more than 4 quads in a quad block.");
                        return null;
                    }

                    //get xy based on index as follows, used to shift quad indices
                    //01
                    //23

                    byte x = (byte)i;
                    byte y = (byte)(i >> 1);

                    x &= 1;
                    y &= 1;

                    //get vertices
                    buf.Add(vertexArray[ind2[x + 0, y + 0]].Clone());
                    buf.Add(vertexArray[ind2[x + 1, y + 0]].Clone());
                    buf.Add(vertexArray[ind2[x + 0, y + 1]].Clone());
                    buf.Add(vertexArray[ind2[x + 1, y + 1]].Clone());

                    //assign UVs, move to ctrtex maybe
                    for (int j = 0; j < 4; j++)
                    {
                        if (tex[i] != null)
                        {
                            if (!tex[i].isAnimated)
                            {
                                buf[j].uv = tex[i].lod2.normuv[j];
                            }
                            else
                            {
                                buf[j].uv = tex[i].animframes[1].normuv[j];
                            }
                        }
                        else
                        {
                            buf[j].uv = new Vector2(0, 0);//new Vector2b((byte)((j & 3) >> 1), (byte)(j & 1));
                        }
                    }

                    //handle texture rotations
                    switch (faceFlags[i].Rotation)
                    {
                        case RotateFlipType.None: break;
                        case RotateFlipType.Rotate90: QuadTexRotate(buf, 1); break; //3142
                        case RotateFlipType.Rotate180: QuadTexRotate(buf, 2); break;
                        case RotateFlipType.Rotate270: QuadTexRotate(buf, 3); break;
                        case RotateFlipType.FlipRotate270: QuadTexFlip(buf); QuadTexRotate(buf, 3); break;
                        case RotateFlipType.FlipRotate180: QuadTexFlip(buf); QuadTexRotate(buf, 2); break;
                        case RotateFlipType.FlipRotate90: QuadTexFlip(buf); QuadTexRotate(buf, 1); break; //3142
                        case RotateFlipType.Flip: QuadTexFlip(buf); break;
                        default: throw new Exception("Impossible QuadRotation.");
                    }

                    //handle degenerated quads
                    switch (faceFlags[i].faceMode)
                    {
                        case FaceMode.Normal: break;
                        case FaceMode.SingleUV1: QuadTexRotateTri(buf, 2); break;
                        case FaceMode.SingleUV2: QuadTexRotateTri(buf); break;
                        case FaceMode.Unknown: Helpers.Panic(this, PanicType.Assume, $"quad {id}: both quad flags set"); break;
                    }

                    if (buf.Count != 4)
                        Helpers.Panic(this, PanicType.Error, "not a quad! " + buf.Count);
                }

                return buf;
            }
            catch (Exception ex)
            {
                Helpers.Panic(this, PanicType.Error, "Can't export quad to MG. Give null.\r\n" + this.id.ToString("X8") + " " + this.BaseAddress.ToString("X8") + "\r\n" + ex.Message);
                return null;
            }
        }

        public string ToObj(List<Vertex> v, Detail detail, ref int a, ref int b)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"g\t{(visDataFlags.HasFlag(VisDataFlags.Hidden) ? "invisible" : "visible")}");
            sb.AppendLine($"o\tpiece_{id.ToString("X4")}\r\n");

            switch (detail)
            {
                case Detail.Low:
                    {
                        List<Vertex> list = GetVertexListq(v, -1);

                        foreach (var vt in list)
                        {
                            sb.AppendLine(vt.ToObj());
                            sb.AppendLine("vt\t" + vt.uv.X / 255f + " " + vt.uv.Y / -255f);
                        }

                        sb.AppendLine("\r\nusemtl " + (ptrTexLow != UIntPtr.Zero ? texlow.Tag : "default"));

                        if (OBJ.SaveQuads)
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
                                    sb.AppendLine("vt\t" + vt.uv.X / 255f + " " + vt.uv.Y / -255f);
                                }

                                /*
                                foreach (var subvert in subdiv)
                                {
                                    sb.AppendLine(subvert.ToObj());
                                }
                                */

                                sb.AppendLine("\r\nusemtl " + (ptrTexMid[i] != UIntPtr.Zero ? (tex[i] != null ? tex[i].lod2.Tag : "default") : "default"));
                                //sb.AppendLine($"\r\nusemtl {midunk.ToString("X2")}");


                                if (OBJ.SaveQuads)
                                {
                                    sb.Append(OBJ.ASCIIQuad("f", a, b));
                                }
                                else
                                {
                                    sb.AppendLine(OBJ.ASCIIFace("f", a, b, 1, 3, 2, 1, 3, 2));

                                    if (faceFlags[i].faceMode == FaceMode.Normal)
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

        public void SetFaceColor(List<Vertex> vertices, Vector4b color)
        {
            foreach (var index in ind)
            {
                vertices[index].Color = color;
                vertices[index].MorphColor = color;
            }
        }


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