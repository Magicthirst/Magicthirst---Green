using System.Collections;
using System.Collections.Generic;
using Levels.AI.Util;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.AI.Bandit
{
    public class ScreenerSquadMemberMovement
    {
        private readonly int _id;
        private readonly float _speed;
        private readonly NavMeshAgent _agent;
        private readonly Dictionary<int, Vector3> _membersPositions;
        private readonly float _tacticUpdatePeriod;

        private readonly WaitForSeconds _tacticUpdateWaiter;

        public ScreenerSquadMemberMovement
        (
            int id,
            float speed,
            NavMeshAgent agent,
            Dictionary<int, Vector3> membersPositions,
            float tacticUpdatePeriod
        )
        {
            _id = id;
            _speed = speed;
            _agent = agent;
            _membersPositions = membersPositions;
            _tacticUpdatePeriod = tacticUpdatePeriod;
        }

        public IEnumerator Screen()
        {
            _agent.isStopped = false;
            _agent.speed = _speed;

            while (true)
            {
                yield return null;

                _agent.SetDestination(_membersPositions[_id]);
                yield return InterruptableWait.ForSeconds(_tacticUpdatePeriod);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}