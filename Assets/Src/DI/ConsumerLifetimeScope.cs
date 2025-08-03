using VContainer;
using VContainer.Unity;

namespace DI
{
    public class ConsumerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            /*builder
                .Register<ConsumeMovement.Subscribe>(_ => observer => _consumer.MovementCommanded += observer, Lifetime.Singleton)
                .AsSelf();*/
        }
    }
}