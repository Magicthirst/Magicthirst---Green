using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

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

        private readonly List<(IIntent Intent, IImpact[] Impacts)> _storage = new(); // LinkedList could be fitting in theory

        public bool TryConsume(IIntent intent, IImpact[] impacts)
        {
            if (TryConsume(intent))
            {
                _storage.Add((intent, impacts));
                return true;
            }

            return false;
        }

        protected void Pass(IIntent intent)
        {
            if (!_storage.TryRemoveBy(out var pair, i => ReferenceEquals(i.Intent, intent)))
            {
                Debug.LogError($"No intent {intent} in storage");
                return;
            }

            Passed?.Invoke(pair.Impacts);
        }

        protected void Decline(IIntent intent)
        {
            if (!_storage.TryRemoveBy(out _, i => ReferenceEquals(i.Intent, intent)))
            {
                Debug.LogError($"No intent {intent} in storage");
            }
        }

        protected abstract bool TryConsume(IIntent intent);
    }
}