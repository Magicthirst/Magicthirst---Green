using System;
using System.Collections;
using Levels.Core.Statuses;
using Levels.Util;
using Levels.Core;
using Levels.Directorship;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundOnStatus : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SubtypeProperty(typeof(IStatus))]
        [SerializeField] private string statusType;
        private Type _StatusType => _statusType ??= Type.GetType(statusType);
        private Type _statusType;

        [SerializeField] private RepeatingSound sound;

        private AudioSource _audioSource;
        private Coroutine _soundRoutine;
        private InterruptionQueue _statusInterruptions;

        [Inject] private StatusesRepository _statuses;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = false;
            
            _statusInterruptions = new InterruptionQueue(this, null); 
        }

        protected override void DidEnabled()
        {
            _statuses.StatusApplied += OnApplied;
            _statuses.StatusDisappeared += OnDisappeared;
        }

        protected override void DidDisabled()
        {
            _statuses.StatusApplied -= OnApplied;
            _statuses.StatusDisappeared -= OnDisappeared;
            StopSoundRoutine();
            
            _statusInterruptions?.Dispose();
        }

        private void OnApplied(IStatus status)
        {
            if (status.GetType() == _StatusType && _soundRoutine == null)
            {
                _soundRoutine = StartCoroutine(PlayRepeatingSoundRoutine()
                    .WithInterruptions(_statusInterruptions)
                    .WithInterruptions(_LevelLifecycle));
            }
        }

        private void OnDisappeared(IStatus status)
        {
            if (status.GetType() == _StatusType)
            {
                StopSoundRoutine();
            }
        }

        private IEnumerator PlayRepeatingSoundRoutine()
        {
            while (true)
            {
                var (clip, pitch) = sound.GetNextClip();

                _audioSource.pitch = pitch;
                _audioSource.clip = clip;
                _audioSource.Play();

                yield return InterruptableWait.ForSeconds(clip.length);
            }
        }

        private void StopSoundRoutine()
        {
            if (_soundRoutine != null)
            {
                StopCoroutine(_soundRoutine);
                _soundRoutine = null;
            }
            
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}