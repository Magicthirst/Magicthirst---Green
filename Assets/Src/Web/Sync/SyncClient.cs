using System;
using Riptide.Utils;
using static Riptide.MessageSendMode;

namespace Web.Sync
{
    public class SyncClient : IDisposable
    {
        public delegate bool TryCollectPlayerUpdate(out PlayerUpdate update);

        private readonly TryCollectPlayerUpdate _tryCollectPlayerUpdate;
        private readonly SyncConfig _config;

        private readonly Riptide.Client _client = new();
        private float _clientUpdateTimer = 0f;
        private float _publishUpdateTimer = 0f;

        public SyncClient(TryCollectPlayerUpdate tryCollectPlayerUpdate, SyncConfig config, RiptideLogger.LogMethod log)
        {
            _tryCollectPlayerUpdate = tryCollectPlayerUpdate;
            _config = config;
            RiptideLogger.Initialize(log, true);
        }

        public void Start() => _client.Connect(_config.hostAddress);

        public void Update(float dt)
        {
            _clientUpdateTimer += dt;
            _publishUpdateTimer += dt;

            if (_clientUpdateTimer >= _config.clientUpdateDt)
            {
                _clientUpdateTimer -= _config.clientUpdateDt;
                ClientUpdate();
            }
            if (_publishUpdateTimer >= _config.publishUpdateDt)
            {
                _publishUpdateTimer -= _config.publishUpdateDt;
                PublishingUpdate();
            }
        }

        private void ClientUpdate() => _client.Update();

        private void PublishingUpdate()
        {
            if (_tryCollectPlayerUpdate(out var playerUpdate))
            {
                _client.Send(Riptide.Message
                    .Create(Unreliable, MessageMark.PlayerUpdate)
                    .AddPlayerUpdate(playerUpdate)
                );
            }
        }

        public void Dispose() => _client.Disconnect();
    }
}
