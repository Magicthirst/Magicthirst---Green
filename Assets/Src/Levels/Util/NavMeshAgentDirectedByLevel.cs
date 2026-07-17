using Levels.Directorship;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.Util
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentDirectedByLevel : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)mask;

        [SerializeField] private EditorLevelActivityMask mask;

        private bool _savedIsStopped = false;
        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected override void DidEnabled()
        {
            _agent.isStopped = _savedIsStopped;
        }

        protected override void DidDisabled()
        {
            _savedIsStopped = _agent.isStopped;
            _agent.isStopped = true;
        }
    }
}