using System;
using UnityEngine;

namespace Levels
{
    public interface IObservableMovement
    {
        event Action MovementUpdated;

        Vector2 AbsoluteMovement { get; }

        Vector2 RelativeMovement { get; }
    }
}