using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.HitScanShoot
{
    public class HitScanShooter : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<HitScanShootIntent> _publishShoot;
        [Inject] private IShootConfig _config;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke()
        {
            Vector3 direction;

            if (Physics.Raycast(_camera.transform.position, _camera.forward, out var hit))
            {
                direction = (hit.point - transform.position).normalized;
            }
            else
            {
                direction = _camera.forward;
            }

            var origin = transform.position + direction * _config.Offset;

            _publishShoot(new HitScanShootIntent(gameObject, origin, direction, _config));
        }
    }
}