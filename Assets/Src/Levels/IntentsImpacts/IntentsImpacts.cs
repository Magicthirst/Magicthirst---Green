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

        public PublishIntent<TIntent> GetIntentPublisher<TIntent>() where TIntent : IIntent => TryPublish;

        public IImpactConsumer<TImpact> GetImpactConsumerFor<TImpact>(GameObject target) where TImpact : IImpact
        {
            return ImpactConsumer<TImpact>.Registered(target, this);
        }

        private bool TryPublish<TIntent>(TIntent intent) where TIntent : IIntent
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

            private ImpactConsumer(int targetID, IntentsImpacts manager)
            {
                _targetID = targetID;
                _manager = manager;
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
}