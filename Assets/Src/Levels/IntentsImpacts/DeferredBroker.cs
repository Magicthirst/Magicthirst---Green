using System;
using System.Collections.Generic;

namespace Levels.IntentsImpacts
{
    public abstract class DeferredBroker<TIntent> : DeferredBroker where TIntent : IIntent
    {
        public abstract bool TryConsume(TIntent intent);

        protected sealed override bool TryConsume(IIntent intent) => TryConsume((TIntent)intent);
    }

    public abstract class DeferredBroker
    {
        public event Action<IImpact[]> Passed;

        private readonly Dictionary<IIntent, IImpact[]> _storage = new();

        public bool TryConsume(IIntent intent, IImpact[] impacts)
        {
            if (TryConsume(intent))
            {
                _storage[intent] = impacts;
                return true;
            }

            return false;
        }

        protected void Pass(IIntent intent)
        {
            Passed?.Invoke(_storage[intent]);
            _storage.Remove(intent);
        }

        protected void Decline(IIntent intent) => _storage.Remove(intent);

        protected abstract bool TryConsume(IIntent intent);
    }
}