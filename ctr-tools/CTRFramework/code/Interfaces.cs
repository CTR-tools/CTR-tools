using System.IO;

namespace CTRFramework
{
    public interface IRead
    {
        /// <summary>Reads data from BinaryReader.</summary>
        /// <param name="br">BinaryReader to read data from.</param>
        void Read(BinaryReader br);
    }

    public interface IWrite
    {
        /// <summary>Writes data to BinaryWriter.</summary>
        /// <param name="bw">BinaryWriteer to write data to.</param>
        void Write(BinaryWriter bw);
    }

    public interface IByteArray
    {
        /// <summary>Returns a structured array of bytes.</summary>
        byte[] ToByteArray();
    }
}