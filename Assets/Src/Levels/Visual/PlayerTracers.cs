using Levels.Abilities.CommonImpacts;
using Levels.Directorship;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    public class PlayerTracers : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SerializeField] private Color parriedTracerTint;

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
            if (effect.Context.HasFlag(ImpactContext.ResultOfBadParry))
            {
                TracersManager.SpawnLine(effect.Origin, effect.Destination, parriedTracerTint);
            }
            else
            {
                TracersManager.SpawnLine(effect.Origin, effect.Destination);
            }
        }

        private void AddMissedTracer(CastersBulletMissedEffect effect)
        {
            if (effect.Context.HasFlag(ImpactContext.ResultOfBadParry))
            {
                TracersManager.SpawnRay(effect.Origin, effect.Direction, parriedTracerTint);
            }
            else
            {
                TracersManager.SpawnRay(effect.Origin, effect.Direction);
            }
        }
    }
}