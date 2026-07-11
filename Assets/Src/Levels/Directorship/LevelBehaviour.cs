using Levels.Util;
using UnityEngine;

namespace Levels.Directorship
{
    public abstract class LevelBehaviour : MonoBehaviour
    {
        protected abstract LevelActivityMask _LifecycleMask { get; }

        private bool _MustRun => _LifecycleMask.IsRunningDuring(LevelDirector.ActivityMask);
        protected InterruptionQueue _LevelLifecycle => LevelDirector.Interruptions[_LifecycleMask];

        protected void OnEnable()
        {
            LevelDirector.ActivityMaskChanged += OnMaskChanged;
            if (LevelDirector.IsStarted)
            {
                OnMaskChanged((0, LevelDirector.ActivityMask));
            }
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

        protected virtual void OnMaskChanged(LevelActivityMask previous, LevelActivityMask current) {}

        private void OnMaskChanged((LevelActivityMask previous, LevelActivityMask current) p)
        {
            var wasRunning = _LifecycleMask.IsRunningDuring(p.previous);
            var mustRun = _LifecycleMask.IsRunningDuring(p.current);

            if (wasRunning == mustRun)
            {
                return;
            }

            OnMaskChanged(p.previous, p.current);

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