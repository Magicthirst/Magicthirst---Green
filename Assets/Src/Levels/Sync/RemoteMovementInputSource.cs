using System;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    [RequireComponent(typeof(PlayerStateUpdatesReceiver))]
    public class RemoteMovementInputSource : SyncBehavior, IMovementInputSource
    {
        public Vector2 AbsoluteMovement { get; private set; }
        public Vector2 RelativeMovement { get; private set; } // TODO

        public event Action MovementUpdated { add {} remove {} } 
        public event Action<Vector2> ForcePositionUpdated;

        private PlayerStateUpdatesReceiver _stateUpdates;

        [Inject] private IConsumer _consumer;

        protected override void OnAwake()
        {
            _stateUpdates = GetComponent<PlayerStateUpdatesReceiver>();
        }

        protected override void OnEnableSync()
        {
            _stateUpdates.MovementUpdated += OnMovementCommanded;
            _consumer.MovementCommanded += OnMovementCommanded;
        }

        private void OnMovementCommanded(Vector2 position, Vector2 vector, double elapsedSeconds)
        {
            MainThreadContext.Post(_ =>
            {
                AbsoluteMovement = vector;
                Debug.Log($"{_consumer}");
                var estimatedPosition = position + vector * (float)elapsedSeconds;
                ForcePositionUpdated?.Invoke(estimatedPosition);
            }, null);
        }

        protected override void OnDisableSync()
        {
            _stateUpdates.MovementUpdated -= OnMovementCommanded;
            _consumer.MovementCommanded -= OnMovementCommanded;
        }
    }
}