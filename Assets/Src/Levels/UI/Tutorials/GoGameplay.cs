using UnityEngine;

namespace Levels.UI.Tutorials
{
    public class GoGameplay : MonoBehaviour
    {
        public void Go()
        {
            LevelDirector.ActivityMask = LevelActivityMask.Gameplay;
        }
    }
}