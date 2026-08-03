using Levels.Abilities.PlayerShared;
using Levels.Core;
using Levels.IntentsImpacts;
using Levels.Util.MasksRegistry;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.HitScanShoot
{
    public class HitScanShooter : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<HitScanShootIntent> _publishShoot;
        [Inject] private ShootConfig _config;
        [Inject] private MasksRegistry _registry;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke()
        {
            var direction = PlayerAim.GetDirection(_camera, transform, _registry);
            var origin = transform.position + direction * _config.Offset;

            _publishShoot(new HitScanShootIntent(gameObject, origin, direction, _config));
        }
    }
}