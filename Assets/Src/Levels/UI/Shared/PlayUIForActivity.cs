using System.Collections;
using JetBrains.Annotations;
using Levels.Directorship;
using Levels.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Levels.UI.Shared
{
    public class PlayUIForActivity : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)mask;

        [SerializeField] private EditorLevelActivityMask mask;
        [SerializeField] private Image animatedSprite;
        [SerializeField] private Graphic[] others;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float frameDuration;
        [SerializeField] private float fadeDuration;

        [CanBeNull] private Coroutine _animationFadeCoroutine;
        [CanBeNull] private Coroutine _othersFadeCoroutine;
        [CanBeNull] private Coroutine _animationCoroutine;

        protected override void DidEnabled()
        {
            if (_animationFadeCoroutine is not null)
            {
                StopCoroutine(_animationFadeCoroutine);
                _animationFadeCoroutine = null;
            }
            if (_othersFadeCoroutine is not null)
            {
                StopCoroutine(_othersFadeCoroutine);
                _othersFadeCoroutine = null;
            }
            if (_animationCoroutine is not null)
            {
                return;
            }

            _animationFadeCoroutine = StartCoroutine(Fade(0f, 1f, animatedSprite));
            _animationCoroutine = StartCoroutine(PlayAnimation());
            foreach (var other in others)
            {
                other.color = other.color.With(a: 1f);
            }
        }

        protected override void DidDisabled()
        {
            if (_animationFadeCoroutine is not null)
            {
                StopCoroutine(_animationFadeCoroutine);
                _animationFadeCoroutine = null;
            }
            if (_othersFadeCoroutine is not null)
            {
                StopCoroutine(_othersFadeCoroutine);
                _othersFadeCoroutine = null;
            }

            var wasntRunning = _animationCoroutine is null;
            if (wasntRunning)
            {
                return;
            }

            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
            _animationFadeCoroutine = StartCoroutine(Fade(1f, 0f, animatedSprite));
            _othersFadeCoroutine = StartCoroutine(Fade(1f, 0f, others));
        }

        private IEnumerator PlayAnimation()
        {
            var waitForFrame = new WaitForSeconds(frameDuration);

            for (var index = 0;; index = ++index % sprites.Length)
            {
                animatedSprite.sprite = sprites[index];
                yield return waitForFrame;
            }
        }

        private IEnumerator Fade(float from, float to, params Graphic[] targets)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                var alpha = Mathf.Lerp(from, to, t / fadeDuration);

                foreach (var graphic in targets)
                {
                    graphic.color = graphic.color.With(a: alpha);
                }

                yield return null;
            }

            foreach (var graphic in targets)
            {
                graphic.color = graphic.color.With(a: to);
            }
        }
    }
}