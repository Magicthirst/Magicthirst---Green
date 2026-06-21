using System.Collections;
using Levels.Directorship;
using Levels.Extensions;
using Levels.Util;
using UnityEngine;

namespace Levels.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFadingOnEnable : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)activityType;

        [SerializeField] private EditorLevelActivityMask activityType;
        [SerializeField] private float fadingTime;
        [SerializeField] private GameObject destroyOnFadeEnd;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        protected override void DidEnabled()
        {
            StartCoroutine(Fade().WithInterruptions(_LevelLifecycle));
            Debug.Log("Launched fading coroutine");
        }

        private IEnumerator Fade()
        {
            var startAlpha = _renderer.color.a;
            var timeLeft = fadingTime;
            Debug.Log("Started fading");

            while (timeLeft > 0)
            {
                _renderer.color = _renderer.color.With(a: Mathf.Lerp(0f, startAlpha, timeLeft / fadingTime));

                yield return null;
                timeLeft -= Time.deltaTime;
            }

            if (destroyOnFadeEnd != null)
            {
                Destroy(destroyOnFadeEnd);
            }
        }
    }
}