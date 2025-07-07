using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    public class SyncBehaviour : MonoBehaviour
    {
        [Inject] private IsOnline _isOnline;

        public delegate bool IsOnline();

        protected virtual void Awake()
        {
            if (!_isOnline())
            {
                Destroy(this);
            }
        }
    }
}
