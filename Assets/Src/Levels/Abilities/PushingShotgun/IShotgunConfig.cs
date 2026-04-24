using System;

namespace Levels.Abilities.PushingShotgun
{
    public interface IShotgunConfig
    {
        int Damage { get; }
        float Velocity { get; }
        TimeSpan Duration { get; }
        float CircleRadius { get; }
        float CircleCenterOffset { get; }
    }
}