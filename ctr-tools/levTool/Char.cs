using System.IO;

namespace CTRTools
{
    class Char
    {
        public uint ptrWheelTex;    // 4 bytes	- ptr to wheel textures
        public ushort wheelScale;   // 2 bytes - wheels scale
        public ushort wheelTurn;    // 2 bytes - front wheels rotation (0x0000 - center, 0x0040 - far left, 0xFFC0 - far right)
        public uint unk1;           // 4 bytes - unknown, rewrites every frame
        public ushort clockTimer;   // 2 bytes - clock effect countdown
        public ushort unk2;         // 2 bytes - unknown, rewrites every frame
        public uint unk3;           // 4 bytes - unknown, resets to 0 when O is pressed
        public uint unk4;           // 4 bytes - unknown, resets to 0 when O is pressed 2nd time
        public uint scale;          // 4 bytes - affects scale, not the squeezed one
        public uint somePtr;        // 4 bytes - some ptr
        public uint unk5;           // 4 bytes - unknown
        public uint respawnTimer;   // 4 bytes - invincibility countdown (used in 2p battle?)
        public uint burnedTimer;    // 4 bytes - burned countdown, ends up invisible for some reason
        public uint unk6;           // 4 bytes - unknown seems unchanged
        public byte wumpasCount;    // 1 byte - wumpas
        public byte[] skip = new byte[5];    // 5 bytes - unknown
        public byte curWeapon;      // 1 byte - current weapon
        public byte numCharges;     // 1 byte - number of charges

        // 2 bytes - turbo countdown (negative = infinite)
        // 2 bytes - current weapon roulette value
        // 2 bytes - some countdown that triggers by mask activation
        // 7 bytes - unknown
        // 1 byte - clock effect again
        // 2 bytes - last jump height
        // 2 bytes - some countdown, probably related to turbo
        // 2 bytes - turn power, signed - 0x18 - normal turn, 0x40 - turn with brake
        // 2 bytes - something related to turbo as well
        // 2 bytes - number of consecutive turbos
        // 2 bytes - unknown, overwrites each frame, changes when you drive into wall, lol
        // 7 bytes - unknown

        //12 uints - pointers to smth
        //uint overwrites



        public Char(BinaryReader br)
        {
            ptrWheelTex = br.ReadUInt32();
            wheelScale = br.ReadUInt16();
            wheelTurn = br.ReadUInt16();    // 2 bytes - front wheels rotation (0x0000 - center, 0x0040 - far left, 0xFFC0 - far right)
            unk1 = br.ReadUInt32();           // 4 bytes - unknown, rewrites every frame
            clockTimer = br.ReadUInt16();   // 2 bytes - clock effect countdown
            unk2 = br.ReadUInt16();         // 2 bytes - unknown, rewrites every frame
            unk3 = br.ReadUInt32();           // 4 bytes - unknown, resets to 0 when O is pressed
            unk4 = br.ReadUInt32();          // 4 bytes - unknown, resets to 0 when O is pressed 2nd time
            scale = br.ReadUInt32();          // 4 bytes - affects scale, not the squeezed one
            somePtr = br.ReadUInt32();        // 4 bytes - some ptr
            unk5 = br.ReadUInt32();           // 4 bytes - unknown
            respawnTimer = br.ReadUInt32();   // 4 bytes - invincibility countdown (used in 2p battle?)
            burnedTimer = br.ReadUInt32();    // 4 bytes - burned countdown, ends up invisible for some reason
            unk6 = br.ReadUInt32();          // 4 bytes - unknown seems unchanged
            wumpasCount = br.ReadByte();    // 1 byte - wumpas
            skip = br.ReadBytes(5);    // 5 bytes - unknown
            curWeapon = br.ReadByte();      // 1 byte - current weapon
            numCharges = br.ReadByte();     // 1 byte - number of charges
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ptrWheelTex);
            bw.Write(wheelScale);
            bw.Write(wheelTurn);    // 2 bytes - front wheels rotation (0x0000 - center, 0x0040 - far left, 0xFFC0 - far right)
            bw.Write(unk1);           // 4 bytes - unknown, rewrites every frame
            bw.Write(clockTimer);   // 2 bytes - clock effect countdown
            bw.Write(unk2);         // 2 bytes - unknown, rewrites every frame
            bw.Write(unk3);           // 4 bytes - unknown, resets to 0 when O is pressed
            bw.Write(unk4);          // 4 bytes - unknown, resets to 0 when O is pressed 2nd time
            bw.Write(scale);          // 4 bytes - affects scale, not the squeezed one
            bw.Write(somePtr);        // 4 bytes - some ptr
            bw.Write(unk5);           // 4 bytes - unknown
            bw.Write(respawnTimer);   // 4 bytes - invincibility countdown (used in 2p battle?)
            bw.Write(burnedTimer);    // 4 bytes - burned countdown, ends up invisible for some reason
            bw.Write(unk6);          // 4 bytes - unknown seems unchanged
            bw.Write(wumpasCount);    // 1 byte - wumpas
            bw.Write(skip);    // 5 bytes - unknown
            bw.Write(curWeapon);      // 1 byte - current weapon
            bw.Write(numCharges);     // 1 byte - number of charges
        }
    }
}
