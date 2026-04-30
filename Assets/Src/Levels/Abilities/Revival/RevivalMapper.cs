using System.Collections.Generic;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;

namespace Levels.Abilities.Revival
{
    public class RevivalMapper : IIntentToImpactsMapper<ReviveIntent>
    {
        public IEnumerable<IImpact> Map(ReviveIntent intent)
        {
            yield return new CasterCastedSpellEffect(intent.Caster);
            yield return new HealImpact(intent.Target, intent.Config.InstantHealthAddition);

            if (intent.Config.PeriodicHeal is not null and var status)
            {
                yield return new ReceivedStatusImpact(intent.Target, status);
            }
        }
    }
}