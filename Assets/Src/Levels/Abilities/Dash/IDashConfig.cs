using System;

namespace Levels.Abilities.Dash
{
    public interface IDashConfig
    {
        float Velocity { get; }
        TimeSpan Duration { get; }
    }
}