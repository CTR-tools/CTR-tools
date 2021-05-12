using CTRFramework.Shared;
using System;
using System.IO;

namespace CTRFramework.Big
{
    public class BigEntry
    {
        public int Index;
        public string Name;
        public int Offset;
        public int Size
        {
            get { return Data != null ? Data.Length : 0; }
        }

        public int SizePadded
        {
            get { return (Size + 2047) >> 11 << 11; }
        }

        public byte[] Data;

        public BigEntry()
        {
        }

        public BigEntry(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        /// <summary>
        /// Creates Big Entry from an external file
        /// </summary>
        /// <param name="path">Path to file</param>
        public BigEntry(string path, string name = null)
        {
            Name = name != null ? name : path;
            Data = new byte[0];

            if (File.Exists(path))
            {
                try
                {
                    Data = File.ReadAllBytes(path);
                }
                catch
                {
                    Console.WriteLine("Unable to access the file.");
                }
            }
        }

        public void Save(string path)
        {
            Helpers.WriteToFile(Path.Combine(path, Name), Data);
        }

        public T ParseAs<T>() where T : IRead, new ()
        {
            using (BinaryReaderEx br = new BinaryReaderEx(new MemoryStream(Data)))
            {
                return Instance<T>.FromReader(br, 0);
            }
        }

        public override string ToString()
        {
            return $"{Name} [{(Size / 1024) + 1} kb]";
        }
    }
}
