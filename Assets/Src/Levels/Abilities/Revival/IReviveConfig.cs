using JetBrains.Annotations;
using Levels.Core.Statuses;

namespace Levels.Abilities.Revival
{
    public interface IReviveConfig
    {
        int InstantHealthAddition { get; }
        [CanBeNull] PeriodicHeal PeriodicHeal { get; }
    }
}