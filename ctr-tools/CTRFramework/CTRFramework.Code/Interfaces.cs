using CTRFramework.Shared;

namespace CTRFramework
{
    public interface IReadWrite : IRead, IWrite
    {
    }

    public interface IRead
    {
        /// <summary>Reads data from BinaryReader.</summary>
        /// <param name="br">BinaryReaderEx to read data from.</param>
        void Read(BinaryReaderEx br);
    }

    public interface IWrite
    {
        /// <summary>Writes data to BinaryWriter.</summary>
        /// <param name="bw">BinaryWriteer to write data to.</param>
        void Write(BinaryWriterEx bw);
    }

    public interface IByteArray
    {
        /// <summary>Returns a structured array of bytes.</summary>
        byte[] ToByteArray();
    }
}