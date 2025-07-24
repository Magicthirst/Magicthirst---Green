using System;

namespace Levels.Sync
{
    public interface IConnectionEstablishedEventHolder
    {
        public event Action ConnectionEstablished;
    }
}