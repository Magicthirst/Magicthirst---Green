using System;

namespace Levels
{
    [Flags]
    public enum LevelActivityMask
    {
        None = 0,

        Gameplay = 1 << 0,
        Tutorial = 1 << 1,
        Pause = 1 << 2
    }
}