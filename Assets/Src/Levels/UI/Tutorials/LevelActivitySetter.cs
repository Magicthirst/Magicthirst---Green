using Levels.Directorship;
using UnityEngine;

namespace Levels.UI.Tutorials
{
    public class LevelActivitySetter : MonoBehaviour
    {
        [SerializeField] private LevelActivityMask levelActivityMask;

        public void OnEnable()
        {
            LevelDirector.ActivityMask = levelActivityMask;
        }
    }
}