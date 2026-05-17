using System;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.Util
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentObservableMovement : MonoBehaviour, IObservableMovement
    {
        public event Action<Vector2> MovementUpdated;

        public Vector2 Movement => _lastVelocity;

        private NavMeshAgent _agent;
        private Vector2 _lastVelocity;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            var velocity = _agent.velocity.InFloorCoordinates();

            if (velocity != _lastVelocity)
            {
                MovementUpdated?.Invoke(velocity);
                _lastVelocity = velocity;
            }
        }
    }
}