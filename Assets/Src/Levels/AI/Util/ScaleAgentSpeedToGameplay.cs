using Levels.Directorship;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.AI.Util
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ScaleAgentSpeedToGameplay : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private float _baseAgentSpeed;
        private float _speedScale = LevelDirector.GameplayTimeSpeed;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _baseAgentSpeed = _agent.speed;
        }

        private void Update()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (LevelDirector.GameplayTimeSpeed == _speedScale)
            {
                return;
            }

            _speedScale = LevelDirector.GameplayTimeSpeed;
            _agent.speed = _baseAgentSpeed * _speedScale;
        }
    }
}