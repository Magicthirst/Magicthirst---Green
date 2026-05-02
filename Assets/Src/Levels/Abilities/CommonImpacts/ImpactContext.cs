using System;

namespace Levels.Abilities.CommonImpacts
{
    [Flags]
    public enum ImpactContext : uint
    {
        None = 0u,
        AntiTough = 1u << 0,
        Chaos = 1u << 1 | AntiTough,

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Global
        SomethingToMakeEditorToNotSet_EVERYTHING_Flag = uint.MaxValue
    }
}