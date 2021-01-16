using System;

namespace CTRFramework
{
    public enum Level
    {
        DingoCanyon = 0,
        DragonMines,
        BlizzardBluff,
        CrashCove,
        TinyTemple = 4,
        PapuPyramid,
        RooTubes,
        HotAirSkyway,
        SewerSpeedway = 8,
        MysteryCaves,
        CortexCastle,
        NGinLabs,
        PolarPass = 12,
        OxideStation,
        CocoPark,
        TinyArena,
        Coliseum = 16,
        TurboTrack,
        battle1,
        battle2,
        battle3,
        battle4,
        battle5,
        battle6,
        battle7,
        hub1,
        hub2,
        hub3,
        hub4,
        hub5
    }

    [Flags]
    public enum ExportFlags
    {
        MeshLow = 1 << 0,
        TexLow = 1 << 1,
        MeshMed = 1 << 2,
        TexMed = 1 << 3,
        MeshHi = 1 << 4,
        TexHigh = 1 << 5,
        Models = 1 << 6,
        SkyBox = 1 << 7,
        All = -1
    }


    public enum RotateFlipType
    {
        None = 0,
        Rotate90 = 1,
        Rotate180 = 2,
        Rotate270 = 3,
        FlipRotate270 = 4,
        FlipRotate180 = 5,
        FlipRotate90 = 6,
        Flip = 7
    }

    public enum FaceMode
    {
        Normal = 0,
        SingleUV1 = 1,
        SingleUV2 = 2,
        Unknown = 3
    }

    public enum TerrainFlags
    {
        Asphalt = 0,
        Dirt = 1,
        Grass = 2,
        Wood = 3,
        Water = 4,
        Stone = 5,
        Ice = 6,
        Track = 7,
        IcyRoad = 8,
        Snow = 9,
        None = 10,
        HardPack = 11,
        Metal = 12,
        FastWater = 13,
        Mud = 14,
        SideSlip = 15,
        RiverAsphalt = 16,
        SteamAsphalt = 17,
        OceanAsphalt = 18,
        SlowGrass = 19,
        SlowDirt = 20
    }


    //quadblock flags byte 1
    [Flags]
    public enum QuadFlags
    {
        None = 0,
        Invisible = 1 << 0,     //check
        MoonGravity = 1 << 1,   //triggers MG in Oxide Station
        Reflection = 1 << 2,    //used in snow levels
        Kickers = 1 << 3,       //?? maybe denotes that you whould be awarded extra turbo for landing?
        OutOfBounds = 1 << 4,   //check
        NeverUsed = 1 << 5,    //??
        TriggerScript = 1 << 6, //turbo pads, but not only. maybe quad has a linked script?
        Reverb = 1 << 7,        //used in various indoor areas or tunnels
        KickersToo = 1 << 8,    //??
        KillRacer = 1 << 9,     //these quads trigger mask
        TikiMouth = 1 << 10,    //only spotted on tiger temple
        Unknown = 1 << 11,   //mostly same as ground, but more.
        Ground = 1 << 12,       //ground, makes it collidable
        Wall = 1 << 13,         //walls, if it's flat, char will keep bouncing
        NoCollision = 1 << 14,  //visible stuff like water and outside areas
        InvisibleTriggers = 1 << 15 //invisible stuff like triggers
    }

    //defines mesh quality while exporting to OBJ
    public enum Detail
    {
        High, Med, Low
    }

    public enum VecFormat
    {
        Numbers, CommaSeparated, Braced
    }


    public enum CTREvent
    {
        Nothing = -1,

        SingleFruit = 0x02,

        CrateNitro = 0x06,
        CrateFruit = 0x07,
        CrateWeapon = 0x08,

        StateBurned = 0x12, //18
        StateEaten = 0x13, //19

        StateSquished = 0x21, //33,
        StateSquishedBall = 0x22, //34,

        StateRotatedArmadillo = 0x24, //36,
        StateKilledBlades = 0x25, //37,

        CrateTNT = 0x27,

        FruitDispenser = 0x37, //55
        Big1 = 0x38, //56 - maybe next 7 are for 2-8?

        pass_seal = 0x4C, //76

        StateSquishedBarrel = 0x4E, //78

        StateTurleJump = 0x51, //81
        StateRotatedSpider = 0x52, // 82

        StateBurnedInAir = 0x54, //84
        labs_drum = 0x55, // 85

        Pipe = 0x57, //87

        Vent = 0x59, //89

        StateCastleSign = 0x5B, //91, //what?
        CrateRelic1 = 0x5C, //92

        Crystal = 0x60, //96
        Relic = 0x61, //97
        Trophy = 0x62, //98
        Key = 0x63, //99
        CrateRelic2 = 0x64, //100
        CrateRelic3 = 0x65, //101

        WarpPad = 0x6C, //108

        Teeth = 0x70, //112 - trigger secret passage script?

        SaveScreen = 0x72, //114
        GaragePin = 0x73, //115
        GaragePapu = 0x74, //116
        GarageRoo = 0x75, //117
        GarageJoe = 0x76, //118
        GarageOxide = 0x77, //119

        DoorUnknown = 0x7A, //122

        Token = 0x7D, //125

        LetterC = 0x93, //147
        LetterT = 0x94, //148
        LetterR = 0x95, //149

        FinishLap = 0xA6, //166 - check

        HubDoor = 0xE1 //225
    }

    public enum Vcolor
    {
        Default,
        Morph,
        Flag
    }

    //defines instrument type in CSEQ
    public enum InstType
    {
        Long,
        Short
    }
}