using System.Threading;
using Common;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public abstract class SyncBehavior : MonoBehaviour
    {
        protected ISyncConnection Connection;

        private SynchronizationContext _mainThreadContext;

        [Inject]
        public void ObserveConnection(IConnectionEstablishedEventHolder eventHolder)
        {
            eventHolder.ConnectionEstablished += connection =>
            {
                Connection = connection;
                _mainThreadContext.Post(_ => enabled = true, null);
            };
        }

        protected virtual void Awake()
        {
            enabled = false;
            _mainThreadContext = SynchronizationContext.Current;
        }
    }
}
