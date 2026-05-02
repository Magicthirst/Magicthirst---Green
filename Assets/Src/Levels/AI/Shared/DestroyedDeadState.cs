using Levels.Abilities.CommonImpacts;
using Levels.Abilities.KillAndDown;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.AI.Shared
{
    public class DestroyedDeadState : FsmState
    {
        protected override bool _IsReady => _health.IsDown;

        [Inject] private Health _health;
        [Inject] private GameObject _self;
        [Inject] private IImpactConsumer<DamageImpact> _consumer;
        [Inject] private PublishIntent<ImpactIntent> _publish;

        private DamageImpact _lastAttack = null;

        private void OnEnable()
        {
            _consumer.Impacted += HandleDamage;
        }

        public override void OnFrame()
        {
            base.OnFrame();

            _publish(new ImpactIntent(_self, new KilledImpact(_lastAttack.Attacker, Victim: _self, _lastAttack.Context)));

            _self.SetActive(false);
            Destroy(_self);
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleDamage;
        }

        private void HandleDamage(DamageImpact damage) => _lastAttack = damage;
    }
}