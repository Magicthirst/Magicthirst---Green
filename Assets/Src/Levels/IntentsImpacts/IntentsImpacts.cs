using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly Dictionary<Type, List<Action<IImpact>>> _receiversByTypes = new();

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

        public IImpactConsumer<TImpact> GetImpactConsumer<TImpact>()
        {
            return ImpactConsumer<TImpact>.RegisteredIn(this);
        }

        private bool TryPublish<TIntent>(TIntent intent) where TIntent : IIntent
        {
            if (!_mappersByTypes.TryGetValue(typeof(TIntent), out var mappers))
            {
                return false;
            }

            var receiversImpacts =
                from impact in mappers.SelectMany(map => map(intent))
                where _receiversByTypes.ContainsKey(impact.GetType())
                from receiver in _receiversByTypes[impact.GetType()]
                select (Receiver: receiver, Impact: impact);

            foreach (var (receiver, impact) in receiversImpacts)
            {
                receiver.Invoke(impact);
            }

            return true;
        }

        private class ImpactConsumer<TImpact> : IImpactConsumer<TImpact>
        {
            private readonly IntentsImpacts _manager;

            public event Action<TImpact> Impacted;

            private ImpactConsumer(IntentsImpacts manager)
            {
                _manager = manager;
            }

            public static ImpactConsumer<TImpact> RegisteredIn(IntentsImpacts manager)
            {
                var self = new ImpactConsumer<TImpact>(manager);
                var tImpact = typeof(TImpact);

                if (!manager._receiversByTypes.TryGetValue(tImpact, out var list))
                {
                    list = manager._receiversByTypes[tImpact] = new List<Action<IImpact>>();
                }

                list.Add(self.Receive);

                return self;
            }

            private void Receive(object impact) => Impacted?.Invoke((TImpact) impact);

            public void Dispose() => _manager._receiversByTypes
                .GetValueOrDefault(typeof(TImpact))
                ?.Remove(Receive);
        }
    }
}