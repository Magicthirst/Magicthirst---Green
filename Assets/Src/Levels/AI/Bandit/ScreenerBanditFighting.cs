using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Abilities.PushingShotgun;
using Levels.AI.Shared;
using Levels.AI.Util;
using Levels.Extensions;
using Levels.IntentsImpacts;
using Levels.Util;
using Levels.Util.MasksRegistry;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Levels.AI.Bandit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ScreenerBanditFighting : FsmState, IInterruptable<IMovementReason>
    {
        [SerializeField] private PublishContacts startFightArea;
        [SerializeField] private PublishContacts stopFightArea;
        [SerializeField] private ScreenerSquadBrain brain;

        [Header("Shooting Settings")]
        [SerializeField] private float minDistance;
        [SerializeField] private float shotSpreadDegrees;
        [SerializeField] private float betweenShotDelay;

        [Header("Util")]
        [SerializeField] private float lookAheadTime;

        [Inject] private ShotgunConfig _config = null!;
        [Inject] private MasksRegistry _registry = null!;
        [Inject] private PublishIntent<PushingShotgunShootIntent> _publishShoot;
        [Inject] private IImpactConsumer<DamageImpact> _wasDamaged;
        private Enemy _enemy;

        private ShotgunShooter _shooter;
        private ScreenerSquadMemberMovement _movement;

        private InterruptionQueue _squadShootingCooldown;
        private InterruptionQueue _movementInterruptionQueue;
        private Coroutine[] _coroutines;

        private NavMeshAgent _agent;

        protected override bool _IsReady => _enemy.IsSet;

        protected override void Awake()
        {
            base.Awake();

            _enemy = new Enemy { LookAheadTime = lookAheadTime };

            _agent = GetComponent<NavMeshAgent>();

            _movementInterruptionQueue = new InterruptionQueue(this, new WaitForFixedUpdate());

            _shooter = new ShotgunShooter
            (
                self: transform,
                config: _config,
                publishShoot: _publishShoot.WithSideeffect(_ => brain.NotifyShot()),
                minDistance: minDistance,
                shotSpreadDegrees: shotSpreadDegrees,
                betweenShootPeriod: betweenShotDelay
            );

            (_movement, _squadShootingCooldown) = brain.RegisterMember(_agent);
        }

        protected override void DidEnabled()
        {
            startFightArea.ContactEntered += OnPlayerCameClose;
            stopFightArea.ContactExited += OnPlayerGotAway;
            _wasDamaged.Impacted += OnWasDamaged;
        }

        private void OnWasDamaged(DamageImpact impact)
        {
            if (_enemy.IsSet)
            {
                return;
            }

            _enemy.SetTo(impact.Attacker.transform);
            Ready();
        }

        private void OnPlayerCameClose(Collider other)
        {
            if (_registry.Is(other.gameObject, Mask.PlayerCharacter))
            {
                _enemy.SetTo(other.transform);
                Ready();
            }
        }

        public override void Enter()
        {
            base.Enter();
            _coroutines = new[]
            {
                StartCoroutine(_shooter.Shoot(_enemy)
                    .WithInterruptions(_squadShootingCooldown)
                    .WithInterruptions(_LevelLifecycle)),

                StartCoroutine(_movement.Screen(_enemy)
                    .WithInterruptions(_movementInterruptionQueue)
                    .WithInterruptions(_LevelLifecycle))
            };
        }

        public override void Exit()
        {
            base.Exit();
            _agent.isStopped = true;
            foreach (var coroutine in _coroutines)
            {
                StopCoroutine(coroutine);
            }
            _coroutines = null;
        }

        private void OnPlayerGotAway(Collider other)
        {
            if (_enemy.Is(other))
            {
                _enemy.Unset();
                Finish();
            }
        }

        public void Interrupt(IEnumerator block) => _movementInterruptionQueue.Interrupt(block);

        protected override void DidDisabled()
        {
            startFightArea.ContactEntered -= OnPlayerCameClose;
            stopFightArea.ContactExited -= OnPlayerGotAway;
            _wasDamaged.Impacted -= OnWasDamaged;
        }
    }
}