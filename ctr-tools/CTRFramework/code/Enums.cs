using System;

namespace CTRFramework
{
    //quadblock flags byte 1
    [Flags]
    public enum QuadFlags
    {
        Invisible = 1 << 0,
        NeverUsed1 = 1 << 1,
        Reflection = 1 << 2,
        Kickers = 1 << 3,
        OutOfBounds = 1 << 4,
        NeverUsed2 = 1 << 5,
        TriggerScript = 1 << 6,
        Reverb = 1 << 7,
        KickersToo = 1 << 8,
        KillRacer = 1 << 9, //confirmed, terrain kills racer
        TikiMouth = 1 << 10,
        Renderable = 1 << 11, //confirmed, setting this to 1 makes quad visible
        Collidable = 1 << 12, //confirmed, setting this to 1 makes quad collidable, able to drive
        NotTrack = 1 << 13,
        OutsideStuff = 1 << 14,
        InvisibleTriggers = 1 << 15
    }

    //defines mesh quality while exporting to OBJ
    public enum Detail
    {
        High, Low
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