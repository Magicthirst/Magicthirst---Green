using Levels.Config;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.PushingShotgun
{
    public class PushingShotgunShooter : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<PushingShotgunShootIntent> _publishPush;
        [Inject] private AbilitiesConfig _config;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke() => _publishPush(new PushingShotgunShootIntent(gameObject, _camera.forward));
    }
}