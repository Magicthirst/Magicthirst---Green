using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Levels.AI
{
    public abstract partial class FsmState : MonoBehaviour
    {
        public event Action Readied;
        public event Action Finished;

        public FsmState Next => nextStates.FirstOrDefault(next => next._IsReady) ?? fallback;

        protected abstract bool _IsReady { get; }

        [SerializeField] private List<FsmState> nextStates;
        [SerializeField] private List<FsmState> overridesStates;
        [SerializeField] private FsmState fallback;

        private HashSet<FsmState> _overridesSet;
        private HashSet<FsmState> _nextStatesSet;

        protected virtual void Awake()
        {
            _overridesSet = new HashSet<FsmState>(overridesStates);
            _nextStatesSet = new HashSet<FsmState>(nextStates);
        }

        public virtual void Enter() {}

        public virtual void Exit() {}

        public bool Overrides(FsmState state) => _overridesSet.Contains(state);

        public bool TransitionsTo(FsmState state) => _nextStatesSet.Contains(state);

        protected void Ready() => Readied?.Invoke();

        protected void Finish() => Finished?.Invoke();
    }
}