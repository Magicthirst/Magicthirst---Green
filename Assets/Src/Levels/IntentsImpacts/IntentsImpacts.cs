using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Levels.IntentsImpacts
{
    public delegate IEnumerable<IImpact> MapIntentToImpacts(IIntent intent);

    public delegate IEnumerable<IImpact> MapIntentToImpacts<in TIntent>(TIntent intent) where TIntent : IIntent;

    public interface IIntentToImpactsMapper<in TIntent> where TIntent : IIntent
    {
        IEnumerable<IImpact> Map(TIntent intent);
    }

    public class IntentsImpacts
    {
        private readonly Dictionary<(int TargetID, Type Type), List<IImpactReceiver>> _receivers = new();
        private readonly Dictionary<Type, Dictionary<int, List<DeferredBroker>>> _brokers = new();

        private readonly Dictionary<Type, List<MapIntentToImpacts>> _mappersByTypes = new();

        public IntentsImpacts RegisterTransformation<TIntent>(IIntentToImpactsMapper<TIntent> mapper) where TIntent : IIntent =>
            RegisterTransformation<TIntent>(map: mapper.Map);

        public IntentsImpacts RegisterTransformation<TIntent>(MapIntentToImpacts<TIntent> map) where TIntent : IIntent
        {
            var tIntent = typeof(TIntent);

            if (!_mappersByTypes.TryGetValue(tIntent, out var mappers))
            {
                _mappersByTypes[tIntent] = mappers = new List<MapIntentToImpacts>();
            }

            mappers.Add(intent => map((TIntent)intent));
            return this;
        }

        public IntentsImpacts RegisterBroker<TIntent>(DeferredBroker<TIntent> broker, int targetId) where TIntent : IIntent
        {
            var tIntent = typeof(TIntent);

            if (!_brokers.TryGetValue(tIntent, out var brokersByTargets))
            {
                _brokers[tIntent] = brokersByTargets = new();
            }

            if (!brokersByTargets.TryGetValue(targetId, out var brokers))
            {
                brokersByTargets[targetId] = brokers = new List<DeferredBroker>();
            }

            brokers.Add(broker);

            return this;
        }

        public PublishIntent<TIntent> GetIntentPublisher<TIntent>() where TIntent : IIntent => TryPublish;

        public object GetIntentPublisher(Type tIntent)
        {
            var genericMethod = typeof(IntentsImpacts)
                .GetMethod(nameof(GetIntentPublisher), new Type[] {})!
                .MakeGenericMethod(tIntent);

            return genericMethod.Invoke(this, new object[] { });
        }

        public IImpactConsumer<TImpact> GetImpactConsumerFor<TImpact>(GameObject target) where TImpact : IImpact
        {
            return ImpactConsumer<TImpact>.Registered(target, this);
        }

        public IImpactConsumer GetImpactConsumerFor(GameObject target, Type impactType)
        {
            var genericMethod = typeof(IntentsImpacts)
                .GetMethod(nameof(GetImpactConsumerFor), new [] { typeof(GameObject) })!
                .MakeGenericMethod(impactType);

            return (IImpactConsumer) genericMethod.Invoke(this, new object[] { target });
        }

        private bool TryPublish<TIntent>(TIntent intent) where TIntent : IIntent
        {
            var tIntent = typeof(TIntent);

            if (!_brokers.TryGetValue(tIntent, out var brokersByTypes))
            {
                return PlainTryPublish(intent);
            }

            if (!_mappersByTypes.TryGetValue(tIntent, out var mappers))
            {
                return false;
            }

            var impactsByTargets = mappers
                .SelectMany(map => map(intent))
                .GroupBy(impact => impact.Target);

            foreach (var impactsByTarget in impactsByTargets)
            {
                var targetId = impactsByTarget.Key.GetInstanceID();

                if (brokersByTypes.TryGetValue(targetId, out var brokers))
                {
                    foreach (var broker in brokers)
                    {
                        broker.Consume(intent, impactsByTarget.ToArray());
                    }

                    continue;
                }

                foreach (var impact in impactsByTarget)
                {
                    if (!_receivers.TryGetValue((targetId, impact.GetType()), out var receivers))
                    {
                        continue;
                    }

                    foreach (var receiver in receivers)
                    {
                        receiver.Receive(impact);
                    }
                }
            }

            return true;
        }

        private bool PlainTryPublish<TIntent>(TIntent intent) where TIntent : IIntent
        {
            if (!_mappersByTypes.TryGetValue(typeof(TIntent), out var mappers))
            {
                return false;
            }

            var receiversImpacts =
                from impact in mappers.SelectMany(map => map(intent))
                where _receivers.ContainsKey(Key(impact))
                from receiver in _receivers[Key(impact)]
                select (Receiver: receiver, Impact: impact);


            foreach (var (receiver, impact) in receiversImpacts)
            {
                receiver.Receive(impact);
            }

            return true;

            (int, Type) Key(IImpact impact) => (impact.Target.GetInstanceID(), impact.GetType());
        }

        private class ImpactConsumer<TImpact> : IImpactConsumer<TImpact>, IImpactReceiver where TImpact : IImpact
        {
            private readonly int _targetID;
            private readonly IntentsImpacts _manager;

            public event Action<TImpact> Impacted;
            event Action IImpactConsumer.Impacted
            {
                add => NonParamImpacted += value;
                remove => NonParamImpacted -= value;
            }
            private event Action NonParamImpacted;

            private ImpactConsumer(int targetID, IntentsImpacts manager)
            {
                _targetID = targetID;
                _manager = manager;
                Impacted += _ => NonParamImpacted?.Invoke();
            }

            public static ImpactConsumer<TImpact> Registered(GameObject target, IntentsImpacts manager)
            {
                var self = new ImpactConsumer<TImpact>(target.GetInstanceID(), manager);
                var key = (target.GetInstanceID(), typeof(TImpact));

                if (!manager._receivers.TryGetValue(key, out var list))
                {
                    list = manager._receivers[key] = new List<IImpactReceiver>();
                }

                list.Add(self);

                return self;
            }

            public void Receive(IImpact impact) => Impacted?.Invoke((TImpact)impact);

            public void Dispose() => _manager._receivers
                .GetValueOrDefault((_targetID, typeof(TImpact)))
                ?.Remove(this);
        }

        private interface IImpactReceiver
        {
            public void Receive(IImpact impact);
        }
    }

    public abstract class DeferredBroker
    {
        public event Action<IImpact[]> Passed;

        private readonly Dictionary<IIntent, IImpact[]> _storage = new();

        public void Consume(IIntent intent, IImpact[] impacts)
        {
            _storage[intent] = impacts;
            Consume(intent);
        }

        protected void Pass(IIntent intent)
        {
            Passed?.Invoke(_storage[intent]);
            _storage.Remove(intent);
        }

        protected void Decline(IIntent intent) => _storage.Remove(intent);

        protected abstract void Consume(IIntent intent);
    }

    public abstract class DeferredBroker<TIntent> : DeferredBroker where TIntent : IIntent
    {
        public abstract void Consume(TIntent intent);

        protected sealed override void Consume(IIntent intent) => Consume((TIntent)intent);
    }
}