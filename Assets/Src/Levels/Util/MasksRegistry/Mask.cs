using System;

namespace Levels.Util.MasksRegistry
{
    [Flags]
    public enum Mask : uint
    {
        Pushable = 1u << 0,
        Damageable = 1u << 1,
        StopsProjectiles = 1u << 2,
        Flesh = 1u << 3,
        // To be continued
        PlayerCharacter = 1u << 30,

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Global
        SomethingToMakeEditorToNotSet_EVERYTHING_Flag = 1u << 31
    }
}