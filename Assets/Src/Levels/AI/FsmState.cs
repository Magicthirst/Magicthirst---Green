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

        /// <summary>
        /// The safety-net state. If this state finishes and none of the 'Next States' are ready, the FSM will transition here.
        /// </summary>
        [Header("Transitions")]
        [Tooltip("The safety-net state. If this state finishes and none of the 'Next States' are ready, the FSM will transition here.")]
        [SerializeField]
        private FsmState fallback;

        /// <summary>
        /// Potential successor states. When this state finishes, the FSM will transition to the first state in this list that is 'Ready'.
        /// </summary>
        [Tooltip("Potential successor states. When this state finishes, the FSM will transition to the first state in this list that is 'Ready'.")]
        [SerializeField]
        private List<FsmState> nextStates;

        /// <summary>
        /// A list of states that THIS state is allowed to interrupt. If this state becomes 'Ready' while one of these is active, it will force a transition to itself.
        /// </summary>
        [Tooltip("A list of states that THIS state is allowed to interrupt. If this state becomes 'Ready' while one of these is active, it will force a transition to itself.")]
        [SerializeField]
        private List<FsmState> overridesStates;

        private HashSet<FsmState> _overridesSet;

        protected virtual void Awake()
        {
            _overridesSet = new HashSet<FsmState>(overridesStates);
        }

        // ReSharper disable once Unity.RedundantEventFunction
        protected void Update() {}

        public virtual void OnFrame() {}

        public virtual void Enter() {}

        public virtual void Exit() {}

        public bool Overrides(FsmState state) => _overridesSet.Contains(state);

        protected void Ready() => Readied?.Invoke();

        protected void Finish() => Finished?.Invoke();
    }
}