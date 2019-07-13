using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace model_reader
{
    interface IRead
    {
        /// <summary>Reads data from BinaryReader.</summary>
        /// <param name="br">BinaryReader to read data from.</param>
        void Read(BinaryReader br);
    }

    interface IWrite
    {
        /// <summary>Writes data to BinaryWriter.</summary>
        /// <param name="bw">BinaryWriteer to write data to.</param>
        void Write(BinaryWriter bw);
    }

    interface IByteArray
    {
        /// <summary>Returns a structured array of bytes.</summary>
        byte[] ToByteArray();
    }
}