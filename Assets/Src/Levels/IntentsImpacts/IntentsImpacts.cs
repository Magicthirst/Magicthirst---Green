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

        public IntentsImpacts RegisterTransformation<TIntent>(IIntentToImpactsMapper<TIntent> mapper)
            where TIntent : IIntent =>
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

        public IntentsImpacts RegisterBroker<TIntent>(DeferredBroker<TIntent> broker, int targetId)
            where TIntent : IIntent
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
                .GetMethod(nameof(GetIntentPublisher), new Type[] { })!
                .MakeGenericMethod(tIntent);

            return genericMethod.Invoke(this, new object[] { });
        }

        public IImpactConsumer<TImpact> GetImpactConsumerFor<TImpact>(GameObject target) where TImpact : IImpact
        {
            return ImpactConsumer<TImpact>.Registered(target, this);
        }

        public IImpactConsumer<TImpact> GetImpactConsumerFor<TImpact>(GameObject target, IModifyingImpacts affectable) where TImpact : IImpact
        {
            var consumer = ImpactConsumer<TImpact>.Registered(target, this);
            return new AffectableImpactConsumer<TImpact>(consumer, affectable);
        }

        public IImpactConsumer GetImpactConsumerFor(GameObject target, Type impactType)
        {
            var genericMethod = typeof(IntentsImpacts)
                .GetMethod(nameof(GetImpactConsumerFor), new[] { typeof(GameObject) })!
                .MakeGenericMethod(impactType);

            return (IImpactConsumer)genericMethod.Invoke(this, new object[] { target });
        }

        public IImpactConsumer GetImpactConsumerFor(GameObject target, Type impactType, IModifyingImpacts affectable)
        {
            var genericMethod = typeof(IntentsImpacts)
                .GetMethod(nameof(GetImpactConsumerFor), new[] { typeof(GameObject), typeof(IModifyingImpacts) })!
                .MakeGenericMethod(impactType);

            return (IImpactConsumer)genericMethod.Invoke(this, new object[] { target, affectable });
        }

        private bool TryPublish<TIntent>(TIntent intent) where TIntent : IIntent
        {
            var tIntent = typeof(TIntent);

            if (!_mappersByTypes.TryGetValue(tIntent, out var mappers))
            {
                return false;
            }

            var impacts = mappers.SelectMany(map => map(intent));

            if (!_brokers.TryGetValue(tIntent, out var brokersByTypes))
            {
                PublishImpacts(impacts);
                return true;
            }

            var impactsByTargets = impacts.GroupBy(impact => impact.Target);

            foreach (var impactsByTarget in impactsByTargets)
            {
                if (!TrySendToBrokers(impactsByTarget))
                {
                    PublishImpacts(impactsByTarget);
                }
            }

            return true;

            bool TrySendToBrokers(IGrouping<GameObject, IImpact> group)
            {
                var targetId = group.Key.GetInstanceID();

                if (!brokersByTypes.TryGetValue(targetId, out var brokers))
                {
                    return false;
                }

                var usages = brokers.Count(broker => broker.TryConsume(intent, group.ToArray()));
                return usages > 0;
            }
        }

        private void PublishImpacts(IEnumerable<IImpact> impacts)
        {
            foreach (var impact in impacts)
            {
                var key = (impact.Target.GetInstanceID(), impact.GetType());
                if (!_receivers.TryGetValue(key, out var receivers))
                {
                    continue;
                }

                foreach (var receiver in receivers)
                {
                    receiver.Receive(impact);
                }
            }
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

        private class AffectableImpactConsumer<TImpact> : IImpactConsumer<TImpact>, IImpactReceiver where TImpact : IImpact
        {
            event Action IImpactConsumer.Impacted
            {
                add => _ImpactConsumer.Impacted += value;
                remove => _ImpactConsumer.Impacted -= value;
            }

            event Action<TImpact> IImpactConsumer<TImpact>.Impacted
            {
                add => _baseConsumer.Impacted += value;
                remove => _baseConsumer.Impacted -= value;
            }
            
            private readonly ImpactConsumer<TImpact> _baseConsumer;
            private readonly IModifyingImpacts _affector;

            private IImpactConsumer _ImpactConsumer => _baseConsumer;

            public AffectableImpactConsumer(ImpactConsumer<TImpact> baseConsumer, IModifyingImpacts affector)
            {
                _baseConsumer = baseConsumer;
                _affector = affector;
            }

            public void Receive(IImpact impact)
            {
                if ((impact = _affector.ApplyModifiers(impact)) != null)
                {
                    _baseConsumer.Receive(impact);
                }
            }

            public void Dispose() => _baseConsumer.Dispose();
        }

        private interface IImpactReceiver
        {
            public void Receive(IImpact impact);
        }
    }
}