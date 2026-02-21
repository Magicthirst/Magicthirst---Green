using System;
using Levels.Extensions;
using Levels.IntentsImpacts;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Levels.Abilities.Dash
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(IMovementInputSource))]
    public class Dasher : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private IMovementInputSource _movementInput;

        [Inject] private PublishIntent<DashIntent> _publishDash;
        [Inject] private IDashConfig _config;

        private IDisposable _observer;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _movementInput = GetComponent<IMovementInputSource>();
        }

        private void OnEnable()
        {
            var map = _playerInput.currentActionMap;
            _observer = map.ConsumeAction("Dash").OnPerformed(() =>
            {
                if (_movementInput.Movement.IsMoving())
                {
                    _publishDash(new DashIntent(gameObject, _movementInput.Movement, _config));
                }
            });
        }

        private void OnDisable() => _observer.Dispose();
    }
}