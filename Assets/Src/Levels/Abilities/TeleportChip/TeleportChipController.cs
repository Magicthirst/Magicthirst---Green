using System;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Levels.Abilities.TeleportChip
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(IMovementInputSource))]
    public class TeleportChipController : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private IMovementInputSource _movementInput;

        [Inject] private Core.TeleportChip _state;
        [Inject] private PublishIntent<TeleportChipThrowIntent> _publishThrow;
        [Inject] private PublishIntent<TeleportChipActivateIntent> _publishActivate;
        [Inject] private ITeleportChipConfig _config;
        private Transform _camera;

        private IDisposable _observer;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _movementInput = GetComponent<IMovementInputSource>();
        }

        private void OnEnable()
        {
            var map = _playerInput.currentActionMap;
            _observer = map.ConsumeAction("UseTeleportChip").OnPerformed(() =>
            {
                switch (_state.State)
                {
                    case TeleportChipState.Ready:
                        var intent = new TeleportChipThrowIntent(gameObject, _state.Instance.gameObject, _camera.forward, _movementInput.Movement, _config);
                        _publishThrow(intent);
                        _state.Throw();
                        break;
                    case TeleportChipState.Thrown:
                        break;
                    case TeleportChipState.OnGround:
                        var activateIntent = new TeleportChipActivateIntent(gameObject, _state.Instance);
                        _publishActivate(activateIntent);
                        _state.Restore();
                        break;
                }
            });
        }

        private void OnDisable() => _observer.Dispose();
    }
}