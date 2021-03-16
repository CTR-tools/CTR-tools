using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace CTRTools
{

    [Serializable]
    public class MemMapStruct
    {
        public int BasePointer;
        public string Name;
        public List<MemMapEntry> Entries = new List<MemMapEntry>();

        public MemMapStruct()
        {
        }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(File.Create(filename)))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                XmlSerializer x = new XmlSerializer(this.GetType());
                x.Serialize(sw, this, ns);
            }
        }

        public static MemMapStruct Load(string filename)
        {
            MemMapStruct pp = new MemMapStruct();

            using (StreamReader sr = new StreamReader(File.OpenRead(filename)))
            {
                XmlSerializer x = new XmlSerializer(pp.GetType());
                pp = (MemMapStruct)x.Deserialize(sr);
                sr.Close();
            }

            return pp;
        }


        public void Write(Mem mem, string name, string value)
        {
            MemMapEntry entry = Entries.Find(x => x.Name == name);

            if (entry == null)
                return;

            uint absPtr = (uint)(BasePointer + entry.Offset);

            Console.WriteLine($"Writing {value} at {absPtr.ToString("X8")}");

            switch (entry.Type)
            {
                case "uint":
                    uint uintValue = UInt32.Parse(value);
                    mem.WritePSXUInt32((uint)absPtr, uintValue, null);
                    break;
                case "short":
                    short shortValue = Int16.Parse(value);
                    mem.WritePSXInt16(absPtr, shortValue, null);
                    break;
                case "byte":
                    byte byteValue = Byte.Parse(value);
                    mem.WritePSXByte(absPtr, byteValue, null);
                    break;
                default:
                    Console.WriteLine("Unimplemented write type.");
                    break;
            }
        }
    }

    [Serializable]
    public class MemMapEntry
    {
        [XmlAttribute]
        public int Offset;
        [XmlAttribute]
        public string Type;
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Comment;

        public MemMapEntry()
        {
        }
    }

    public class Mem
    {
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_MAGIC = 0x1F0FFF;

        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProccessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public Process process;
        private IntPtr processHandle;

        public bool ready = false;


        public Mem()
        {
        }

        public Mem(string pn)
        {
            try
            {
                process = Process.GetProcessesByName(pn)[0];
                ready = true;
            }
            catch
            {
                ready = false;
            }
        }

        public byte[] GetBytes(int pointer, int length)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_READ, false, process.Id);
            byte[] buffer = new byte[length];
            ReadProcessMemory((int)processHandle, pointer, buffer, buffer.Length, ref length);
            return buffer;
        }

        public float ReadFloat(int pointer)
        {
            return BitConverter.ToSingle(GetBytes(pointer, 4), 0);
        }

        public int ReadInt(int pointer)
        {
            return BitConverter.ToInt32(GetBytes(pointer, 4), 0);
        }


        public byte ReadByte(int pointer)
        {
            return GetBytes(pointer, 1)[0];
        }


        public uint ReadPSXUInt32(uint pointer)
        {
            pointer += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;
            return BitConverter.ToUInt32(GetBytes((int)pointer, 4), 0);
        }

        public ushort ReadPSXUInt16(uint pointer)
        {
            pointer += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;
            return BitConverter.ToUInt16(GetBytes((int)pointer, 2), 0);
        }

        public byte[] ReadArray(uint where, int cnt)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesRead = cnt;
            byte[] buffer = new byte[cnt];

            ReadProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesRead);

            return buffer;
        }

        public char[] ReadCharArray(uint where, int cnt)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesRead = cnt;
            byte[] buffer = new byte[cnt];

            ReadProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesRead);

            char[] cb = new char[cnt];

            for (int i = 0; i < cnt; i++)
            {
                cb[i] = (char)buffer[i];
            }

            return cb;
        }

        public void WriteArray(uint where, byte[] wr)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = wr.Length;
            byte[] buffer = wr;

            WriteProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesWritten);
        }


        public void WritePSXUInt16(uint where, ushort wr, System.Windows.Forms.TextBox tb)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 2;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesWritten);
        }
        public void WritePSXInt16(uint where, short wr, System.Windows.Forms.TextBox tb)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 2;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesWritten);
        }

        public void WritePSXByte(uint where, byte wr, System.Windows.Forms.TextBox tb)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 1;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesWritten);
        }


        public void WritePSXUInt32(uint where, uint wr, System.Windows.Forms.TextBox tb)
        {
            where += (uint)process.MainModule.BaseAddress + 0xA82020 - 0x80000000;

            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 4;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, (int)where, buffer, buffer.Length, ref bytesWritten);
        }


        public void WriteFloat(int where, float wr)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 4;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesWritten);
        }

        public void WriteByte(int where, byte wr)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 1;
            byte[] buffer = new byte[1];
            buffer[0] = wr;

            WriteProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesWritten);
        }

        public void WriteInt(int where, int wr)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 4;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesWritten);
        }

        public void WriteShort(int where, short wr)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 2;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesWritten);
        }

        public void WriteUInt32(int where, uint wr)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            int bytesWritten = 4;
            byte[] buffer = BitConverter.GetBytes(wr);

            WriteProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesWritten);
        }

        public string ReadString(int where, int count)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_READ, false, process.Id);

            int bytesRead = count;
            byte[] buffer = new byte[count];

            ReadProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesRead);

            return System.Text.Encoding.Default.GetString(buffer);
        }

        public string ReadString(int where)
        {
            List<byte> str = new List<byte>();

            int offset = where;
            byte x;

            do
            {
                x = ReadByte(offset);
                if (x != 0x0) str.Add(x);
                offset++;
            }
            while (x != 0x0);

            return System.Text.Encoding.Default.GetString(str.ToArray());
        }

        public void WriteString(int where, string wr)
        {
            IntPtr processHandle = OpenProcess(PROCESS_VM_MAGIC, false, process.Id);

            byte[] buffer = System.Text.Encoding.Default.GetBytes(wr);
            int bytesWritten = buffer.Length;

            WriteProcessMemory((int)processHandle, where, buffer, buffer.Length, ref bytesWritten);
        }

        public void WriteFile(uint where, string fn)
        {
            byte[] buf = File.ReadAllBytes(fn);
            WriteArray(where, buf);
        }

    }
}
