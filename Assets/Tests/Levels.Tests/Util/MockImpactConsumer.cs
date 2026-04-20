using System;
using Levels.IntentsImpacts;

namespace Levels.Tests.Util
{
    public class MockImpactConsumer<TImpact> : IImpactConsumer<TImpact> where TImpact : IImpact
    {
        public event Action<TImpact> Impacted;

        event Action IImpactConsumer.Impacted
        {
            add => NonParamImpacted += value;
            remove => NonParamImpacted -= value;
        }

        private event Action NonParamImpacted;

        public MockImpactConsumer()
        {
            Impacted += _ => NonParamImpacted?.Invoke();
        }

        public void Receive(TImpact impact) => Impacted?.Invoke(impact);

        public void Dispose() {}
    }
}