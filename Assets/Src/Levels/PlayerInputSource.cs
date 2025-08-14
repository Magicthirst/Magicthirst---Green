using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Levels
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputSource : MonoBehaviour, IInputSource
    {
        public Vector2 Movement { get; private set; } = Vector3.zero;

        public event Action<Vector2> PositionUpdated;

        private Transform _camera;
        private PlayerInput _input;

        private Vector2 _relativeMovement;

        private IDisposable _movementObserver;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            var map = _input.currentActionMap;
            _movementObserver = map.ConsumeAction<Vector2>("Move")
                .OnPerformed(v => _relativeMovement = v);
            map.Enable();
        }

        private void FixedUpdate()
        {
            if (!_camera.hasChanged) return;

            var radians = math.radians(-_camera.eulerAngles.y);
            var cos = math.cos(radians);
            var sin = math.sin(radians);
            Movement = new Vector2
            (
                x: _relativeMovement.x * cos - _relativeMovement.y * sin,
                y: _relativeMovement.x * sin + _relativeMovement.y * cos
            ).normalized;

            _camera.hasChanged = false;
        }

        private void OnDisable()
        {
            _movementObserver.Dispose();
        }
    }
}