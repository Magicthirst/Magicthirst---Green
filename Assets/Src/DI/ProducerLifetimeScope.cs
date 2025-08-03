using Common;
using Levels.Sync;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class ProducerLifetimeScope : LifetimeScope
    {
        [Inject] private IConnectionEstablishedEventHolder _connectionHolder;

        private IProducer _producer;

        private void Construct(ISyncConnection connection) => _producer = connection.Self;

        private void OnEnable()
        {
            _connectionHolder.ConnectionEstablished += Construct;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder
                .Register<SendInput.SendMovement>(_ => (position, vector) => _producer?.SendMovement(position, vector), Lifetime.Singleton)
                .AsSelf();
        }

        private void OnDisable()
        {
            _connectionHolder.ConnectionEstablished -= Construct;
        }
    }
}