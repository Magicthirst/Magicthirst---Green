using System;

namespace Levels.IntentsImpacts
{
    public interface IImpactConsumer<out TImpact> : IDisposable
    {
        event Action<TImpact> Impacted;
    }
}