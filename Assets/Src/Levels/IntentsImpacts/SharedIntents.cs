using System.Collections.Generic;
using UnityEngine;

namespace Levels.IntentsImpacts
{
    public record ImpactIntent(GameObject Caster, IImpact Impact) : IIntent
    {
        public static ImpactIntent SelfCast(IImpact impact) => new(impact.Target, impact);
    }

    public class ImpactIntentMapper : IIntentToImpactsMapper<ImpactIntent>
    {
        public IEnumerable<IImpact> Map(ImpactIntent intent) => new[] { intent.Impact };
    }
}