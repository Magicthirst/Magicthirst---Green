using System;
using UnityEngine;
using Web.Sync;

namespace Levels.Sync
{
    public class Output : SyncBehaviour
    {
        private IMovementInputSource _movementInput;

        private PlayerUpdate _update = new();

        public bool TryCollectPlayerUpdate(out PlayerUpdate update)
        {
            update = _update; // PlayerUpdate is struct, thus, here copying happens instead of assigning the reference
            _update.Clear();
            return !_update.IsEmpty();
        }

        protected override void Awake()
        {
            base.Awake();

            _movementInput = GetComponent<IMovementInputSource>()!;
            Service.instance.PlayerOutput = this;
        }

        private void OnEnable()
        {
            _movementInput.PositionChanged += OnPositionChanged;
            _movementInput.VelocityChanged += OnVelocityChanged;
        }

        private void OnDisable()
        {
            _movementInput.PositionChanged -= OnPositionChanged;
            _movementInput.VelocityChanged -= OnVelocityChanged;
        }

        private void OnPositionChanged(Vector3 v) => _update.Position = (v.x, v.y, v.z);

        private void OnVelocityChanged(Vector3 v) => _update.Velocity = (v.x, v.y, v.z);
    }

    public interface IMovementInputSource
    {
        public event Action<Vector3> PositionChanged;
        public event Action<Vector3> VelocityChanged;
    }
}
