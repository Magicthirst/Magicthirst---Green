using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.AI
{
    public delegate void DisposeAction();

    public partial class Fsm : MonoBehaviour
    {
        public IReadOnlyList<FsmState> States => GetComponents<FsmState>().ToArray();

        private FsmState[] _states;
        private FsmState _current;

        private DisposeAction _disposeObservers = delegate {};

        private void Awake()
        {
            _states = States.ToArray();
        }

        private void OnEnable()
        {
            Assert.IsTrue(_states.Length > 0);

            RunState(_states[0]);
            
            _disposeObservers = _states
                .Select(state => RunOnReady(state) + RunNextOnFinish(state))
                .Aggregate((acc, state) => acc + state);
        }

        private void RunState(FsmState state)
        {
            Debug.Log($"FSM exiting {_current?.name}", gameObject);
            Debug.Log($"FSM entering {state?.name}", gameObject);
            _current?.Exit();
            _current = state;
            state?.Enter();
        }

        private DisposeAction RunOnReady(FsmState state)
        {
            state.Readied += OnStateReadied;
            return () => state.Readied -= OnStateReadied;

            void OnStateReadied()
            {
                Debug.Log($"FSM ready to run {state.name}", gameObject);

                if (_current.TransitionsTo(state) && state.Overrides(_current))
                {
                    RunState(state);
                }
            }
        }

        private DisposeAction RunNextOnFinish(FsmState state)
        {
            state.Finished += OnStateFinished;
            return () => state.Finished -= OnStateFinished;

            void OnStateFinished() => RunState(state.Next);
        }

        private void OnDisable()
        {
            RunState(null);
            _disposeObservers?.Invoke();
        }
    }
}