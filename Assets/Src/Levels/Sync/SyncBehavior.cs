using System.Threading;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public abstract class SyncBehavior : MonoBehaviour
    {
        protected ISyncConnection Connection;
        protected SynchronizationContext MainThreadContext;

        [Inject]
        public void ObserveConnection(IConnectionEstablishedEventHolder eventHolder)
        {
            eventHolder.ConnectionEstablished += connection =>
            {
                Connection = connection;
                MainThreadContext.Post(_ => enabled = true, null);
            };
        }

        protected virtual void Awake()
        {
            enabled = false;
            MainThreadContext = SynchronizationContext.Current;
        }
    }
}
