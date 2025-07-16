using System;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Levels.Sync
{
    public class SendInput : MonoBehaviour
    {
        [SerializeField] private float movementChangeThreshold;

        [Inject] private SendMovement _sendMovement = null!;

        private PlayerInput _input = null!;
        private IDisposable _movementObserver = null!;

        private Vector2 _lastSentMovement;

        public delegate void SendMovement(Vector2 movement);

        [Inject]
        public void AssertProperConnectionRole(IsPublishingInput isPublishingInput)
        {
            if (!isPublishingInput())
            {
                Destroy(this);
            }
        }

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            var map = _input.currentActionMap;
            _movementObserver = map.ConsumeAction<Vector2>("Move").OnPerformed(SendMovementIfChanged);
            map.Enable();
        }

        private void SendMovementIfChanged(Vector2 movement)
        {
            var change = (movement.Abs() - _lastSentMovement.Abs()).magnitude;
            if (change < movementChangeThreshold) return;

            _lastSentMovement = movement;
            _sendMovement(movement);
        }

        private void OnDisable() => _movementObserver.Dispose();
    }
}
