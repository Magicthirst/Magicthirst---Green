using Levels.Directorship;
using UnityEngine;

namespace Levels.Visual
{
    public class Pulsating : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SerializeField] private AnimationCurve size;

        private Vector3 _baseScale;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        protected override void DidUpdate()
        {
            transform.localScale = _baseScale * size.Evaluate(LevelDirector.GameplayTime);
        }
    }
}