using System;
using Levels.Directorship;
using UnityEngine;
using static Levels.Util.ActivityOnLevelStateMode;

namespace Levels.Util
{
    public class ActivityOnLevelState : MonoBehaviour
    {
        [SerializeField] private ActivityOnLevelStateMode mode = TurnOff;
        [SerializeField] private LevelActivityMask activityMask;
        [SerializeField] private GameObject[] objects;

        private void OnEnable()
        {
            LevelDirector.ActivityMaskChanged += OnActivityMaskChanged;
        }

        private void OnDisable()
        {
            LevelDirector.ActivityMaskChanged -= OnActivityMaskChanged;
        }

        private void OnActivityMaskChanged((LevelActivityMask, LevelActivityMask) _)
        {
            var isActive = (LevelDirector.ActivityMask & activityMask) != 0;

            if (isActive && (mode & TurnOn) == 0 || !isActive && (mode & TurnOff) == 0)
            {
                return;
            }

            foreach (var o in objects)
            {
                o.SetActive(isActive);
            }
        }
    }

    [Flags]
    public enum ActivityOnLevelStateMode
    {
        None = 0b00,
        TurnOn = 0b01,
        TurnOff = 0b10,
        All = 0b11
    }
}