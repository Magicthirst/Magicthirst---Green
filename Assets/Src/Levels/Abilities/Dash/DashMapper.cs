using System;
using System.Collections.Generic;
using Levels.Abilities.Impacts;
using Levels.Extensions;
using Levels.IntentsImpacts;

namespace Levels.Abilities.Dash
{
    public class DashMapper : IIntentToImpactsMapper<DashIntent>
    {
        public IEnumerable<IImpact> Map(DashIntent intent) => new []
        {
            new ImpulseImpact(intent.Caster, intent.Direction.ToX0Y() * intent.Config.Velocity, intent.Config.Duration)
        };
    }

    public interface IDashConfig
    {
        float Velocity { get; }
        TimeSpan Duration { get; }
    }
}