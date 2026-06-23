using System;
using Levels.Core;
using Levels.Directorship;
using Levels.IntentsImpacts;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using static Levels.Directorship.LevelActivityMask;

namespace Levels.Abilities.TeleportChip
{
    [RequireComponent(typeof(IMovementInputSource))]
    public class TeleportChipController : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => Gameplay | TutorialTeleportToChip;

        private IMovementInputSource _movementInput;

        [Inject] private PlayerInput _playerInput;
        [Inject] private Core.TeleportChip _state;
        [Inject] private PublishIntent<TeleportChipThrowIntent> _publishThrow;
        [Inject] private PublishIntent<TeleportChipActivateIntent> _publishActivate;
        [Inject] private TeleportChipConfig _config;
        private Transform _camera;

        private IDisposable _observer;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _movementInput = GetComponent<IMovementInputSource>();
        }

        protected override void DidEnabled()
        {
            var map = _playerInput.currentActionMap;
            _observer = map.ConsumeAction("UseTeleportChip").OnPerformed(() =>
            {
                switch (_state.State)
                {
                    case TeleportChipState.Ready:
                        var intent = new TeleportChipThrowIntent(gameObject, _state.Instance.gameObject, _camera.forward, _movementInput.AbsoluteMovement, _config);
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

        protected override void DidDisabled() => _observer?.Dispose();
    }
}