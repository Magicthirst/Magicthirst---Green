using System;
using System.Linq;
using JetBrains.Annotations;
using Levels.AI;
using Levels.Visual.FsmSpritesVisibility;
using UnityEngine;
using Util;

namespace Levels.Visual
{
    public class FsmSpritesVisibilityController : MonoBehaviour
    {
        [SerializeField] private SpriteVisibilityRule[] rules;

        private Fsm _fsm;
        private SpriteRenderer[] _sprites;

        private void Awake()
        {
            _fsm = GetComponent<Fsm>() ?? GetComponentInParent<Fsm>();
            _sprites = GetComponentsInChildren<SpriteRenderer>();

            Array.Sort(rules);
        }

        private void OnEnable()
        {
            _fsm.OnStateChanged += ResolveVisibilities;
        }

        private void Start()
        {
            ResolveVisibilities();
        }

        private void OnDisable()
        {
            _fsm.OnStateChanged -= ResolveVisibilities;
        }

        private void ResolveVisibilities([CanBeNull] FsmState state = null)
        {
            state ??= _fsm.Current;

            var rulesForState = rules.Where(rule => rule.state.IsAppliesTo(state)).ToArray(); 

            foreach (var sprite in _sprites)
            {
                if (rulesForState.TryGetFirst(out var rule, rule => rule.sprite == sprite))
                {
                    sprite.enabled = rule.visible;
                }
            }
        }
    }
}

namespace Levels.Visual.FsmSpritesVisibility
{
    [Serializable]
    public class SpriteVisibilityRule : IComparable<SpriteVisibilityRule>
    {
        [SerializeReference]
        [SubclassSelector]
        public IStatePredicate state;
        public SpriteRenderer sprite;
        public bool visible;

        public int CompareTo(SpriteVisibilityRule other) => state.Order.CompareTo(other.state.Order);

        public override string ToString() => $"SpriteVisibilityRule {{ {state} && {sprite.name} => {(visible ? "+" : "-")} }}";
    }

    public interface IStatePredicate
    {
        public int Order { get; }

        public bool IsAppliesTo(FsmState key);
    }

    [Serializable]
    public class Is : IStatePredicate
    {
        public int Order => 0;

        public FsmState[] anyOfThese;

        public bool IsAppliesTo(FsmState key) => anyOfThese.Contains(key);

        public override string ToString() => $"Is anyOfThese ({string.Join(",", anyOfThese.Select(state => state.ToString()))})";
    }

    [Serializable]
    public class IsNot : IStatePredicate
    {
        public int Order => 1;

        public FsmState[] anyOfThese;

        public bool IsAppliesTo(FsmState key) => !anyOfThese.Contains(key);

        public override string ToString() => $"Is Not anyOfThese ({string.Join(",", anyOfThese.Select(state => state.ToString()))})";
    }
}