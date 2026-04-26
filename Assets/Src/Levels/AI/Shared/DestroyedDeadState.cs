using Levels.Core;
using UnityEngine;
using VContainer;

namespace Levels.AI.Shared
{
    public class DestroyedDeadState : FsmState
    {
        protected override bool _IsReady => _health.IsDown;

        [Inject] private Health _health;
        [Inject] private GameObject _self;

        public override void Enter()
        {
            base.Enter();
            _self.SetActive(false);
            Destroy(_self);
        }
    }
}