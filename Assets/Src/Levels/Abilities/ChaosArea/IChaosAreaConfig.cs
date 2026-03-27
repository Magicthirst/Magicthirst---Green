using Levels.Core.Statuses;

namespace Levels.Abilities.ChaosArea
{
    public interface IChaosAreaConfig
    {
        float CircleRadius { get; }

        IStatus Status { get; }
    }
}