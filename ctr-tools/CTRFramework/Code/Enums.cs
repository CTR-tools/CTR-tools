using System;

namespace CTRFramework
{
    #region howl enums

    [Flags]
    enum HowlExportFlags
    {
        All = -1,
        SamplesVag = 1 << 0,
        SamplesWav = 1 << 1,
        Sequences = 1 << 2,
        Midis = 1 << 3,
        Banks = 1 << 4,
    }

    //defines instrument type in CSEQ
    public enum InstType
    {
        Long,
        Short
    }

    #endregion

    public enum Level
    {
        DingoCanyon = 0,
        DragonMines,
        BlizzardBluff,
        CrashCove,
        TigerTemple = 4,
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
        SlideColiseum = 16,
        TurboTrack,
        NitroCourt,
        RampageRuins,
        ParkingLot,
        SkullRock,
        NorthBowl,
        RockyRoad,
        LabBasement,
        hub1,
        hub2,
        hub3,
        hub4,
        hub5
    }

    public enum Cutscenes
    {
        IntroBox = 0,
        RaceToday,
        CanyonCoco,
        PassTiny,
        TemplePolar,
        SkywayDingodile,
        SewerCortex,
        Oxide1,
        SleepingCrash,
        Oxide2,
        Oxide3,
        Oxide4
    }

    [Flags]
    public enum CtrDrawFlags
    {
        s = 1 << 7,     //starts a new tristrip
        l = 1 << 6,     //swap 1st vertex of the new face with the 1st from the last one
        n = 1 << 5,     //defines whether the face normal should be flipped
        d = 1 << 4,     //single sided if set
        k = 1 << 3,     //tells whether it should take color from scratchpad or ram. original models all use this, but custom result in random colors.
        v = 1 << 2,     //takes vertex from stack instead of vertex array
        b3 = 1 << 1,    //assumed never used
        b4 = 1 << 0     //assumed never used
    }

    [Flags]
    public enum ExportFlags
    {
        None = 0,
        MeshLow = 1 << 0,
        TexLow = 1 << 1,
        MeshMed = 1 << 2,
        TexMed = 1 << 3,
        MeshHi = 1 << 4,
        TexHigh = 1 << 5,
        Models = 1 << 6,
        TexModels = 1 << 7,
        SkyBox = 1 << 8,
        All = -1
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


    //quadblock flags
    [Flags]
    public enum QuadFlags
    {
        None = 0,
        Invisible = 1 << 0,     //check
        MoonGravity = 1 << 1,   //triggers MG in Oxide Station
        Reflection = 1 << 2,    //used in snow levels
        Kickers = 1 << 3,       //?? maybe denotes that you should be awarded extra turbo for landing?
        OutOfBounds = 1 << 4,   //check
        NeverUsed = 1 << 5,     //??
        TriggerScript = 1 << 6, //turbo pads, but not only. maybe quad has a linked script?
        Reverb = 1 << 7,        //reverberation (echo), used in various indoor areas or tunnels
        KickersToo = 1 << 8,    //??
        KillRacer = 1 << 9,     //these quads trigger mask
        TikiMouth = 1 << 10,    //only spotted on tiger temple
        Unknown = 1 << 11,      //mostly same as ground, but more.
        Ground = 1 << 12,       //ground, makes it collidable
        Wall = 1 << 13,         //walls, if it's flat, char will keep bouncing
        NoCollision = 1 << 14,  //visible stuff like water and outside areas
        InvisibleTriggers = 1 << 15 //invisible stuff like triggers
    }


    [Flags]
    public enum VisDataFlags
    {
        None = 0,
        Leaf = 1 << 0,      // defines if entry is leaf or branch, other bits assumed to only be used 1 at a time (? check)
        Water = 1 << 1,     // renders quads node as water
        Unk2 = 1 << 2,      // ?? caps on cove bridge
        Subdiv4x1 = 1 << 3, // reduces 4x4 subdivision to 4x1
        Subdiv4x2 = 1 << 4, // reduces 4x4 subdivision to 4x2
        Unk5 = 1 << 5,      // additive blended?
        Hidden = 1 << 6,    // doesn't render child quads
        Unk7 = 1 << 7,      // ??
        All = -1
    }


