using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRFramework
{
    //quadblock flags byte 1
    public enum Flags1
    {
        Invisible = 0x01,
        NeverUsed1 = 0x02,
        Reflection = 0x04,
        Kickers = 0x08,
        OutOfBounds = 0x10,
        NeverUsed2 = 0x20,
        TriggerScript = 0x40,
        Reverb = 0x80
    }

    //quadblock flags byte 2
    public enum Flags2
    {
        Kickers = 0x01,
        Unknown1 = 0x02,
        TikiMouth = 0x04,
        Ground1 = 0x08,
        Ground2 = 0x10,
        NotTrack = 0x20,
        OutsideStuff = 0x40,
        InvisibleTriggers = 0x80
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

}