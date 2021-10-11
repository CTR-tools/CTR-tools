using CTRFramework.Shared;
using System;
using System.Collections.Generic;

namespace CTRFramework
{
    public interface IReadWrite : IRead, IWrite
    {
    }

    public interface IRead
    {
        /// <summary>Reads data from BinaryReader.</summary>
        /// <param name="br">BinaryReaderEx instance to read data from.</param>
        void Read(BinaryReaderEx br);
    }

    public interface IWrite
    {
        /// <summary>Writes data to BinaryWriter.</summary>
        /// <param name="bw">BinaryWriterEx instance to write data to.</param>
        void Write(BinaryWriterEx bw, List<UIntPtr> patchTable = null);
    }
}