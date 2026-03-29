using Levels.Abilities.ChaosArea;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.ParrySabre;
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
                    var masks = resolver.Resolve<MasksRegistry>();
                    var teleportChipMapper = new TeleportChipMapper();

                    return new IntentsImpacts()
                        .RegisterTransformation(new ImpactIntentMapper())
                        .RegisterTransformation(new PushingShotgunShotMapper(masks))
                        .RegisterTransformation(new HitScanShotMapper(masks))
                        .RegisterTransformation<TeleportChipThrowIntent>(teleportChipMapper)
                        .RegisterTransformation<TeleportChipActivateIntent>(teleportChipMapper)
                        .RegisterTransformation(new ParrySabreSwingMapper(masks))
                        .RegisterTransformation(new InfuseAreaWithChaosMapper(masks));
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