using Levels.Config;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.Push
{
    [RequireComponent(typeof(IMovementInputSource))]
    public class Pusher : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<PushIntent> _publishPush;
        [Inject] private AbilitiesConfig _config;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke() => _publishPush(new PushIntent(gameObject, _camera.forward));
    }
}