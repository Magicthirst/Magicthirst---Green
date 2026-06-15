using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Directorship;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.AI
{
    using DisposeAction = Action;

    public partial class Fsm : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay;

        public Action<FsmState> OnStateChanged;

        public FsmState Current => _Current;

        /// <summary>
        /// Starting state
        /// <code>[*] --> Fsm.initialState</code>
        /// </summary>
        [Tooltip("Starting state")]
        [SerializeField]
        private FsmState initialState;
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
            DebugAwake();
        }

        protected override void DidEnabled()
        {
            Assert.IsTrue(_states.Length > 0 && initialState is not null);

            RunState(initialState);
            
            _disposeObservers = _states
                .Select(state => RunOnReady(state) + RunNextOnFinish(state))
                .Aggregate((acc, state) => acc + state);
        }

        protected override void DidUpdate()
        {
            _Current?.OnFrame();
            DebugUpdate();
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
                if (state.Overrides(_Current))
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

        protected override void DidDisabled()
        {
            RunState(null);
            _disposeObservers?.Invoke();
        }
    }
}