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
            _movementObserver = map.ConsumeAction<Vector2>("Move").OnPerformed(v =>
            {
                var radians = math.radians(-_camera.eulerAngles.y);
                var cos = math.cos(radians);
                var sin = math.sin(radians);
                Movement = new Vector2
                (
                    x: v.x * cos - v.y * sin,
                    y: v.x * sin + v.y * cos
                ).normalized;
            });
            map.Enable();
        }

        private void OnDisable()
        {
            _movementObserver.Dispose();
        }
    }
}