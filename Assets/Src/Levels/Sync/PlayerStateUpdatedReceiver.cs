using Common;

namespace Levels.Sync
{
    internal interface IPlayerStateUpdatedReceiver
    {
        internal void OnPlayerStateUpdated(PlayerState state);
    }
}