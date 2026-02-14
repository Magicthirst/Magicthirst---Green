using System.Collections;
using Levels.Abilities.HitScanShoot;
using Levels.IntentsImpacts;
using Levels.Util;
using Levels.Util.MasksRegistry;
using UnityEngine;
using VContainer;

namespace Levels.AI.Turret
{
    public class TurretShootsOnSight : FsmState
    {
        private static readonly Vector3 InvalidPosition = Vector3.negativeInfinity;

        protected override bool _IsReady => _enemy != null;

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
        private Collider _enemy = null!;

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
                _enemy = other;
                Ready();
            }
        }

        public override void Enter()
        {
            base.Enter();
            StartCoroutine(Shoot());
        }

        private IEnumerator Shoot()
        {
            var betweenShootDelayWaiter = new WaitForSeconds(betweenShotDelay);
            var betweenBurstDelayWaiter = new WaitForSeconds(betweenBurstDelay - betweenShotDelay);

            yield return new WaitForSeconds(initialDelay - betweenBurstDelay + betweenShotDelay);

            // ReSharper disable once MoveVariableDeclarationInsideLoopCondition : this will do worse
            Vector3 targetPosition;
            var iBurst = 0;

            while ((targetPosition = _enemy?.transform.position ?? InvalidPosition) != InvalidPosition &&
                   iBurst++ < burstCount)
            {
                yield return betweenBurstDelayWaiter;

                var direction = (targetPosition - transform.position).normalized;

                for (var iShot = 0; iShot < shotCount; iShot++)
                {
                    var spreadDirection = MathExt.SpreadDirection(direction, shotSpreadDegrees);

                    var intent = HitScanShootIntent.FromCenter(transform.gameObject, spreadDirection, _config);
                    _publishShoot(intent);

                    yield return betweenShootDelayWaiter;
                }
            }

            Finish();

            yield return betweenBurstDelayWaiter;

            if (_enemy is not null)
            {
                Ready();
            }
        }

        private void OnPlayerGotAway(Collider other)
        {
            if (other == _enemy)
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