using System.Collections.Generic;
using Levels.Abilities.Impacts;
using Levels.Config;
using Levels.Extensions;
using Levels.IntentsImpacts;

namespace Levels.Abilities.Dash
{
    public class DashMapper : IIntentToImpactsMapper<DashIntent>
    {
        private readonly AbilitiesConfig _config;

        public DashMapper(AbilitiesConfig config)
        {
            _config = config;
        }

        public IEnumerable<IImpact> Map(DashIntent intent) => new []
        {
            new ImpulseImpact(intent.Caster, intent.Direction.ToX0Y() * _config.dashVelocity, _config.DashDuration)
        };
    }
}