using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.AI
{
    public delegate void DisposeAction();

    public partial class Fsm : MonoBehaviour
    {
        public Action<FsmState> OnStateChanged;

        [SerializeField] private FsmState initialState;
        public IReadOnlyList<FsmState> States => GetComponents<FsmState>().ToArray();

        private FsmState _currentBacking;
        private FsmState _Current
        {
            get => _currentBacking;
            set
            {
                _currentBacking = value;
                OnStateChanged?.Invoke(_currentBacking);
            }
        }

        private FsmState[] _states;

        private DisposeAction _disposeObservers = delegate {};

        private void Awake()
        {
            _states = States.ToArray();
        }

        private void OnEnable()
        {
            Assert.IsTrue(_states.Length > 0 && initialState is not null);

            RunState(initialState);
            
            _disposeObservers = _states
                .Select(state => RunOnReady(state) + RunNextOnFinish(state))
                .Aggregate((acc, state) => acc + state);
        }

        private void RunState(FsmState state)
        {
            _Current?.Exit();
            _Current = state;
            _Current?.Enter();
        }

        private DisposeAction RunOnReady(FsmState state)
        {
            state.Readied += OnStateReadied;
            return () => state.Readied -= OnStateReadied;

            void OnStateReadied()
            {
                if (_Current.TransitionsTo(state) && state.Overrides(_Current))
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