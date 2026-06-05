using UnityEngine;

namespace Levels.Util
{
    public class DeactivationOnLeavingLevelState : MonoBehaviour
    {
        [SerializeField] private LevelActivityMask leavingMask;
        [SerializeField] private GameObject[] objects;

        private void OnEnable()
        {
            LevelDirector.ActivityMaskChanged += OnActivityMaskChanged;
            OnActivityMaskChanged(default);
        }

        private void OnDisable()
        {
            LevelDirector.ActivityMaskChanged -= OnActivityMaskChanged;
        }

        private void OnActivityMaskChanged((LevelActivityMask, LevelActivityMask) _)
        {
            if ((LevelDirector.ActivityMask & leavingMask) == 0)
            {
                foreach (var o in objects)
                {
                    o.SetActive(false);
                }
            }
        }
    }
}