using Levels.Abilities.Dash;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.PushingShotgun;
using Levels.Config;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
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
                resolver => new IntentsImpacts()
                    .RegisterTransformation(new DashMapper(config))
                    .RegisterTransformation(new PushingShotgunShotMapper(config, resolver.Resolve<MasksRegistry>()))
                    .RegisterTransformation(new HitScanShotMapper(resolver.Resolve<MasksRegistry>())),
                Lifetime.Singleton
            ).AsSelf();

            RegisterPublisher<DashIntent>();
            RegisterPublisher<PushingShotgunShootIntent>();
            RegisterPublisher<HitScanShootIntent>();

            return;

            void RegisterPublisher<T>() where T : IIntent => builder.Register
            (
                resolver => resolver.Resolve<IntentsImpacts>().GetIntentPublisher<T>(),
                Lifetime.Transient
            ).AsSelf();
        }
    }
}