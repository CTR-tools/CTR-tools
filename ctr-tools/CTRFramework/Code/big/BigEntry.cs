using CTRFramework.Shared;
using System.IO;
using System;

namespace CTRFramework.Big
{
    public class BigEntry
    {
        public string Name { get; set; } = String.Empty;
        public int Size => Data != null ? Data.Length : 0;

        // pads size to Meta.SectorSize (2048)
        public int SizePadded => (Size + 2047) >> 11 << 11;

        public byte[] Data { get; set; }

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

            //the logic is that if file doesnt exist, it still creates a valid dummy entry for bigfile.
            if (!File.Exists(path)) return;

            try
            {
                Data = File.ReadAllBytes(path);
            }
            catch
            {
                Helpers.Panic(this, PanicType.Info, $"Unable to access the file: {path}");
            }
        }

        /// <summary>
        /// Saves entry data as a separate file. By default does not create empty files.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path, bool includeEmpty = false)
        {
            if (Size == 0 && !includeEmpty) return;
            
            Helpers.WriteToFile(Helpers.PathCombine(path, Name), Data);
        }

        /// <summary>
        /// Generic method to parse entry data as a CTR specific class. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ParseAs<T>() where T : IRead, new()
        {
            using (var br = new BinaryReaderEx(new MemoryStream(Data)))
            {
                return Instance<T>.FromReader(br, 0);
            }
        }

        public override string ToString() => $"{Name} [{(Size / 1024) + 1} kb]";
    }
}
