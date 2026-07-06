using Levels.Directorship;
using UnityEngine;

namespace Levels.Util
{
    public class MouseLock : MonoBehaviour
    {
        private void OnEnable()
        {
            LevelDirector.ActivityMaskChanged += OnActivityMaskChanged;
        }

        private void OnActivityMaskChanged((LevelActivityMask _, LevelActivityMask mask) p)
        {
            if (p.mask == LevelActivityMask.Pause)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnDisable()
        {
            LevelDirector.ActivityMaskChanged -= OnActivityMaskChanged;
        }
    }
}