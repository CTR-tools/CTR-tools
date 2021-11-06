using CTRFramework.Shared;

namespace CTRFramework.Sound.CSeq
{
    public class CseqRootEvent
    {
    }

    public class CseqNoteOnEvent : CseqRootEvent
    {
        public byte NoteNumber;
        public byte Velocity;

        public void Read(BinaryReaderEx br)
        {
            NoteNumber = br.ReadByte();
            Velocity = br.ReadByte();
        }

        public void Write(BinaryWriterEx br)
        {
            br.Write(NoteNumber);
            br.Write(Velocity);
        }

        public override string ToString()
        {
            return $"CseqNoteOnEvent: {NoteNumber}, {Velocity}";
        }
    }

    public class CseqNoteOffEvent : CseqRootEvent
    {
        public byte NoteNumber;

        public void Read(BinaryReaderEx br)
        {
            NoteNumber = br.ReadByte();
        }
        public void Write(BinaryWriterEx br)
        {
            br.Write((byte)CSEQEvent.NoteOff);
            br.Write(NoteNumber);
        }

        public override string ToString()
        {
            return $"CseqNoteOffEvent: {NoteNumber}";
        }
    }
}