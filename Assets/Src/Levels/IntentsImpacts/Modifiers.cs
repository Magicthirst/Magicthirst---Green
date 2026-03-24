using System.Collections.Generic;
using JetBrains.Annotations;
using Levels.Core.Statuses;

namespace Levels.IntentsImpacts
{
    public interface IModifyingImpacts
    {
        IEnumerable<IModifierStatus> Modifiers { get; } 
    }

    public interface IModifierStatus : IStatus
    {
        bool TryMap(IImpact impact, [CanBeNull] out IImpact result);
    }

    public static class ModifyingImpacts
    {
        [CanBeNull]
        public static IImpact ApplyModifiers(this IModifyingImpacts affector, IImpact impact)
        {
            var result = impact;
            foreach (var effect in affector.Modifiers)
            {
                if (effect.TryMap(result, out var temp))
                {
                    result = temp;
                }

                if (result == null)
                {
                    return null;
                }
            }

            return result;
        }
    }
}