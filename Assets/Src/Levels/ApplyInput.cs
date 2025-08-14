using System;
using UnityEngine;

namespace Levels
{
    [RequireComponent(typeof(CharacterController))]
    public class ApplyInput : MonoBehaviour
    {
        private const float MovementChangeThreshold = 0.05f;

        public event Action<Vector2> Moved;

        [SerializeField] private bool broadcasting;

        [SerializeField] private float gravityPull;
        [SerializeField] private float speed;

        private CharacterController _controller;
        private IInputSource _inputSource;

        private Vector2? _resetPosition = null;
        private Vector2 _previousVelocity = Vector2.zero;

        private int _leftPhysicalFramesToLog = 0;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputSource = GetComponent<IInputSource>();
        }

        private void OnEnable()
        {
            _inputSource.PositionUpdated += OnPositionUpdated; 
        }

        private void FixedUpdate()
        {
            if (_leftPhysicalFramesToLog == 0)
            {
                _leftPhysicalFramesToLog = 25;
                Debug.Log($"_resetPosition={_resetPosition}");
                Debug.Log($"_inputSource.Movement={_inputSource.Movement}");
            }

            if (_resetPosition.HasValue)
            {
                transform.position = _resetPosition.Value;
                _resetPosition = null;
            }

            var movement3d = new Vector3(_inputSource.Movement.x, 0f, _inputSource.Movement.y);
            _controller.Move(movement3d * (speed * Time.fixedDeltaTime));

            var dMagnitude = (_inputSource.Movement - _previousVelocity).magnitude;
            if (broadcasting && dMagnitude > MovementChangeThreshold)
            {
                _previousVelocity = _inputSource.Movement;
                Moved?.Invoke(_inputSource.Movement);
            }

            _controller.Move(Vector3.down * gravityPull);
        }

        private void OnPositionUpdated(Vector2 position)
        {
            _resetPosition = position;
        }

        private void OnDisable()
        {
            _inputSource.PositionUpdated -= OnPositionUpdated;
        }
    }
}
