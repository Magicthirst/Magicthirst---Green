using System;
using UnityEngine;

namespace Levels
{
    public interface IMovementInputSource
    {
        public Vector2 Movement { get; }

        public event Action<Vector2> PositionUpdated;
    }
}