using System.Collections.Generic;
using Levels.Config;
using Levels.IntentsImpacts.Impacts;
using Levels.IntentsImpacts.Intents;

namespace Levels.IntentsImpacts.Mappings
{
    public class DashImpactsMapper : IIntentToImpactsMapper<DashIntent>
    {
        private readonly AbilitiesConfig _config;

        public DashImpactsMapper(AbilitiesConfig config)
        {
            _config = config;
        }

        public IEnumerable<IImpact> Map(DashIntent intent) => new []
        {
            new ImpulseImpact(intent.Caster, intent.Direction * _config.dashVelocity, _config.DashDuration)
        };
    }
}