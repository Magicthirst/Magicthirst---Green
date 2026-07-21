using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.HitScanShoot;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using Util;

namespace Levels.AI.Shared
{
    public class BurstShooter
    {
        private static readonly Vector3 InvalidPosition = Vector3.negativeInfinity;

        private readonly LayerMask _wallsLayer;
        private readonly LayerMask _unitsLayer;

        private readonly float _shotSpreadDegrees;
        private readonly int _burstCount;
        private readonly int _shotCount;
        private readonly float _initialDelay;
        private readonly float _betweenBurstPeriod;
        private readonly float _betweenShotPeriod;

        private readonly Transform _self;
        private readonly ShootConfig _config;
        private readonly PublishIntent<HitScanShootIntent> _publishShoot;

        private readonly RaycastHit[] _hits = new RaycastHit[16];

        public BurstShooter
        (
            float shotSpreadDegrees,
            int burstCount,
            int shotCount,
            float initialDelay,
            float betweenBurstPeriod,
            float betweenShotPeriod,
            Transform self,
            ShootConfig config,
            LayerMask wallsLayer,
            LayerMask unitsLayer,
            PublishIntent<HitScanShootIntent> publishShoot
        )
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
            _wallsLayer = wallsLayer;
            _unitsLayer = unitsLayer;
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
                    var direction = (targetPosition - _self.position).normalized;

                    if (!IsSeesEnemy(enemy, direction))
                    {
                        yield return null;
                    }

                    yield return InterruptableWait.ForSeconds(betweenBurstDelay);

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

        private bool IsSeesEnemy(Transform enemy, Vector3 direction)
        {
            var hitsCount = Physics.RaycastNonAlloc
        (
                new Ray(_self.position, direction),
                _hits,
                _config.Distance,
                _wallsLayer | _unitsLayer
            );

            _hits.Sort(hitsCount);

            foreach (var hit in _hits.AsSpan(0, hitsCount))
            {
                var hitTransform = hit.transform;
                var hitLayerMask = 1 << hitTransform.gameObject.layer;

                var hitWall = (hitLayerMask & _wallsLayer) != 0;
                if (hitWall)
                {
                    return false;
                }

                if (hitTransform == enemy)
                {
                    return true;
                }
            }

            return false;
        } 
    }
}