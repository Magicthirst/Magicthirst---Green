using System;
using Common;

namespace Levels.Sync
{
    public interface IConnectionEstablishedEventHolder
    {
        public event Action<ISyncConnection> ConnectionEstablished;
    }
}