using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Levels.Sync;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Levels
{
    public class ApplyInput : MonoBehaviour, IMovementInputSource
    {
        public event Action<Vector3> PositionChanged;
        public event Action<Vector3> VelocityChanged;

        [SerializeField] private float gravityPull;
        [SerializeField] private float speed;

        [SerializeField] private new Transform camera;

        private CharacterController _controller;
        private PlayerInput _input;

        private Vector2 _relativeMovement = Vector2.zero;
        private Vector3 _previousVelocity = Vector2.zero;

        private readonly List<IDisposable> _disposables = new();

        private void Awake()
        {
            _controller = GetComponent<CharacterController>()!;
            _input = GetComponent<PlayerInput>()!;
        }

        private void OnEnable()
        {
            var map = _input.currentActionMap;
            ToBeDisposed(map.ConsumeAction<Vector2>("Move")).Performed += v => _relativeMovement = v;
            map.Enable();
            return;

            T ToBeDisposed<T>(T disposable) where T : IDisposable
            {
                _disposables.Add(disposable);
                return disposable;
            }
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

            if ((vector - _previousVelocity).magnitude > 0.05f)
            {
                _previousVelocity = vector;
                VelocityChanged?.Invoke(vector); 
            }

            if (_controller.velocity.magnitude > 0.05f)
            {
                PositionChanged?.Invoke(transform.position.HorizontalVector());
            }
        }

        private void OnDisable()
        {
            var disposables = _disposables.ToArray();
            _disposables.Clear();
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }

    internal static class PlayerInputExtension
    {
        public static GenericAction<T> ConsumeAction<T>(this InputActionMap map, string actionName) where T : struct => new(map, actionName);

        public class GenericAction<T> : IDisposable where T : struct
        {
            public event Action<T> Performed;

            private readonly InputAction _action;

            public GenericAction(InputActionMap map, string actionName)
            {
                try
                {
                    _action = map.FindAction(actionName, true);
                    _action.Enable();
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException
                    (
                        "Existing action names: " + string.Join(", ", map.actions.Select(a => a.name).ToArray()),
                        e
                    );
                }
                _action.performed += OnActionPerformed;
                _action.canceled += OnActionPerformed;
            }

            public void Dispose()
            {
                _action.performed -= OnActionPerformed;
            }

            private void OnActionPerformed(InputAction.CallbackContext context) => Performed?.Invoke(context.ReadValue<T>());
        }
    }
}
