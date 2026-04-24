using System.Collections;
using Levels.Abilities.PushingShotgun;
using Levels.AI.Util;
using Levels.Extensions;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;

namespace Levels.AI.Shared
{
    public class ShotgunShooter
    {
        private readonly Transform _self;
        private readonly IShotgunConfig _config;
        private readonly PublishIntent<PushingShotgunShootIntent> _publishShoot;

        private readonly float _minDistance;
        private readonly float _shotSpreadDegrees;
        private readonly float _betweenShootPeriod;

        public ShotgunShooter
        (
            Transform self,
            IShotgunConfig config,
            PublishIntent<PushingShotgunShootIntent> publishShoot,
            float minDistance,
            float shotSpreadDegrees,
            float betweenShootPeriod
        )
        {
            _self = self;
            _config = config;
            _publishShoot = publishShoot;
            _shotSpreadDegrees = shotSpreadDegrees;
            _betweenShootPeriod = betweenShootPeriod;
            _minDistance = minDistance;
        }

        public IEnumerator Shoot(Transform enemy)
        {
            yield return InterruptableWait.ForSeconds(Random.Range(0f, _betweenShootPeriod));

            while (true)
            {
                if (Vector3.Distance(_self.position, enemy.position) > _minDistance)
                {
                    yield return null;
                    continue;
                }

                var direction = (enemy.position - _self.position).normalized;
                var spreadDirection = MathExt.SpreadDirection(direction, _shotSpreadDegrees);
                var intent = new PushingShotgunShootIntent(_self.gameObject, spreadDirection.With(y: 0f).normalized, _config);

                _publishShoot(intent);

                yield return InterruptableWait.ForSeconds(_betweenShootPeriod);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}