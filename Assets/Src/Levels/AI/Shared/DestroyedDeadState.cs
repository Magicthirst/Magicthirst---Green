using System.Collections;
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
        [Inject] private PublishIntent<KilledIntent> _publish;

        private DamageImpact _lastAttack = null;

        protected override void DidEnabled()
        {
            _consumer.Impacted += HandleDamage;
        }

        public override void Enter()
        {
            base.Enter();
            StartCoroutine(DestroySafely());
        }

        private IEnumerator DestroySafely()
        {
            _publish(new KilledIntent(_lastAttack.Attacker, Victim: _self, _lastAttack.Context));
            yield return null;
            _self.SetActive(false);
            Destroy(_self);
        }

        protected override void DidDisabled()
        {
            _consumer.Impacted -= HandleDamage;
        }

        private void HandleDamage(DamageImpact damage) => _lastAttack = damage;
    }
}