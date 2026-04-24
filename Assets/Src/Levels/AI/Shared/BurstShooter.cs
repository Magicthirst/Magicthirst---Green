using System.Collections;
using Levels.Abilities.HitScanShoot;
using Levels.AI.Util;
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

        private readonly Transform _self;
        private readonly IShootConfig _config;
        private readonly PublishIntent<HitScanShootIntent> _publishShoot;

        public BurstShooter(
            float shotSpreadDegrees,
            int burstCount,
            int shotCount,
            float initialDelay,
            float betweenBurstPeriod,
            float betweenShotPeriod,
            Transform self,
            IShootConfig config,
            PublishIntent<HitScanShootIntent> publishShoot)
        {
            _shotSpreadDegrees = shotSpreadDegrees;
            _burstCount = burstCount;
            _shotCount = shotCount;
            _initialDelay = initialDelay;
            _betweenBurstPeriod = betweenBurstPeriod;
            _betweenShotPeriod = betweenShotPeriod;
            _self = self;
            _config = config;
            _publishShoot = publishShoot;
        } 

        public IEnumerator Shoot(Transform enemy, bool retryWhenTargetLost = false, IEnumerator continuation = null)
        {
            var betweenShootDelay = _betweenShotPeriod;
            var betweenBurstDelay = _betweenBurstPeriod - _betweenShotPeriod;
            var initialDelay = _initialDelay - _betweenBurstPeriod + _betweenShotPeriod;

            do
            {
                yield return InterruptableWait.ForSeconds(initialDelay);

                Vector3 targetPosition;
                var iBurst = 0;

                while ((targetPosition = enemy?.position ?? InvalidPosition) != InvalidPosition && // TODO investigate
                       iBurst++ < _burstCount)
                {
                    yield return InterruptableWait.ForSeconds(betweenBurstDelay);

                    var direction = (targetPosition - _self.position).normalized;

                    for (var iShot = 0; iShot < _shotCount; iShot++)
                    {
                        var spreadDirection = MathExt.SpreadDirection(direction, _shotSpreadDegrees);

                        var intent = HitScanShootIntent.FromCenter(_self.gameObject, spreadDirection, _config);
                        _publishShoot(intent);

                        yield return InterruptableWait.ForSeconds(betweenShootDelay);
                    }
                }
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            } while (retryWhenTargetLost);

            yield return continuation;
        }
    }
}