using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace model_reader
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
}
