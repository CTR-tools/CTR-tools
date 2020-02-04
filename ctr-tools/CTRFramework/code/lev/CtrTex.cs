using CTRFramework.Shared;

namespace CTRFramework
{
    public class CtrTex : IRead
    {
        public TextureLayout[] midlods = new TextureLayout[3];
        public TextureLayout[] hi = new TextureLayout[4];
        public uint ptrHi;

        public CtrTex(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            long pos = br.BaseStream.Position;

            for (int i = 0; i < 3; i++)
                midlods[i] = new TextureLayout(br);

            ptrHi = br.ReadUInt32();

            if (ptrHi != 0)
            {
                if (ptrHi > br.BaseStream.Length)
                {
                    //there must be some flag that defines hi tex existence
                    Helpers.Panic(this, "hi tex ptr overflow at " + pos.ToString("X8"));
                }
                else
                {
                    br.Jump(ptrHi);

                    for (int i = 0; i < 4; i++)
                    {
                        hi[i] = new TextureLayout(br);
                    }
                }
            }
        }
    }
}
