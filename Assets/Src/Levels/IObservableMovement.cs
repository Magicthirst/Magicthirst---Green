using System;
using UnityEngine;

namespace Levels
{
    public interface IObservableMovement
    {
        event Action<Vector2> MovementUpdated;

        Vector2 Movement { get; }
    }
}