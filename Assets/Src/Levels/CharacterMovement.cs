using System;
using System.Collections;
using Levels.Abilities.CommonImpacts;
using Levels.Extensions;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using VContainer;

namespace Levels
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(IMovementInputSource))]
    public class CharacterMovement : MonoBehaviour, IInterruptable<IMovementReason>
    {
        public event Action<Vector2> Moved;

        [SerializeField] private bool broadcasting;

        [SerializeField] private float gravityPull;
        [SerializeField] private float speed;

        private CharacterController _controller;
        private IMovementInputSource _inputSource;
        [Inject] private IImpactConsumer<TeleportImpact> _teleportsConsumer;

        private Vector2? _resetPosition = null;
        private Vector2 _previousVelocity = Vector2.zero;

        private InterruptionQueue _interruptionQueue;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputSource = GetComponent<IMovementInputSource>();
            _interruptionQueue = new InterruptionQueue(this, new WaitForFixedUpdate());
        }

        private void OnEnable()
        {
            _inputSource.PositionUpdated += OnPositionUpdated;
            _teleportsConsumer.Impacted += OnTeleport;
        }

        public void Interrupt(IEnumerator routine) => _interruptionQueue.Interrupt(routine);

        private void FixedUpdate()
        {
            if (_resetPosition.HasValue)
            {
                transform.position = _resetPosition.Value.ToX0Y();
                _resetPosition = null;
                _controller.enabled = true;
                return;
            }

            if (_interruptionQueue.Running)
            {
                return;
            }

            var movement3d = new Vector3(_inputSource.Movement.x, 0f, _inputSource.Movement.y);
            _controller.Move(movement3d * (speed * Time.fixedDeltaTime));

            var moving = (_inputSource.Movement - _previousVelocity).IsMoving();
            if (broadcasting && moving)
            {
                _previousVelocity = _inputSource.Movement;
                Moved?.Invoke(_inputSource.Movement);
            }

            _controller.Move(Vector3.down * (gravityPull * Time.fixedDeltaTime));
        }

        private void OnTeleport(TeleportImpact impact)
        {
            
            Interrupt(Interruption());
            return;

            IEnumerator Interruption()
            {
                yield return new WaitForFixedUpdate();
                transform.position = impact.Position;
            }
        }

        private void OnPositionUpdated(Vector2 position)
        {
            _controller.enabled = false;
            _resetPosition = position;
        }

        private void OnDisable()
        {
            _inputSource.PositionUpdated -= OnPositionUpdated;
            _teleportsConsumer.Impacted -= OnTeleport;
        }

        private void OnDestroy()
        {
            _interruptionQueue.Dispose();
        }
    }
}
