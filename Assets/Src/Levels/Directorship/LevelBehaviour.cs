using Levels.Util;
using UnityEngine;

namespace Levels.Directorship
{
    public abstract class LevelBehaviour : MonoBehaviour
    {
        protected abstract LevelActivityMask _LifecycleMask { get; }

        private bool _MustRun => (LevelDirector.ActivityMask & _LifecycleMask) != 0;
        protected InterruptionQueue _LevelLifecycle => LevelDirector.Interruptions[_LifecycleMask];

        protected void OnEnable()
        {
            LevelDirector.ActivityMaskChanged += OnMaskChanged;
        }

        protected void Update()
        {
            if (_MustRun)
            {
                DidUpdate();
            }
        }

        protected void FixedUpdate()
        {
            if (_MustRun)
            {
                DidFixedUpdate();
            }
        }

        protected void OnDisable()
        {
            LevelDirector.ActivityMaskChanged -= OnMaskChanged;
            DidDisabled();
        }

        protected virtual void DidEnabled() {}

        protected virtual void DidUpdate() {}

        protected virtual void DidFixedUpdate() {}

        protected virtual void DidDisabled() {}

        private void OnMaskChanged((LevelActivityMask previous, LevelActivityMask current) p)
        {
            var wasRunning = (_LifecycleMask & p.previous) != 0;
            var mustRun = (_LifecycleMask & p.current) != 0;

            if (wasRunning == mustRun)
            {
                return;
            }

            if (mustRun)
            {
                DidEnabled();
            }
            else
            {
                DidDisabled();
            }
        }
    }
}