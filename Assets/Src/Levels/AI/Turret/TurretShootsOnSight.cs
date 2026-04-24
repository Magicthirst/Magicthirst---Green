using System.Collections;
using Levels.Abilities.HitScanShoot;
using Levels.AI.Shared;
using Levels.IntentsImpacts;
using Levels.Util;
using Levels.Util.MasksRegistry;
using UnityEngine;
using VContainer;

namespace Levels.AI.Turret
{
    public class TurretShootsOnSight : FsmState
    {
        [SerializeField] private PublishContacts contacts;

        [SerializeField] private float shotSpreadDegrees;
        [SerializeField] private int burstCount;
        [SerializeField] private int shotCount;
        [SerializeField] private float initialDelay;
        [SerializeField] private float betweenBurstDelay;
        [SerializeField] private float betweenShotDelay;

        [Inject] private IShootConfig _config = null!;
        [Inject] private MasksRegistry _registry = null!;
        [Inject] private PublishIntent<HitScanShootIntent> _publishShoot;
        private Transform _enemy = null!;

        private BurstShooter _shooter;

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
                self: transform,
                config: _config,
                publishShoot: _publishShoot);
        }


        private void OnEnable()
        {
            contacts.ContactEntered += OnPlayerCameClose;
            contacts.ContactExited += OnPlayerGotAway;
        }

        private void OnPlayerCameClose(Collider other)
        {
            if (_registry.Is(other.gameObject, Mask.PlayerCharacter) &&
                Physics.Linecast(transform.position, other.transform.position))
            {
                _enemy = other.transform;
                Ready();
            }
        }

        public override void Enter()
        {
            base.Enter();
            StartCoroutine(_shooter.Shoot(_enemy, retryWhenTargetLost: false, continuation: FinishThenMaybeRerun()));

            return;

            IEnumerator FinishThenMaybeRerun()
            {
                Finish();

                yield return new WaitForSeconds(betweenBurstDelay);

                if (_enemy is not null)
                {
                    Ready();
                }
            }
        }

        private void OnPlayerGotAway(Collider other)
        {
            if (other.transform == _enemy)
            {
                _enemy = null;
            }
        }

        private void OnDisable()
        {
            contacts.ContactEntered -= OnPlayerCameClose;
            contacts.ContactExited -= OnPlayerGotAway;
        }
    }
}