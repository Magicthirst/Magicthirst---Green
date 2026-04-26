using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.AI.Shared
{
    public class DownedDyingState : FsmState
    {
        [SerializeField] private float bleedingOutDelay;
        [SerializeField] private float invulnerabilityDelay;

        protected override bool _IsReady => _downed;

        private float _bleedingOutTimePoint = 0f;
        private float _invulnerabilityEndTimePoint = 0f;
        private bool _downed = false;

        [Inject] private IImpactConsumer<DownedImpact> _consumer;

        private void OnEnable()
        {
            _consumer.Impacted += OnDowned;
        }

        public override void Enter()
        {
            base.Enter();
            _downed = true;
            _bleedingOutTimePoint = Time.time + bleedingOutDelay;
            _invulnerabilityEndTimePoint = Time.time + invulnerabilityDelay;
        }

        public override void OnFrame()
        {
            if (Time.time >= _bleedingOutTimePoint)
            {
                Finish();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _downed = false;
            _bleedingOutTimePoint = 0f;
            _invulnerabilityEndTimePoint = 0f;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= OnDowned;
        }

        private void OnDowned(DownedImpact _)
        {
            if (!_downed)
            {
                Ready();
            }
            else if (Time.time > _invulnerabilityEndTimePoint)
            {
                Finish();
            }
        }
    }
}