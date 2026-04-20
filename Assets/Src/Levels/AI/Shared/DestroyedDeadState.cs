using Levels.Abilities.Kill;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.AI.Shared
{
    public class DestroyedDeadState : FsmState
    {
        protected override bool _IsReady => _dead;

        private bool _dead = false;

        [Inject] private GameObject _self;
        [Inject] private IImpactConsumer<DiedImpact> _consumer;

        private void OnEnable()
        {
            _consumer.Impacted += OnDead;
        }

        public override void Enter()
        {
            base.Enter();
            _self.SetActive(false);
            Destroy(_self);
        }

        private void OnDisable()
        {
            _consumer.Impacted -= OnDead;
        }

        private void OnDead(DiedImpact _)
        {
            _dead = true;
            Ready();
        }
    }
}