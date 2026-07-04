using System;
using System.Collections;
using Levels.Directorship;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundDuringCast : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SubtypeProperty(typeof(IImpact))]
        [SerializeField] private string startImpactType;

        [SubtypeProperty(typeof(IImpact))]
        [SerializeField] private string endImpactType;

        [SerializeField] private RepeatingSound sound;

        private AudioSource _audioSource;
        private Coroutine _soundRoutine;
        private InterruptionQueue _castInterruptions;

        private IImpactConsumer _startConsumer;
        private IImpactConsumer _endConsumer;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            var startType = Type.GetType(startImpactType);
            var startConsumerType = typeof(IImpactConsumer<>).MakeGenericType(startType);
            _startConsumer = (IImpactConsumer)resolver.Resolve(startConsumerType);

            var endType = Type.GetType(endImpactType);
            var endConsumerType = typeof(IImpactConsumer<>).MakeGenericType(endType);
            _endConsumer = (IImpactConsumer)resolver.Resolve(endConsumerType);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = false;
            
            _castInterruptions = new InterruptionQueue(this, null); //
        }

        protected override void DidEnabled()
        {
            _startConsumer.Impacted += OnStartCasting;
            _endConsumer.Impacted += OnEndCasting;
        }

        protected override void DidDisabled()
        {
            _startConsumer.Impacted -= OnStartCasting;
            _endConsumer.Impacted -= OnEndCasting;
            
            StopSoundRoutine();
            _castInterruptions?.Dispose();
        }

        private void OnStartCasting()
        {
            _soundRoutine ??= StartCoroutine(PlayRepeatingSoundRoutine()
                .WithInterruptions(_castInterruptions)
                .WithInterruptions(_LevelLifecycle));
        }

        private void OnEndCasting()
        {
            StopSoundRoutine();
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