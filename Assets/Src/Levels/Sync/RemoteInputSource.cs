using System;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    [RequireComponent(typeof(PlayerStateUpdatesReceiver))]
    public class RemoteInputSource : SyncBehavior, IInputSource
    {
        public Vector2 Movement { get; private set; }

        public event Action<Vector2> PositionUpdated;

        private PlayerStateUpdatesReceiver _stateUpdates;

        [Inject] private IConsumer _consumer;

        protected override void Awake()
        {
            base.Awake();

            _stateUpdates = GetComponent<PlayerStateUpdatesReceiver>();
        }

        private void OnEnable()
        {
            _stateUpdates.MovementUpdated += OnMovementCommanded;

            if (_consumer != null)
            {
                _consumer.MovementCommanded += OnMovementCommanded;
            }
        }

        private void OnMovementCommanded(Vector2 position, Vector2 vector, double elapsedSeconds)
        {
            MainThreadContext.Post(_ =>
            {
                Movement = vector;
                Debug.Log($"{_consumer}");
                var estimatedPosition = position + vector * (float)elapsedSeconds;
                PositionUpdated?.Invoke(estimatedPosition);
            }, null);
        }

        private void OnDisable()
        {
            if (_stateUpdates != null)
            {
                _stateUpdates.MovementUpdated -= OnMovementCommanded;
            }
            _consumer.MovementCommanded -= OnMovementCommanded;
        }
    }
}