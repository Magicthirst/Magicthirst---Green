using Levels.Abilities.CommonModifiers;

namespace Levels.Abilities.ChaosArea
{
    public interface IChaosAreaConfig
    {
        float CircleRadius { get; }
        
        // Periodic Damage Settings
        int DamagePerTick { get; }
        float DamageInterval { get; }
        
        // Scale Damage Settings
        ScaleReceivedDamage.IScale DamageScale { get; }
        float Duration { get; }
    }
}