using System;
using UnityEngine;

namespace Levels
{
    public interface IMovementInputSource : IObservableMovement
    {
        event Action<Vector2> ForcePositionUpdated;
    }
}