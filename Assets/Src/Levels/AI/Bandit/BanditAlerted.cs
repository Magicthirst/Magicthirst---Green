using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.AI.Bandit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BanditAlerted : FsmState
    {
        [SerializeField] private Transform alertSeat;
        [SerializeField] private float beforeReturnDelay;

        private NavMeshAgent _agent;
        private WaitForSeconds _beforeReturnWaiter;
        private Coroutine _runningCoroutine;

        protected override bool _IsReady => true;

        protected override void Awake()
        {
            base.Awake();

            _agent = GetComponent<NavMeshAgent>();
            _beforeReturnWaiter = new WaitForSeconds(beforeReturnDelay);
        }

        public override void Enter()
        {
            base.Enter();
            _runningCoroutine = StartCoroutine(RunToAlertPosition());
        }

        private IEnumerator RunToAlertPosition()
        {
            _agent.isStopped = true;
            yield return _beforeReturnWaiter;
            _agent.isStopped = false;
            _agent.SetDestination(alertSeat.position);
        }

        public override void Exit()
        {
            base.Exit();
            _agent.isStopped = true;
            if (_runningCoroutine != null)
            {
                StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }            
        }
    }
}