using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core.Statuses
{
    [Serializable]
    public class PeriodicDamage : IStatus
    {
        [SerializeField] private int damage;
        [SerializeField] private float interval;
        [SerializeField] private float duration;

        [Inject] private PublishIntent<ImpactIntent> _publish;

        private WaitForSeconds _waitForSeconds;

        public PeriodicDamage()
        {
            _waitForSeconds = new WaitForSeconds(interval);
        }

        public PeriodicDamage(int damage, float interval, float duration)
        {
            this.damage = damage;
            this.interval = interval;
            this.duration = duration;
            _waitForSeconds = new WaitForSeconds(interval);
        }

        public IEnumerator Run(Entity entity)
        {
            var time = duration;
            while (time > 0)
            {
                time -= interval;
                _publish(ImpactIntent.SelfCast(new DamageImpact(entity.Owner, damage)));
                yield return _waitForSeconds;
            }
        }
    }
}