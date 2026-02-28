using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.ParrySabre
{
    public class ParrySabreSwinger : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;
        [Inject] private ISabreConfig _config;
        [Inject] private PublishIntent<ParrySabreSwingIntent> _swing;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform; 

        public void Invoke()
        {
            _swing(new ParrySabreSwingIntent(gameObject, _camera.forward, _config));
        }
    }
}