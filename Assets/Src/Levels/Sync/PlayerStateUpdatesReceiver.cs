using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Levels.Sync
{
    public class PlayerStateUpdatesReceiver : MonoBehaviour
    {
        [CanBeNull] internal event MovementCommand MovementUpdated;

        internal void OnPlayerStateUpdated(PlayerState state)
        {
            transform.position = state.Position;
            MovementUpdated?.Invoke(state.Position, state.Vector, 0);
        }
    }
}