using System.Collections.Generic;
using Levels.IntentsImpacts;
using UnityEngine;

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
            yield return new TargetKilledVictimImpact(intent.Caster, intent.Victim, intent.Context);
            yield return new TargetIsDeadImpact(intent.Victim);
        }
    }
}