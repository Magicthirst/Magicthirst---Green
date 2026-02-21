using Levels.Abilities.Dash;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.PushingShotgun;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using VContainer;

namespace DI
{
    public partial class LevelLifetimeScope
    {
        private void ConfigureIntentsImpacts(IContainerBuilder builder)
        {
            builder.Register
            (
                resolver => new IntentsImpacts()
                    .RegisterTransformation(new DashMapper())
                    .RegisterTransformation(new PushingShotgunShotMapper(resolver.Resolve<MasksRegistry>()))
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