using System;

namespace Voltage.Story.StoryDivisions
{
    [Flags]
    public enum LockType
    {
        None = 0x0,
        Progress = 0x1,
        Favorability = 0x2,
		Clothing = 0x4
    }
}

