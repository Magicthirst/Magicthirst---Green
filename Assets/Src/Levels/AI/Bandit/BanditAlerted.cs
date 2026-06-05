using System.Collections;
using JetBrains.Annotations;
using Levels.Util;
using UnityEngine;
using UnityEngine.AI;

namespace Levels.AI.Bandit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BanditAlerted : FsmState
    {
        [SerializeField]
        [CanBeNull]
        private Transform alertSeat = null;
        [SerializeField] private float beforeReturnDelay;

        private Vector3 _alertPosition; 

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

        private void Start()
        {
            _alertPosition = alertSeat != null ? alertSeat.position : transform.position;
        }

        public override void Enter()
        {
            base.Enter();
            _runningCoroutine = StartCoroutine(RunToAlertPosition().WithInterruptions(_LevelLifecycle));
        }

        private IEnumerator RunToAlertPosition()
        {
            _agent.isStopped = true;
            yield return _beforeReturnWaiter;
            _agent.isStopped = false;
            _agent.SetDestination(_alertPosition);
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