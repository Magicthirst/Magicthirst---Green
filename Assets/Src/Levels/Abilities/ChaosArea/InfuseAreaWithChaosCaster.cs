using Levels.Abilities.Shared;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.ChaosArea
{
    public class InfuseAreaWithChaosCaster : MonoBehaviour, IInHandAbility
    {
        private Transform _camera;

        [Inject] private PublishIntent<InfuseAreaWithChaosIntent> _cast;
        [Inject] private IChaosAreaConfig _config;
        [Inject] private ISharedSpellConfig _sharedConfig;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        public void Invoke()
        {
            var position = SpellCastAnchor.GetAnchorPosition
            (
                origin: transform.position,
                direction: _camera.forward,
                distance: _sharedConfig.MaxDistance
            );

            _cast(new InfuseAreaWithChaosIntent(gameObject, position, _config));
        }
    }
}