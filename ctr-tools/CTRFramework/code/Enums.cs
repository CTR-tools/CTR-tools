using System;

namespace CTRFramework
{
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
        Invisible = 1 << 0,     //check
        MoonGravity = 1 << 1,   //triggers MG in 
        Reflection = 1 << 2,    //used in snow levels
        Kickers = 1 << 3,       //?? maybe denotes that you whould be awarded extra turbo for landing?
        OutOfBounds = 1 << 4,   //check
        NeverUsed2 = 1 << 5,    //??
        TriggerScript = 1 << 6, //turbo pads, but not only. maybe quad has a linked script?
        Reverb = 1 << 7,        //used in various indoor areas
        KickersToo = 1 << 8,    //??
        KillRacer = 1 << 9,     //these quads trigger mask
        TikiMouth = 1 << 10,    //only spotted on tiny arena
        Renderable = 1 << 11,   //mostly same as ground, but more.
        Ground = 1 << 12,       //ground, makes it collidable
        Wall = 1 << 13,         //walls
        OutsideStuff = 1 << 14, //third state? looks like all quads on the track are either 12, 13 or 14
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
        SingleFruit = 2,
        CrateNitro = 6,
        CrateFruit = 7,
        CrateWeapon = 8,
        StateBurned = 18,
        StateEaten = 19,
        CrateTNT = 0x27,
        StateSquished = 33,
        StateSquishedBall = 34,
        StateRotatedArmadillo = 36,
        StateKilledBlades = 37,
        Pipe = 0x57,
        Vent = 0x59,
        Crystal = 0x60,
        pass_seal = 76,
        StateSquishedBarrel = 78,
        StateTurleJump = 81,
        StateRotatedSpider = 82,
        StateBurnedInAir = 84,
        labs_drum = 85,
        StateCastleSign = 91, //what?
        WarpPad = 108,
        Teeth = 112,  //trigger secret passage script?
        SaveScreen = 114,

        GaragePin = 115,
        GaragePapu = 116,
        GarageRoo = 117,
        GarageJoe = 118,
        GarageOxide = 119,

        DoorUnknown = 122,

        LetterC = 147,
        LetterT = 148,
        LetterR = 149,
        FinishLap = 166, //check

        HubDoor = 225,
        CrateRelic1 = 0x5C,
        CrateRelic2 = 0x64,
        CrateRelic3 = 0x65

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