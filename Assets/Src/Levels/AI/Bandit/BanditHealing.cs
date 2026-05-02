using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Abilities.Revival;
using Levels.AI.Util;
using Levels.Core;
using Levels.Core.Room;
using Levels.IntentsImpacts;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Levels.AI.Bandit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BanditHealing : FsmState
    {
        [SerializeField] private float healDistance;
        [SerializeField] private float tacticUpdatePeriod;
        [SerializeField] private float healDelay;

        private NavMeshAgent _agent;
        private Coroutine _healingCoroutine;
        private bool _IsActiveHealer => _healingCoroutine is not null;

        [Inject] private ReviveConfig _config;
        [Inject] private Entity _entity;
        [Inject] private PublishIntent<ReviveIntent> _revive;
        [Inject] private PublishIntent<ImpactIntent> _publishImpact;
        [Inject] private RoomHealing _roomHealing;

        protected override bool _IsReady => true;

        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            _roomHealing.RegisterHealer(_entity);
            _roomHealing.HelpRequested += OnHelpRequested;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log($"{gameObject.name}_BanditHealing:Enter");
            _healingCoroutine = StartCoroutine(HealRoutine());
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log($"{gameObject.name}_BanditHealing:Exit");
            if (_healingCoroutine != null)
            {
                StopCoroutine(_healingCoroutine);
                _healingCoroutine = null;
            }

            _roomHealing.ReleaseHealerClaims(_entity);

            _agent.ResetPath();
        }

        private void OnDisable()
        {
            _roomHealing.UnregisterHealer(_entity);
            _roomHealing.HelpRequested -= OnHelpRequested;
        }

        private IEnumerator HealRoutine()
        {
            _agent.isStopped = false;

            Transform downedTransform;
            foreach (var downed in _roomHealing.AttendDowned(_entity))
            {
                downedTransform = downed.Owner.transform;

                while (_roomHealing.IsDowned(downed) && downed.IsInWorld)
                {
                    if (!CloseToTarget())
                    {
                        _agent.SetDestination(downedTransform.position);
                        yield return InterruptableWait.ForSeconds(tacticUpdatePeriod);
                    }
                    else
                    {
                        _agent.ResetPath();
                        
                        _publishImpact(ImpactIntent.SelfCast(new CasterStartedSpellCastingEffect(_entity.Owner)));
                        yield return InterruptableWait.ForSeconds(healDelay);

                        _revive(new ReviveIntent(gameObject, downed.Owner, _config));
                        _roomHealing.ResolveHeal(downed);
                    }
                }
            }

            Finish();

            yield break;

            bool CloseToTarget()
            {
                return Vector3.Distance(downedTransform.position, this.transform.position) <= healDistance;
            }
        }

        private void OnHelpRequested()
        {
            if (!_IsActiveHealer)
            {
                Ready();
            }
        }
    }
}