using System.Collections.Generic;
using Levels.IntentsImpacts;

namespace Levels.Abilities.KillAndDown
{
    public class DownedMapper : IIntentToImpactsMapper<DownedIntent>
    {
        public IEnumerable<IImpact> Map(DownedIntent intent)
        {
            yield return new DownedImpact(intent.Victim, intent.Caster);
        }
    }

    public class KilledMapper : IIntentToImpactsMapper<KilledIntent>
    {
        public IEnumerable<IImpact> Map(KilledIntent intent)
        {
            yield return new TargetKilledVictimImpact(intent.Victim, intent.Caster, intent.Context);
            yield return new TargetIsDeadImpact(intent.Victim);
        }
    }
}