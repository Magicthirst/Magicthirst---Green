using Levels.Abilities.HitScanShoot;
using Levels.Abilities.PushingShotgun;
using Levels.Abilities.TeleportChip;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using VContainer;
using VContainer.Internal;

namespace DI
{
    public partial class LevelLifetimeScope
    {
        private void ConfigureIntentsImpacts(IContainerBuilder builder)
        {
            builder.Register
            (
                resolver =>
                {
                    var teleportChipMapper = new TeleportChipMapper();

                    return new IntentsImpacts()
                        .RegisterTransformation(new PushingShotgunShotMapper(resolver.Resolve<MasksRegistry>()))
                        .RegisterTransformation(new HitScanShotMapper(resolver.Resolve<MasksRegistry>()))
                        .RegisterTransformation<TeleportChipThrowIntent>(teleportChipMapper)
                        .RegisterTransformation<TeleportChipActivateIntent>(teleportChipMapper);
                },
                Lifetime.Singleton
            ).AsSelf();

            RegisterPublishers();

            return;

            void RegisterPublishers()
            {
                foreach (var tIntent in CachedTypes.Intents)
                {
                    var publisherType = typeof(PublishIntent<>).MakeGenericType(tIntent);
                    var registration = new FuncRegistrationBuilder(
                        resolver => resolver
                            .Resolve<IntentsImpacts>()
                            .GetIntentPublisher(tIntent),
                        publisherType,
                        Lifetime.Transient
                    );
                    builder.Register(registration);
                }
            }
        }
    }
}