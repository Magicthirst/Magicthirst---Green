using System;

namespace Levels.IntentsImpacts
{
    public interface IImpactConsumer : IDisposable // TODO remove IDisposable
    {
        event Action Impacted;
    }

    public interface IImpactConsumer<out TImpact> : IImpactConsumer
    {
        new event Action<TImpact> Impacted;
    }
}