using Levels.Config;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.Shoot
{
    public class Shooter : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<ShootIntent> _publishShoot;
        [Inject] private AbilitiesConfig _config;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke() => _publishShoot(new ShootIntent(gameObject, _camera.transform.position, _camera.forward, _config.shootDamage));
    }
}