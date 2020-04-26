using System;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class BigEntry
    {
        public int Index;
        public string Name;
        public int Size;
        public int Offset;
        public int SizePadded
        {
            get
            {
                int val = Size;
                while (SizePadded % 2048 != 0) val++;
                return val;
            }
        }

        public byte[] Data;

        public BigEntry()
        {
        }

        /// <summary>
        /// Reads indexed Big Entry from BigFile
        /// </summary>
        /// <param name="br">Source stream</param>
        /// <param name="name">Entry name</param>
        /// <param name="i">Entry index</param>
        public BigEntry(BinaryReaderEx br, string name, int i)
        {
            Index = i;
            Name = name;
            br.Jump(4);

            if (Index >= br.ReadInt32() || Index < 0)
                throw new ArgumentOutOfRangeException("Big entry index out of range.");

            br.Skip(i * 8);

            Offset = br.ReadInt32() * Meta.SectorSize;
            Size = br.ReadInt32();

            if (Offset + Size > br.BaseStream.Length)
            {
                Console.WriteLine("Attempt to read data beyond binary stream.");
                throw new ArgumentOutOfRangeException("Attempt to read data beyond binary stream.");
            }

            br.Jump(Offset);
            Data = br.ReadBytes(Size);
        }

        /// <summary>
        /// Creates Big Entry from an external file
        /// </summary>
        /// <param name="path">Path to file</param>
        public BigEntry(string path, string name = null)
        {
            Name = name != null ? name : path;
            Data = new byte[0];
            Size = 0;

            if (File.Exists(path))
            {
                try
                {
                    Data = File.ReadAllBytes(path);
                    Size = Data.Length;
                }
                catch
                {
                    Console.WriteLine("Unable to access the file.");
                }
            }
        }

        public override string ToString()
        {
            return Name + " [" + ((Size / 1024) + 1) + "kb]";
        }
    }
}
