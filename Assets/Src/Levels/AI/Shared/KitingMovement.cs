using System.Collections;
using Levels.AI.Util;
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
        private readonly float _tacticUpdatePeriod;
        private readonly float _updateShift;

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
            _tacticUpdatePeriod = tacticUpdatePeriod;
            _updateShift = Random.Range(0f, tacticUpdatePeriod);
            _self = self;
            _agent = agent;
            _obstacleMask = obstacleMask;
        }

        public IEnumerator Kite(Transform enemy)
        {
            _agent.speed = _speed;
            _agent.updateRotation = true; 

            yield return InterruptableWait.ForSeconds(_updateShift);

            while (true)
            {
                if (!HasLineOfSight(enemy.position))
                {
                    _agent.SetDestination(enemy.position);
                    _agent.isStopped = false;
                }
                // TODO if (!HasLineOfSight(enemy.position) && !TryStrifeIntoView(enemy.position))
                // TODO {
                // TODO     _agent.SetDestination(enemy.position);
                // TODO     _agent.isStopped = false;
                // TODO }
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

                yield return InterruptableWait.ForSeconds(_tacticUpdatePeriod);
            }
            // ReSharper disable once IteratorNeverReturns
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

                if (TryFindNextBestDestination(testPosition, bestDistance, enemy.position, out var next))
                {
                    bestDistance = next.Distance;
                    fleePosition = next.Position;
                }
            }

            return fleePosition != _self.position;

            bool TryFindNextBestDestination(Vector3 test, float bestDistance, Vector3 enemy, out (Vector3 Position, float Distance) next)
            {
                var positionExists = NavMesh.SamplePosition(test, out var hit, SampleDistance, NavMesh.AllAreas);
                next = (hit.position, Vector3.Distance(hit.position, enemy));
                if (!positionExists)
                {
                    return false;
                }

                var distanceIsBetter = next.Distance > bestDistance;
                if (!distanceIsBetter)
                {
                    return false;
                }

                if (!HasLineOfSight(next.Position, enemy))
                {
                    return false;
                }

                return true;
            }
        }

        private bool HasLineOfSight(Vector3 enemy) => HasLineOfSight(_self.position, enemy);

        private bool HasLineOfSight(Vector3 origin, Vector3 target)
        {
            var direction = target - origin;
            var distance = direction.magnitude;

            return !Physics.Raycast(origin, direction, distance, _obstacleMask);
        }
    }
}