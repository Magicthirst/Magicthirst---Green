using Levels.Directorship;
using UnityEngine;

namespace Levels.UI.Tutorials
{
    public class LevelActivitySetter : MonoBehaviour
    {
        [SerializeField] private EditorLevelActivityMask levelActivityMask;

        public void OnEnable()
        {
            LevelDirector.ActivityMask = (LevelActivityMask)levelActivityMask;
        }
    }
}