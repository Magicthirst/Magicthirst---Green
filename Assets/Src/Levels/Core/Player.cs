using System.Linq;
using UnityEngine;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "Entity", menuName = "Core/Entities/Player", order = 1)]
    public class Player : Entity
    {
        public Health Health { get; private set; }
        public TeleportChip TeleportChip { get; private set; }

        public override void Init()
        {
            base.Init();

            Health = (Health) components.First(c => c is Health);
            TeleportChip = (TeleportChip) components.First(c => c is TeleportChip);
        }
    }

    public enum TeleportChipState { Ready, Thrown, OnGround }
}