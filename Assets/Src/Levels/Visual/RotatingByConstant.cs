using Levels.Directorship;
using UnityEngine;

namespace Levels.Visual
{
    public class RotatingByConstant : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SerializeField] public bool running;
        [SerializeField] private Vector3 vector;

        protected override void DidUpdate()
        {
            if (running)
            {
                transform.eulerAngles += vector * LevelDirector.GameplayDeltaTime;
            }
        }
    }
}