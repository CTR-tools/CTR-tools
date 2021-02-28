using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace CTRFramework
{
    public class CtrModel
    {
        public string path;

        private string name = "defaultname";
        [Browsable(true), DisplayName("Model name"), Description(""), Category("CTR Model")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private CTREvent gameEvent = CTREvent.Nothing;
        [Browsable(true), DisplayName("CTR event"), Description(""), Category("CTR Model")]
        public CTREvent GameEvent
        {
            get { return gameEvent; }
            set { gameEvent = value; }
        }

        //public short numEntries;
        public int ptrHeaders = 0;

        public List<CtrHeader> Entries = new List<CtrHeader>();

        public CtrModel()
        {
        }

        public CtrModel(string s)
        {
            path = s;

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(s));
            BinaryReaderEx br = new BinaryReaderEx(ms);

            int size = br.ReadInt32();

            ms = new MemoryStream(br.ReadBytes((int)size));
            br = new BinaryReaderEx(ms);

            Read(br);
        }

        public CtrModel(BinaryReaderEx br)
        {
            Read(br);
        }

        public static CtrModel FromFile(string s)
        {
            return new CtrModel(s);
        }

        public void Read(BinaryReaderEx br)
        {
            Console.WriteLine(br.BaseStream.Position.ToString("X8"));

            name = br.ReadStringFixed(16);
            gameEvent = (CTREvent)br.ReadInt16();
            int numEntries = br.ReadInt16();
            ptrHeaders = br.ReadInt32();

            Console.WriteLine("LODModel: " + name);

            for (int i = 0; i < numEntries; i++)
            {
                Entries.Add(new CtrHeader(br));
            }

            //Helpers.WriteToFile("test.obj", sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name + ": ");

            foreach (CtrHeader head in Entries)
                sb.Append(head.name + ", ");

            sb.Append("\r\n");

            return sb.ToString();
        }


        public void Export(string dir)
        {
            foreach (CtrHeader h in Entries)
            {
                string fn = Path.Combine(dir, String.Format("{0}\\{1}.obj", name, h.name));
                Helpers.WriteToFile(fn, h.ToObj());
            }
        }

        public void Write(string path)
        {
            Write(path, $"{name}.ctr");
        }

        public void Write(string path, string filename)
        {
            using (BinaryWriterEx bw = new BinaryWriterEx(File.OpenWrite(Path.Combine(path, filename))))
            {
                Write(bw);
                bw.Close();
            }
        }

        public void Write(BinaryWriterEx bw)
        {
            ptrs.Clear();

            bw.Write(FixPointers());

            if (name.Length > 16)
                Helpers.Panic(this, $"Name too long: {name}");

            bw.Write(name.ToCharArray());
            bw.BaseStream.Position = 20;

            bw.Write((ushort)gameEvent);
            bw.Write((ushort)Entries.Count);

            ptrs.Add((int)bw.BaseStream.Position);
            bw.Write(ptrHeaders);

            foreach (var ctr in Entries)
            {
                ctr.Write(bw, CtrWriteMode.Header);
            }

            foreach (var ctr in Entries)
            {
                ctr.Write(bw, CtrWriteMode.Data);
            }

            bw.Write(ptrs.Count * 4);

            foreach (int x in ptrs)
                bw.Write(x - 4);
        }


        public static List<int> ptrs = new List<int>();

        public int FixPointers()
        {
            int curPtr = 0x18;
            ptrHeaders = curPtr;

            curPtr += 64 * Entries.Count;

            if (curPtr % 4 != 0)
                curPtr = ((curPtr / 4) + 1 ) * 4;

            foreach (var ctr in Entries)
            {
                ctr.ptrCmd = curPtr;
                curPtr += (4 + ctr.drawList.Count * 4 + 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrVerts = curPtr;
                curPtr += (8 + 16 + 4 + ctr.vtx.Count * 3);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;

                ctr.ptrTex = curPtr;

                if (ctr.tl.Count > 0)
                {
                    curPtr += ctr.tl.Count * 4 + ctr.tl.Count * 0x0C;

                    if (curPtr % 4 != 0)
                        curPtr = ((curPtr / 4) + 1) * 4;
                }

                ctr.ptrClut = curPtr;
                curPtr += (ctr.cols.Count * 4);

                if (curPtr % 4 != 0)
                    curPtr = ((curPtr / 4) + 1) * 4;
            }

            return curPtr;
        }


        public static CtrModel FromObj(OBJ obj)
        {
            CtrModel ctr = new CtrModel();
            ctr.Name = obj.ObjectName;

            CtrHeader model = new CtrHeader();
            model.name = obj.ObjectName + "_hi";
            model.lodDistance = -1;

            BoundingBox bb = new BoundingBox();

            foreach (var v in obj.vertices)
            {
                if (v.X > bb.Max.X) bb.Max.X = v.X;
                if (v.Y > bb.Max.Y) bb.Max.Y = v.Y;
                if (v.Z > bb.Max.Z) bb.Max.Z = v.Z;
                if (v.X < bb.Min.X) bb.Min.X = v.X;
                if (v.Y < bb.Min.Y) bb.Min.Y = v.Y;
                if (v.Z < bb.Min.Z) bb.Min.Z = v.Z;
            }

            foreach (var v in obj.vertices)
            {
                v.X -= bb.Min.X;
                v.Y -= bb.Min.Y;
                v.Z -= bb.Min.Z;
            }



            BoundingBox bb2 = bb.Clone();

            bb2.Min.X -= bb.Min.X;
            bb2.Min.Y -= bb.Min.Y;
            bb2.Min.Z -= bb.Min.Z;
            bb2.Max.X -= bb.Min.X;
            bb2.Max.Y -= bb.Min.Y;
            bb2.Max.Z -= bb.Min.Z;

            Console.WriteLine(bb.ToString());
            Console.WriteLine(bb2.ToString());

            model.scale = new Vector4s(
                (short)(bb2.Max.X * 10),
                (short)(bb2.Max.Y * 10),
                (short)(bb2.Max.Z * 10),
                0);

            model.vtx.Clear();

            foreach (var v in obj.vertices)
            {
                Vector3b vv = new Vector3b(
                   (byte)((float)v.X / bb2.Max.X * 255),
                   (byte)((float)v.Z / bb2.Max.Z * 255),
                   (byte)((float)v.Y / bb2.Max.Y * 255)
                    );

                model.vtx.Add(vv);
            }



            List<short> accessed = new List<short>();
            List<Vector3b> newlist = new List<Vector3b>();

            //foreach (var f in obj.faces)
            for (int i = 0; i < obj.faces.Count; i++)
            {
                //if (f.X > 255 || f.Y > 255 || f.Z > 255)
                //    throw new Exception("too many vertices. 255 is the limit. reduce vertex count and make sure you merged all vertices.");

                CtrDraw cmd = new CtrDraw();
                cmd.texIndex = 0;
                cmd.colorIndex = (byte)obj.colinds[i].X;
                cmd.stackIndex = 87;
                Console.WriteLine(cmd.stackIndex);
                cmd.flags = CtrDrawFlags.s;

                newlist.Add(model.vtx[obj.faces[i].X]);
                /*
                if (accessed.Contains(cmd.stackIndex))
                {
                    cmd.flags = cmd.flags | CtrDrawFlags.v;
                }
                else
                {
                    accessed.Add(cmd.stackIndex);
                    newlist.Add(model.vtx[cmd.stackIndex]);
                }
                */


                model.drawList.Add(cmd);


                cmd = new CtrDraw();
                cmd.texIndex = 0;
                cmd.colorIndex = (byte)obj.colinds[i].Z;
                cmd.stackIndex = 87;
                Console.WriteLine(cmd.stackIndex);
                cmd.flags = 0;

                newlist.Add(model.vtx[obj.faces[i].Z]);
                /*
                if (accessed.Contains(cmd.stackIndex))
                {
                    cmd.flags = cmd.flags | CtrDrawFlags.v;
                }
                else
                {
                    accessed.Add(cmd.stackIndex);
                    newlist.Add(model.vtx[cmd.stackIndex]);
                }
                */

                model.drawList.Add(cmd);


                cmd = new CtrDraw();
                cmd.texIndex = 0;
                cmd.colorIndex = (byte)obj.colinds[i].Y;
                cmd.stackIndex = 87;
                Console.WriteLine(cmd.stackIndex);
                cmd.flags = 0;

                newlist.Add(model.vtx[obj.faces[i].Y]);
                /*
                if (accessed.Contains(cmd.stackIndex))
                {
                    cmd.flags = cmd.flags | CtrDrawFlags.v;
                }
                else
                {
                    accessed.Add(cmd.stackIndex);
                    newlist.Add(model.vtx[cmd.stackIndex]);
                }
                */

                model.drawList.Add(cmd);
            }

            model.vtx = newlist;

            model.posOffset = new Vector4s(
                (short)(-bb2.Max.X + 15),
                30,//(short)(bb.Min.Y + modelOffset), 
                (short)(-bb2.Max.Y),
                0);

            //model.cols.Add(new Vector4b(0x80, 0x80, 0x80, 0));

            foreach (var c in obj.unique)
                model.cols.Add(c);

            ctr.Entries.Add(model);

            return ctr;
        }
    }

    public enum CtrWriteMode
    {
        Header,
        Data
    }
}