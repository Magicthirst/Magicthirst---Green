using System;
using System.Collections;
using Levels.Directorship;
using UnityEngine;

namespace Levels.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundLoopingForActivity : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)mask;

        [Header("Note: fades the AudioSource for all the using it")]
        [SerializeField] private EditorLevelActivityMask mask;
        [SerializeField] private RepeatingSound sound;
        [SerializeField] [Min(0.001f)] private float fadeDuration;

        private AudioSource _source;
        private Coroutine _fadeCoroutine;
        private float _targetVolume;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _targetVolume = _source.volume;
            _source.loop = true;
        }

        protected override void DidEnabled()
        {
            var (clip, pitch) = sound.GetNextClip();
            _source.clip = clip;
            _source.pitch = pitch;

            StartFade(0f, _targetVolume);
            _source.Play();
        }

        protected override void DidDisabled()
        {
            StartFade(_source.volume, 0f, _source.Stop);
        }

        private void StartFade(float startVolume, float targetVolume, Action onComplete = null)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeAudioRoutine(startVolume, targetVolume, onComplete));
        }

        private IEnumerator FadeAudioRoutine(float startVolume, float targetVolume, Action onComplete)
        {
            var currentTime = 0f;
            _source.volume = startVolume;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                _source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
                yield return null;
            }

            _source.volume = targetVolume;
            onComplete?.Invoke();
            _fadeCoroutine = null;
        }
    }
}