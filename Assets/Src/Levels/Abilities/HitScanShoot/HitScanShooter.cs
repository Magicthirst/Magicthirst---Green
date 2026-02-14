using Levels.Config;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.HitScanShoot
{
    public class HitScanShooter : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<HitScanShootIntent> _publishShoot;
        [Inject] private AbilitiesConfig _config;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke() => _publishShoot(new HitScanShootIntent(gameObject, _camera.transform.position, _camera.forward, _config));
    }
}