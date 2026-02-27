using System.Collections.Generic;

namespace Levels.IntentsImpacts
{
    public record ImpactIntent(IImpact Impact) : IIntent;

    public class ImpactIntentMapper : IIntentToImpactsMapper<ImpactIntent>
    {
        public IEnumerable<IImpact> Map(ImpactIntent intent) => new[] { intent.Impact };
    }
}