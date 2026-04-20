using System.Collections.Generic;
using Levels.IntentsImpacts;

namespace Levels.Abilities.Kill
{
    public class KillMapper : IIntentToImpactsMapper<KilledIntent>
    {
        public IEnumerable<IImpact> Map(KilledIntent intent)
        {
            yield return new KilledImpact(intent.Victim, intent.Caster);
            yield return new DiedImpact(intent.Victim);
        }
    }
}