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
}