using System;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.Visual.SpriteResolution
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentObservableMovement : MonoBehaviour, IObservableMovement
    {
        public event Action<Vector2> Moved;

        private NavMeshAgent _agent;
        private Vector2 _lastVelocity;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void FixedUpdate()
        {
            var velocity = _agent.velocity.InFloorCoordinates();

            if (velocity != _lastVelocity)
            {
                Moved?.Invoke(velocity);
                _lastVelocity = velocity;
            }
        }
    }
}