using System.Collections;
using Levels.Abilities.HitScanShoot;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;

namespace Levels.AI.Shared
{
    public class BurstShooter
    {
        private static readonly Vector3 InvalidPosition = Vector3.negativeInfinity;

        private readonly float _shotSpreadDegrees;
        private readonly int _burstCount;
        private readonly int _shotCount;
        private readonly float _initialDelay;
        private readonly float _betweenBurstPeriod;
        private readonly float _betweenShotPeriod;

        private readonly Transform _transform;
        private readonly IShootConfig _config;
        private readonly PublishIntent<HitScanShootIntent> _publishShoot;

        public BurstShooter(
            float shotSpreadDegrees,
            int burstCount,
            int shotCount,
            float initialDelay,
            float betweenBurstPeriod,
            float betweenShotPeriod,
            Transform transform,
            IShootConfig config,
            PublishIntent<HitScanShootIntent> publishShoot)
        {
            _shotSpreadDegrees = shotSpreadDegrees;
            _burstCount = burstCount;
            _shotCount = shotCount;
            _initialDelay = initialDelay;
            _betweenBurstPeriod = betweenBurstPeriod;
            _betweenShotPeriod = betweenShotPeriod;
            _transform = transform;
            _config = config;
            _publishShoot = publishShoot;
        } 

        public IEnumerator Shoot(Collider enemy, bool retryWhenTargetLost = false, IEnumerator continuation = null)
        {
            var betweenShootDelayWaiter = new WaitForSeconds(_betweenShotPeriod);
            var betweenBurstDelayWaiter = new WaitForSeconds(_betweenBurstPeriod - _betweenShotPeriod);
            var initialDelayWaiter = new WaitForSeconds(_initialDelay - _betweenBurstPeriod + _betweenShotPeriod);

            do
            {
                yield return initialDelayWaiter;

                Vector3 targetPosition;
                var iBurst = 0;

                while ((targetPosition = enemy?.transform.position ?? InvalidPosition) != InvalidPosition &&
                       iBurst++ < _burstCount)
                {
                    yield return betweenBurstDelayWaiter;

                    var direction = (targetPosition - _transform.position).normalized;

                    for (var iShot = 0; iShot < _shotCount; iShot++)
                    {
                        var spreadDirection = MathExt.SpreadDirection(direction, _shotSpreadDegrees);

                        var intent = HitScanShootIntent.FromCenter(_transform.gameObject, spreadDirection, _config);
                        _publishShoot(intent);

                        yield return betweenShootDelayWaiter;
                    }
                }
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            } while (retryWhenTargetLost);

            yield return continuation;
        }
    }
}