using Levels.Config;
using Levels.IntentsImpacts;
using Levels.IntentsImpacts.Impacts;
using Levels.IntentsImpacts.Intents;
using Levels.IntentsImpacts.Mappings;
using UnityEngine;
using VContainer;

namespace DI
{
    public partial class LevelLifetimeScope
    {
        [SerializeField] private AbilitiesConfig config;

        private void ConfigureIntentsImpacts(IContainerBuilder builder)
        {
            builder.RegisterInstance(config).AsSelf();

            builder.Register
            (
                _ => new IntentsImpacts().RegisterTransformation(new DashImpactsMapper(config)),
                Lifetime.Singleton
            ).AsSelf();

            RegisterPublisher<DashIntent>();

            RegisterConsumer<ImpulseImpact>();

            return;

            void RegisterConsumer<T>() => builder.Register
            (
                resolver => resolver.Resolve<IntentsImpacts>().GetImpactConsumer<T>(),
                Lifetime.Transient
            ).AsSelf();

            void RegisterPublisher<T>() where T : IIntent => builder.Register
            (
                resolver => resolver.Resolve<IntentsImpacts>().GetIntentPublisher<T>(),
                Lifetime.Transient
            ).AsSelf();
        }
    }
}