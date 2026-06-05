using Levels.Abilities.CommonImpacts;
using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "HealsOnKills", menuName = "Core/Components/HealsOnKills", order = 1)]
    public class HealsOnKills : CoreObject
    {
        [SerializeField] private int healAmount;

        [Inject] private PublishIntent<ImpactIntent> _publish;
        [Inject] private IImpactConsumer<TargetKilledVictimImpact> _consumer;

        public override void Init()
        {
            _consumer.Impacted += OnKilled;
        }

        private void OnKilled(TargetKilledVictimImpact killed)
        {
            Debug.Log($"{Owner} killed {killed}");
            if (killed.Context.HasFlag(ImpactContext.HealOnKill))
            {
                Debug.Log($"{Owner} heals");
                _publish(ImpactIntent.SelfCast(new HealImpact(Owner, healAmount)));
            }
        }

        public override void Dispose()
        {
            _consumer.Impacted -= OnKilled;
        }
    }
}