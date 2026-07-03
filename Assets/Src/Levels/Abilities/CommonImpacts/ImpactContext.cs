using System;
using static Levels.Abilities.CommonImpacts.ImpactContext;

namespace Levels.Abilities.CommonImpacts
{
    [Flags]
    public enum ImpactContext : uint
    {
        None = 0,
        AntiTough = 1 << 0,
        Chaos = 1 << 1 | AntiTough,
        HealOnKill = 1 << 2,
        ResultOfBadParry = 1 << 3,
        DidBonusDamage = 1 << 4,
        DidVeryBonusDamage = 1 << 5,
        DidReducedDamage = 1 << 6,

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Global
        SomethingToMakeEditorToNotSet_EVERYTHING_Flag = 1u << 31
    }

    public static class ImpactContextExtensions
    {
        public static ImpactContext UpgradeBonusDamageRank(this ImpactContext context)
        {
            return
                context.HasFlag(DidReducedDamage)   ? context & ~DidReducedDamage :
                context.HasFlag(DidVeryBonusDamage) ? context :
                context.HasFlag(DidBonusDamage)     ? context | DidVeryBonusDamage :
                                                      context | DidBonusDamage;
        }

        public static ImpactContext DowngradeBonusDamageRank(this ImpactContext context)
        {
            return
                context.HasFlag(DidVeryBonusDamage) ? context & ~DidVeryBonusDamage :
                context.HasFlag(DidBonusDamage)     ? context & ~DidBonusDamage :
                context.HasFlag(DidReducedDamage)   ? context :
                                                      context | DidReducedDamage;
        }
    }
}