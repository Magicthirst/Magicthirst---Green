using System;
using Levels.IntentsImpacts;

namespace Levels.Util
{
    public struct ImpactObserver
    {
        public IImpactConsumer Consumer;
        public Action Subscription;

        public void Subscribe() => Consumer.Impacted += Subscription;

        public void Unsubscribe() => Consumer.Impacted -= Subscription;
    }
}