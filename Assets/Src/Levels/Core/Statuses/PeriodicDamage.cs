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

        public PeriodicDamage(int damage, float interval, float duration)
        {
            this.damage = damage;
            this.interval = interval;
            this.duration = duration;
        }

        public IEnumerator Run(Entity entity)
        {
            var time = duration;
            while (time > 0)
            {
                time -= interval;
                _publish(ImpactIntent.SelfCast(new DamageImpact(entity.Owner, entity.Owner, damage)));

                var endOfInterval = LevelDirector.GameplayTime + interval;
                yield return new WaitUntil(() => LevelDirector.GameplayTime >= endOfInterval);
            }
        }
    }
}