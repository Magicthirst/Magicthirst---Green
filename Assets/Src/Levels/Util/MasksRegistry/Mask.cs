using System;

namespace Levels.Util.MasksRegistry
{
    [Flags]
    public enum Mask : uint
    {
        Pushable = 1 << 0,
        // To be continued
    }
}