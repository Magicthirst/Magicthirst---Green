using System.Collections;
using System.Collections.Generic;
using Levels.AI.Util;
using Levels.Extensions;
using Levels.Util;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.AI.Bandit
{
    public class ScreenerSquadMemberMovement
    {
        private readonly int _id;
        private readonly NavMeshAgent _agent;
        private readonly Dictionary<int, Vector3> _membersPositions;
        private readonly float _tacticUpdatePeriod;

        public ScreenerSquadMemberMovement
        (
            int id,
            NavMeshAgent agent,
            Dictionary<int, Vector3> membersPositions,
            float tacticUpdatePeriod
        )
        {
            _id = id;
            _agent = agent;
            _membersPositions = membersPositions;
            _tacticUpdatePeriod = tacticUpdatePeriod;
        }

        public IEnumerator Screen(Enemy enemy)
        {
            _agent.isStopped = false;

            while (true)
            {
                yield return null;

                var slot = _membersPositions[_id];
                var enemyPosition = enemy.Position;
                var estimatedEnemyPosition = enemy.EstimatedPosition;

                var memberPosition = _agent.transform.position;
                var currentDistance = Vector3.Distance(memberPosition, enemyPosition);

                // Prefer the squad-brain slot, but keep it tied to the predicted enemy center.
                var toSlot = slot - estimatedEnemyPosition;
                if (toSlot.IsNearlyZero())
                {
                    yield return InterruptableWait.ForSeconds(_tacticUpdatePeriod);
                    continue;
                }

                // Never place the member farther from the current enemy than it already is.
                var desiredDistance = Mathf.Min(toSlot.magnitude, currentDistance);

                var destination = estimatedEnemyPosition + toSlot.normalized * desiredDistance;

                // Extra safety: ensure this destination is not farther from the current enemy.
                var destinationDistance = Vector3.Distance(destination, enemyPosition);
                if (destinationDistance > currentDistance)
                {
                    destination = enemyPosition + (destination - enemyPosition).normalized * currentDistance;
                }

                _agent.SetDestination(destination);

                yield return InterruptableWait.ForSeconds(_tacticUpdatePeriod);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}