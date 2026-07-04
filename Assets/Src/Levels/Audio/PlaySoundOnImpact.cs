using System;
using System.Collections.Generic;
using Levels.Directorship;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundOnImpact : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SerializeField] private ImpactSoundMapping[] mappings;

        private AudioSource _audioSource;

        private readonly Dictionary<IImpactConsumer, Action> _subscriptions = new();

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            foreach (var mapping in mappings)
            {
                var impactType = Type.GetType(mapping.ImpactType);
                var consumerType = typeof(IImpactConsumer<>).MakeGenericType(impactType);

                var consumer = (IImpactConsumer)resolver.Resolve(consumerType);

                _subscriptions[consumer] = () => PlaySound(mapping.Sound);
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void DidEnabled()
        {
            foreach (var pair in _subscriptions)
            {
                pair.Key.Impacted += pair.Value;
            }
        }

        protected override void DidDisabled()
        {
            foreach (var pair in _subscriptions)
            {
                pair.Key.Impacted -= pair.Value;
            }
        }

        private void PlaySound(RepeatingSound sound)
        {
            var (clip, pitch) = sound.GetNextClip();

            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }

        [Serializable]
        private class ImpactSoundMapping
        {
            [field: SubtypeProperty(typeof(IImpact))]
            [field: SerializeField]
            public string ImpactType { get; set; }

            [field: SerializeField]
            public RepeatingSound Sound { get; set; }
        }
    }
}