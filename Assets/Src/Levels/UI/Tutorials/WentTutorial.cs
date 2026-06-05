using UnityEngine;

namespace Levels.UI.Tutorials
{
    public class WentTutorial : MonoBehaviour
    {
        public void OnEnable()
        {
            LevelDirector.ActivityMask = LevelActivityMask.Tutorial;
        }
    }
}