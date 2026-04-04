using System;
using UnityEngine;

namespace Levels
{
    public interface IObservableMovement
    {
        event Action<Vector2> Moved;
    }
}