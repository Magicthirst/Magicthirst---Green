using System;

namespace Levels.IntentsImpacts
{
    public interface IImpactConsumer : IDisposable
    {
        event Action Impacted;
    }

    public interface IImpactConsumer<out TImpact> : IImpactConsumer
    {
        new event Action<TImpact> Impacted;
    }
}