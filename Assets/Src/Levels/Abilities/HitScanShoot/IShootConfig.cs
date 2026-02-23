using System;

namespace Levels.Abilities.HitScanShoot
{
    public interface IShootConfig
    {
        int Damage { get; }
        float Offset { get; }
        float Distance { get; }
        float PushVelocity { get; }
        TimeSpan PushDuration { get; }
    }
}