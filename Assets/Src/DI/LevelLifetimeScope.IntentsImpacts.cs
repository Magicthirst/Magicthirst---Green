using Levels.Abilities.ChaosArea;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.Kill;
using Levels.Abilities.ParrySabre;
using Levels.Abilities.PushingShotgun;
using Levels.Abilities.TeleportChip;
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
                        .RegisterTransformation(new InfuseAreaWithChaosMapper(masks))
                        .RegisterTransformation(new KillMapper());
                },
                Lifetime.Singleton
            ).AsSelf();

            builder.Register
            (
                interfaceType: typeof(PublishIntent<>),
                implementationFactory: (resolver, tIntent) =>
                {
                    var intentsImpacts = resolver.Resolve<IntentsImpacts>();
                    return intentsImpacts.GetIntentPublisher(tIntent);
                },
                lifetime: Lifetime.Scoped
            );
        }
    }
}