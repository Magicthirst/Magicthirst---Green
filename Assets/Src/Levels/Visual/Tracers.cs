using Levels.Abilities.CommonImpacts;
using Levels.Directorship;
using Levels.IntentsImpacts;
using VContainer;

namespace Levels.Visual
{
    public class Tracers : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [Inject] private IImpactConsumer<CastersBulletMissedEffect> _missedBullets;
        [Inject] private IImpactConsumer<CastersBulletHitEffect> _hitBullets;

        protected override void DidEnabled()
        {
            _missedBullets.Impacted += AddMissedTracer;
            _hitBullets.Impacted += AddHitTracer;
        }

        protected override void DidDisabled()
        {
            _missedBullets.Impacted -= AddMissedTracer;
            _hitBullets.Impacted -= AddHitTracer;
        }

        private void AddHitTracer(CastersBulletHitEffect effect)
        {
            TracersManager.SpawnLine(effect.Origin, effect.Destination);
        }

        private void AddMissedTracer(CastersBulletMissedEffect effect)
        {
            TracersManager.SpawnRay(effect.Origin, effect.Direction);
        }
    }
}