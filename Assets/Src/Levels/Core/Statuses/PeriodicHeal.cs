using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Directorship;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core.Statuses
{
    [Serializable]
    public class PeriodicHeal : IStatus
    {
        [SerializeField] private int healingAmount;
        [SerializeField] private float interval;
        [SerializeField] private float duration;

        [Inject] private PublishIntent<ImpactIntent> _publish;

        public PeriodicHeal(int healingAmount, float interval, float duration)
        {
            this.healingAmount = healingAmount;
            this.interval = interval;
            this.duration = duration;
        }

        public IEnumerator Run(Entity entity)
        {
            var time = duration;
            while (time > 0)
            {
                time -= interval;
                _publish(ImpactIntent.SelfCast(new HealImpact(entity.Owner, healingAmount)));

                var endOfInterval = LevelDirector.GameplayTime + interval;
                yield return new WaitUntil(() => LevelDirector.GameplayTime >= endOfInterval);
            }
        }
    }
}