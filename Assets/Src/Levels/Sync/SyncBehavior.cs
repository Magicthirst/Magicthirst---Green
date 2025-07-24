using System.Threading;
using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public abstract class SyncBehavior : MonoBehaviour
    {
        private SynchronizationContext _mainThreadContext;

        [Inject]
        public void ObserveConnection(IConnectionEstablishedEventHolder eventHolder)
        {
            enabled = true;
            eventHolder.ConnectionEstablished += () => _mainThreadContext.Post(_ => enabled = true, null);
        }

        protected virtual void Awake()
        {
            enabled = false;
            _mainThreadContext = SynchronizationContext.Current;
        }
    }
}
