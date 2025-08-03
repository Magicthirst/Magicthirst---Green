using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Common;
using Debug = UnityEngine.Debug;
using RiptideClient = Riptide.Client;

namespace Web.Sync
{
    public partial class SyncConnection : ISyncConnection
    {
        private readonly RiptideClient _client;
        private readonly Stopwatch _syncWatch;
        private readonly CancellationTokenSource _cancellation;

        private readonly Dictionary<int, Consumer> _consumers = new();
        private readonly Producer _self;

        public IProducer Self => _self;

        private SyncConnection(RiptideClient client)
        {
            _client = client;
            _syncWatch = new Stopwatch();
            _cancellation = new CancellationTokenSource();

            _self = new Producer();
            _self.Exited += Dispose;

            Route();
        }

        public IConsumer GetForIndividual(int playerId)
        {
            if (_consumers.TryGetValue(playerId, out var consumer))
            {
                return consumer;
            }

            consumer = new Consumer();
            _consumers[playerId] = consumer;
            return consumer;
        }

        public void Dispose()
        {
            _cancellation.Cancel();
            _client.Disconnect();
            Debug.Log($"Dispose {nameof(SyncConnection)}");
        }
    }
}