using System;
using System.Collections.Generic;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Levels.Util
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentObservableMovement : MonoBehaviour, IObservableMovement
    {
        public event Action MovementUpdated;

        public Vector2 AbsoluteMovement { get; private set; }
        public Vector2 RelativeMovement { get; private set; }

        [Tooltip("Time span in seconds to average out the agent's velocity.")]
        [SerializeField] private float smoothingTime;

        private NavMeshAgent _agent;

        private readonly Queue<Vector3> _velocityHistory = new();
        private Vector3 _summedVelocity = Vector3.zero;
        private int _maxSamplesCount;

        private Vector2 _lastAbsoluteMovement;
        private Vector2 _lastRelativeMovement;

        private Transform _camera;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _maxSamplesCount = Mathf.RoundToInt(smoothingTime / Time.fixedDeltaTime);
        }

        private void FixedUpdate()
        {
            var currentVelocity = _agent.velocity;

            _velocityHistory.Enqueue(currentVelocity);
            _summedVelocity += currentVelocity;

            while (_velocityHistory.Count > _maxSamplesCount)
            {
                _summedVelocity -= _velocityHistory.Dequeue();
            }
        }

        private void Update()
        {
            if (_velocityHistory.Count == 0)
            {
                return;
            }

            var averageVelocity = _summedVelocity / _velocityHistory.Count;
            
            var absoluteMovement = Vector2.zero;
            var relativeMovement = Vector2.zero;

            if (!averageVelocity.IsNearlyZero())
            {
                absoluteMovement = averageVelocity.InFloorCoordinates();
                relativeMovement = DeriveRelativeMovement(averageVelocity);
            }

            if (absoluteMovement != _lastAbsoluteMovement || relativeMovement != _lastRelativeMovement)
            {
                _lastAbsoluteMovement = absoluteMovement;
                _lastRelativeMovement = relativeMovement;

                AbsoluteMovement = absoluteMovement;
                RelativeMovement = relativeMovement;

                MovementUpdated?.Invoke();
            }

            return;

            Vector2 DeriveRelativeMovement(Vector3 vector3)
            {
                var camForward = _camera.forward.With(y: 0).normalized;
                var camRight = _camera.right.With(y: 0).normalized;

                var localX = Vector3.Dot(vector3, camRight);
                var localZ = Vector3.Dot(vector3, camForward);

                relativeMovement = new Vector3(localX, 0, localZ).InFloorCoordinates();
                return relativeMovement;
            }
        }
    }
}