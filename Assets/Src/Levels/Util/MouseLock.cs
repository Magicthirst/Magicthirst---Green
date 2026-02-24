using UnityEngine;

namespace Levels.Util
{
    public class MouseLock : MonoBehaviour
    {
        private CursorLockMode _savedLockMode;

        private void OnEnable()
        {
            _savedLockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.lockState = _savedLockMode;
            Cursor.visible = true;
        }
    }
}