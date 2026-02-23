using System;
using UnityEngine;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "TeleportChip", menuName = "Core/Components/TeleportChip", order = 1)]
    public class TeleportChip : CoreObject
    {
        public event Action<TeleportChipState> StateChanged;

        public TeleportChipState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }

        public Abilities.TeleportChip.TeleportChip Instance { get; private set; }

        private TeleportChipState _state;

        public override void Init()
        {
            _state = TeleportChipState.Ready;
        }

        public void Register(Abilities.TeleportChip.TeleportChip instance) => Instance = instance;

        public void Throw()
        {
            State = TeleportChipState.Thrown;
        }

        public void Land()
        {
            State = TeleportChipState.OnGround;
        }

        public void Restore()
        {
            State = TeleportChipState.Ready;
        }

        public override void Dispose()
        {
        }
    }
}