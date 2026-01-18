using Levels.Abilities.Dash;
using Levels.Abilities.Impacts;
using Levels.Config;
using Levels.IntentsImpacts;
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
                _ => new IntentsImpacts().RegisterTransformation(new DashMapper(config)),
                Lifetime.Singleton
            ).AsSelf();

            RegisterPublisher<DashIntent>();

            RegisterConsumerFactory<ImpulseImpact>();

            return;

            void RegisterConsumerFactory<T>() where T : IImpact
            {
                builder.RegisterFactory<GameObject, IImpactConsumer<T>>
                (
                    resolver =>
                    {
                        return target => resolver.Resolve<IntentsImpacts>().GetImpactConsumerFor<T>(target);
                    },
                    Lifetime.Transient
                ).AsSelf();
            }

            void RegisterPublisher<T>() where T : IIntent => builder.Register
            (
                resolver => resolver.Resolve<IntentsImpacts>().GetIntentPublisher<T>(),
                Lifetime.Transient
            ).AsSelf();
        }
    }
}