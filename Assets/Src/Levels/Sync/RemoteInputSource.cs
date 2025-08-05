using System;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    [RequireComponent(typeof(PlayerStateUpdatesReceiver))]
    public class RemoteInputSource : MonoBehaviour, IInputSource
    {
        public Vector2 Movement { get; private set; }

        public event Action<Vector2> PositionUpdated;

        private PlayerStateUpdatesReceiver _stateUpdates;

        [Inject] private IConsumer _consumer;

        private void Awake()
        {
            _stateUpdates = GetComponent<PlayerStateUpdatesReceiver>();
        }

        private void OnEnable()
        {
            _stateUpdates.MovementUpdated += OnMovementCommanded;
            _consumer.MovementCommanded += OnMovementCommanded;
        }

        private void OnMovementCommanded(Vector2 position, Vector2 vector, double elapsedSeconds)
        {
            Movement = vector;
            var estimatedPosition = position + vector * (float)elapsedSeconds;
            PositionUpdated?.Invoke(estimatedPosition);
        }

        private void OnDisable()
        {
            _stateUpdates.MovementUpdated -= OnMovementCommanded;
            _consumer.MovementCommanded -= OnMovementCommanded;
        }
    }
}