    public enum CtrWriteMode
    {
        Header,
        Data
    }

    //defines mesh quality while exporting to OBJ
    public enum Detail
    {
        High, Med, Low, Models //hacky hack to have another folder for model textures
    }

    public enum VecFormat
    {
        Numbers, CommaSeparated, Braced, Hex
    }


    public enum CharIndex
    {
        Crash = 0,
        Cortex = 1,
        Tiny = 2,
        Coco = 3,
        Ngin = 4,
        Dingo = 5,
        Polar = 6,
        Pura = 7,
        Pinstripe = 8,
        Papu = 9,
        Roo = 10,
        Joe = 11,
        Ntropy = 12,
        Pen = 13,
        Fake = 14,
        Oxide = 15
    }

    public enum CtrThreadID
    {
        None = -1,

        SingleFruit = 0x02,

        CrateNitro = 0x06,
        CrateFruit = 0x07,
        CrateWeapon = 0x08,

        TempleFlameJet = 0x12, //18
        PapuPlant = 0x13, //19

        MinesKart = 0x21, //33,
        BluffSnowball = 0x22, //34,

        CanyonArmadillo = 0x24, //36,
        BlimpBlades = 0x25, //37,
        CrateExplosion = 0x26,
        CrateTNT = 0x27,

        Rocket = 0x29,

        WarpedBurst = 0x2B,
        Turbo0 = 0x2C,
        Turbo1 = 0x2D,
        Turbo2 = 0x2E,
        Turbo3 = 0x2F,
        Turbo4 = 0x30,
        Turbo5 = 0x31,
        Turbo6 = 0x32,
        Turbo7 = 0x33,

        Warpball = 0x36,
        FruitDispenser = 0x37, //55 ?? it's wumpa, but never spawned
        BigNumbers = 0x38, //56 - comes with lods for each number
        AkuMouth = 0x39,
        UkaMouth = 0x3A,
        Bomb = 0x3B,

        AkuBeam = 0x3E,
        UkaBeam = 0x40,

        Cloud = 0x42, //red beaker cloud?
        Wake = 0x43, //?? water stuff around model?
        ShockwaveRed = 0x44,
        ShockwaveGreen = 0x45,
        BeakerRed = 0x46,
        BeakerGreen = 0x47,
        BeakerBreakRed = 0x48, // from demo
        BeakerBreakGreed = 0x49, // from demo
        SisHead = 0x4A,

        PassSeal = 0x4C, //76

        SewerBarrel = 0x4E, //78

