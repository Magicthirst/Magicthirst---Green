using System;
using Levels.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Levels
{
    public class ApplyInput : MonoBehaviour
    {
        private const float MovementChangeThreshold = 0.05f;

        public event Action<Vector2> Moved;

        [SerializeField] private float gravityPull;
        [SerializeField] private float speed;

        [SerializeField] private new Transform camera;

        private CharacterController _controller;
        private PlayerInput _input;

        private Vector2 _relativeMovement = Vector2.zero;
        private Vector3 _previousVelocity = Vector2.zero;

        private IDisposable _movementObserver;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>()!;
            _input = GetComponent<PlayerInput>()!;
        }

        private void OnEnable()
        {
            var map = _input.currentActionMap;
            _movementObserver = map.ConsumeAction<Vector2>("Move").OnPerformed(v => _relativeMovement = v);
            map.Enable();
        }

        private void FixedUpdate()
        {
            var radians = math.radians(-camera.eulerAngles.y);
            var cos = math.cos(radians);
            var sin = math.sin(radians);
            Vector3 vector = new
            (
                x: _relativeMovement.x * cos - _relativeMovement.y * sin,
                y: 0f,
                z: _relativeMovement.x * sin + _relativeMovement.y * cos
            );

            _controller.Move(vector.normalized * (speed * Time.fixedDeltaTime));
            _controller.Move(Vector3.down * gravityPull);

            if ((vector - _previousVelocity).magnitude > MovementChangeThreshold)
            {
                _previousVelocity = vector;
                Moved?.Invoke(vector.InFloorCoordinates());
            }
        }

        private void OnDisable() => _movementObserver.Dispose();
    }
}
