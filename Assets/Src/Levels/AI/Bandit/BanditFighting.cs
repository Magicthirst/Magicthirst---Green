using Levels.Abilities.HitScanShoot;
using Levels.AI.Shared;
using Levels.IntentsImpacts;
using Levels.Util;
using Levels.Util.MasksRegistry;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Levels.AI.Bandit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BanditFighting : FsmState
    {
        [SerializeField] private PublishContacts startFightArea;
        [SerializeField] private PublishContacts stopFightArea;
        [SerializeField] private LayerMask wallLayer;

        [Header("Shooting Settings")]
        [SerializeField] private float shotSpreadDegrees;
        [SerializeField] private int burstCount;
        [SerializeField] private int shotCount;
        [SerializeField] private float initialDelay;
        [SerializeField] private float betweenBurstDelay;
        [SerializeField] private float betweenShotDelay;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float maxDistance;
        [SerializeField] private float tacticUpdatePeriod;

        [Inject] private IShootConfig _config = null!;
        [Inject] private MasksRegistry _registry = null!;
        [Inject] private PublishIntent<HitScanShootIntent> _publishShoot;
        private Collider _enemy = null!;

        private BurstShooter _shooter;
        private KitingMovement _movement;

        private Coroutine[] _coroutines;

        protected override bool _IsReady => _enemy != null;

        protected override void Awake()
        {
            base.Awake();

            _shooter = new BurstShooter(
                shotSpreadDegrees: shotSpreadDegrees,
                burstCount: burstCount,
                shotCount: shotCount,
                initialDelay: initialDelay,
                betweenBurstPeriod: betweenBurstDelay,
                betweenShotPeriod: betweenShotDelay,
                transform: transform,
                config: _config,
                publishShoot: _publishShoot);

            _movement = new KitingMovement(
                speed: movementSpeed,
                maxDistance: maxDistance,
                tacticUpdatePeriod: tacticUpdatePeriod,
                self: transform,
                agent: GetComponent<NavMeshAgent>(),
                obstacleMask: wallLayer);
        }

        private void OnEnable()
        {
            startFightArea.ContactEntered += OnPlayerCameClose;
            stopFightArea.ContactExited += OnPlayerGotAway;
        }

        private void OnPlayerCameClose(Collider other)
        {
            if (_registry.Is(other.gameObject, Mask.PlayerCharacter))
            {
                _enemy = other;
                Ready();
            }
        }

        public override void Enter()
        {
            base.Enter();
            _coroutines = new[]
            {
                StartCoroutine(_shooter.Shoot(_enemy, retryWhenTargetLost: true)),
                StartCoroutine(_movement.Kite(_enemy.transform))
            };
        }

        public override void Exit()
        {
            base.Exit();
            foreach (var coroutine in _coroutines)
            {
                StopCoroutine(coroutine);
            }
            _coroutines = null!;
        }

        private void OnPlayerGotAway(Collider other)
        {
            if (other == _enemy)
            {
                _enemy = null;
                Finish();
            }
        }

        private void OnDisable()
        {
            startFightArea.ContactEntered -= OnPlayerCameClose;
            stopFightArea.ContactExited -= OnPlayerGotAway;
        }
    }
}