        CavesTurtle = 0x51, //81
        CastleSpider = 0x52, // 82
        CastleSpiderShadow = 0x53,
        CavesFireball = 0x54, //84
        LabsDrum = 0x55, // 85
        Shield = 0x56, //blue shield?
        Pipe = 0x57, //87
        //0x58
        Vent = 0x59, //89
        ShieldDark = 0x5A, //blue shield?
        CastleSign = 0x5B, //91
        CrateRelic1 = 0x5C, //92
        Highlight = 0x5D, // ??
        ShieldGreen = 0x5E,
        Gem = 0x5F,
        Crystal = 0x60, //96
        Relic = 0x61, //97
        Trophy = 0x62, //98
        Key = 0x63, //99
        CrateRelic2 = 0x64, //100
        CrateRelic3 = 0x65, //101
        IntroTrophy = 0x66,
        IntroCrash = 0x67,
        RingTop = 0x68,
        RingBottom = 0x69,
        IntroCtr = 0x6A,
        IntroBanner = 0x6B,
        WarpPad = 0x6C, //108
        Big0 = 0x6D, //these bigs are used in warppad requirements
        Big9 = 0x6E,
        BigX = 0x6F,
        TempleTeeth = 0x70, //112 - trigger secret passage script?
        StartText = 0x71, //unused tiny arena banner
        SaveScreen = 0x72, //114
        GaragePin = 0x73, //115
        GaragePapu = 0x74, //116
        GarageRoo = 0x75, //117
        GarageJoe = 0x76, //118
        GarageOxide = 0x77, //119
        Scan = 0x78,
        //0x79?
        HubDoor1 = 0x7A, //122  hub door with a single lock
        WarpBeam = 0x7B,
        WarpBottomRing = 0x7C,
        Token = 0x7D, //125
        PodiumCrash = 0x7E,
        PodiumCortex = 0x7F,
        PodiumTiny = 0x80,
        PodiumCoco = 0x81,
        PodiumNgin = 0x82,
        PodiumDingo = 0x83,
        PodiumPolar = 0x84,
        PodiumPura = 0x85,
        PodiumPinstripe = 0x86,
        PodiumPapu = 0x87,
        PodiumRoo = 0x88,
        PodiumJoe = 0x89,
        PodiumNtropy = 0x8A,
        PodiumPenta = 0x8B,
        PodiumFake = 0x8C,
        PodiumOxide = 0x8D,
        GarageTop = 0x8E,
        PodiumAmi = 0x8F,        //check order
        PodiumIsabella = 0x90,
        PodiumLiz = 0x91,
        PodiumMegumi = 0x92,
        LetterC = 0x93, //147
        LetterT = 0x94, //148
        LetterR = 0x95, //149
        IntroCrashSleep = 0x96,
        IntroCoco = 0x97,
        IntroCortex = 0x98,
        IntroTiny = 0x99,
        IntroPolar = 0x9A,
        IntroDingo = 0x9B,
        IntroOxideHead = 0x9C,
        IntroCocoKartBeta = 0x9D, //shared between multiple models
        IntroTinyKart = 0x9E,
        IntroDingoKart = 0x9F,
        BossBody = 0xA0, //shared between multiple models
        IntroButterfly = 0xA1, //shared between multiple of models
        IntroGrass = 0xA2,
        OxideLilShip = 0xA3,
        OxideCam09 = 0xA4,
        IntroOxideBody = 0xA5,
        StartBanner = 0xA6, //166 - check
        HubDoor2 = 0xA7, // hub door without locks
        Podium = 0xA8,
        BossHeadPinstripe = 0xA9,
        BossHeadPapu = 0xAA,
        BossHeadRoo = 0xAB,
        BossHeadJoe = 0xAC,
        BossHeadOxide = 0xAD,
        //0xAE
        DingoFire = 0xAF, //the one dingo bursts in air?
        Tombstone = 0xB0,
        AkuBig = 0xB1,
        UkaBig = 0xB2,

        HubDoor3 = 0xB5, // hub door with 2 locks
        NdiBox01 = 0xB6,
        NdiBox02 = 0xB7,
        NdiBox02_ = 0xB8,
        //0xB9
        NdiBox02A = 0xBA,
        NdiBox03 = 0xBB,
        NdiBoxCode = 0xBC,
        NdiBoxGlow = 0xBD,
        NdiBoxLid = 0xBE,
        NdiBoxLid2 = 0xBF,
        NdiBoxParticle = 0xC0,
        NdiBoxCrash = 0xC1,
        NdiBoxCoco = 0xC2,
        NdiBoxCortex = 0xC3,
        NdiBoxTiny = 0xC4,
        //C5, C6
        NdiBoxAku = 0xC7,
        NdiBoxOxide = 0xC8,
        NdiBoxLidB = 0xC9,
        NdiBoxLidC = 0xCA,
        NdiBoxLidD = 0xCB,
        IntroFlash = 0xCC,
        IntroDoors = 0xCD,
        SelectCrash = 0xCE,
        SelectCortex = 0xCF,
        SelectTiny = 0xD0,
        SelectCoco = 0xD1,
        SelectNgin = 0xD2,
        SelectDingo = 0xD3,
        SelectPolar = 0xD4,
        SelectPura = 0xD5,
        EndDoors = 0xD6,
        EndFlash = 0xD7,
        EndOxideHead = 0xD8,
        EndOxideHead2 = 0xD9,
        EndBigShip = 0xDA,
        EndLilShip = 0xDB,
        EndOxideCam = 0xDC,
        EndOxideCam2 = 0xDD,
        //0xDE
        IntroOxideSpeaker = 0xDF,
        IntroSparks = 0xE0,
        HubSign = 0xE1 //225
    }

    public enum Vcolor
    {
        Default,
        Morph,
        Flag
    }
}