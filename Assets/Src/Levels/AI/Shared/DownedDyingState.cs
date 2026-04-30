using Levels.Abilities.KillAndDown;
using Levels.Abilities.Revival;
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

        [Inject] private IImpactConsumer<DownedImpact> _downedConsumer;
        [Inject] private IImpactConsumer<RecoveredImpact> _recoveredConsumer;

        private void OnEnable()
        {
            _downedConsumer.Impacted += OnDowned;
            _recoveredConsumer.Impacted += OnRecovered;
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
            _downedConsumer.Impacted -= OnDowned;
            _recoveredConsumer.Impacted -= OnRecovered;
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

        private void OnRecovered(RecoveredImpact _)
        {
            if (_downed)
            {
                Finish();
            }
        }
    }
}