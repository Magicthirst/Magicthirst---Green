using System.Collections.Generic;
using Levels.IntentsImpacts;

namespace Levels.Abilities.KillAndDown
{
    public class KillMapper : IIntentToImpactsMapper<KilledIntent>
    {
        public IEnumerable<IImpact> Map(KilledIntent intent)
        {
            yield return new KilledImpact(intent.Victim, intent.Caster);
            yield return new DiedImpact(intent.Victim);
        }
    }

    public class DownedMapper : IIntentToImpactsMapper<DownedIntent>
    {
        public IEnumerable<IImpact> Map(DownedIntent intent)
        {
            yield return new DownedImpact(intent.Victim, intent.Caster);
        }
    }
}