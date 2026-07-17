using Levels.Directorship;
using UnityEngine;

namespace Levels.UI.Shared
{
    public class DeactivateOnEndOfActivity : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)mask;

        [SerializeField] private EditorLevelActivityMask mask;

        protected override void DidDisabled()
        {
            gameObject.SetActive(false);
        }
    }
}