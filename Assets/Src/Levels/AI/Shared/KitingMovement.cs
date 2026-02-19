using System.Collections;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.AI.Shared
{
    public class KitingMovement
    {
        private const float SampleDistance = 4f;
        private readonly float[] _escapeAngles = { 0f, -30f, 30f, -60f, 60f, -90f, 90f };

        private readonly float _speed;
        private readonly float _maxDistance;
        private readonly WaitForSeconds _tacticUpdatePeriod;
        private readonly WaitForSeconds _updateShift;

        private readonly Transform _self;
        private readonly NavMeshAgent _agent;
        private readonly LayerMask _obstacleMask;

        public KitingMovement(
            float speed,
            float maxDistance,
            float tacticUpdatePeriod,
            Transform self,
            NavMeshAgent agent,
            LayerMask obstacleMask)
        {
            _speed = speed;
            _maxDistance = maxDistance;
            _tacticUpdatePeriod = new WaitForSeconds(tacticUpdatePeriod);
            _updateShift = new WaitForSeconds(Random.Range(0f, tacticUpdatePeriod));
            _self = self;
            _agent = agent;
            _obstacleMask = obstacleMask;
        }

        public IEnumerator Kite(Transform enemy)
        {
            _agent.speed = _speed;
            _agent.updateRotation = true; 

            yield return _updateShift;

            while (true)
            {
                if (!HasLineOfSight(enemy))
                {
                    _agent.SetDestination(enemy.position);
                    _agent.isStopped = false;
                }
                else if (Vector3.Distance(_self.position, enemy.position) >= _maxDistance)
                {
                    _agent.isStopped = true;
                }
                else if (TryFindNextDestination(enemy, out var fleePosition))
                {
                    _agent.SetDestination(fleePosition);
                    _agent.isStopped = false;
                }
                else
                {
                    _agent.isStopped = true;
                }

                yield return _tacticUpdatePeriod;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private bool HasLineOfSight(Transform enemy) => HasLineOfSight(_self.position, enemy);

        private bool HasLineOfSight(Vector3 origin, Transform enemy)
        {
            var target = enemy.position;
            var direction = target - origin;
            var distance = direction.magnitude;

            return !Physics.Raycast(origin, direction, distance, _obstacleMask);
        }

        private bool TryFindNextDestination(Transform enemy, out Vector3 fleePosition)
        {
            var awayFromPlayer = (_self.position - enemy.position).normalized.With(y: 0);

            var bestDistance = float.MinValue;
            fleePosition = _self.position;

            foreach (var angle in _escapeAngles)
            {
                var testDirection = Quaternion.Euler(0, angle, 0) * awayFromPlayer;
                var testPosition = _self.position + testDirection * SampleDistance;

                var distance = Vector3.Distance(enemy.position, testPosition);
                if (NavMesh.SamplePosition(testPosition, out var hit, 2f, NavMesh.AllAreas) &&
                    HasLineOfSight(hit.position, enemy) &&
                    distance > bestDistance)
                {
                    bestDistance = distance;
                    fleePosition = hit.position;
                }
            }

            return fleePosition != _self.position;
        }
    }
}