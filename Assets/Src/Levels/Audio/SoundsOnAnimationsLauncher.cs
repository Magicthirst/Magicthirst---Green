using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Levels.Directorship;
using Levels.Visual;
using UnityEngine;
using Util;

namespace Levels.Audio
{
    /// <summary>
    /// Plays specific sound effects when specific sprite sequences are played during animations.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundsOnAnimationsLauncher : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [Header("References")]
        [SerializeField] private SpriteChangeSource spriteSource;
        
        [Tooltip("Mappings defining which sequence of sprites triggers which audio effect.")]
        [SerializeField] private SoundMapping[] mappings;

        private AudioSource _audioSource;
        private readonly List<Sprite> _spriteHistory = new();
        private int _maxRequiredHistoryLength;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _maxRequiredHistoryLength = mappings.Max(mapping => mapping.TriggerSequence.Count);
        }

        protected override void DidEnabled()
        {
            spriteSource.SpriteChanged += OnSpriteChanged;
            _spriteHistory.Clear();
        }

        protected override void DidDisabled()
        {
            spriteSource.SpriteChanged -= OnSpriteChanged;
        }

        private void OnSpriteChanged(Sprite newSprite)
        {
            if (_spriteHistory.Count != 0 && newSprite == _spriteHistory[^1])
            {
                return;
            }

            if (_maxRequiredHistoryLength == 0)
            {
                return;
            }

            _spriteHistory.Add(newSprite);

            if (_spriteHistory.Count > _maxRequiredHistoryLength)
            {
                _spriteHistory.RemoveAt(0);
            }

            CheckForSoundTriggers();
        }

        private void CheckForSoundTriggers()
        {
            if (mappings.TryGetFirst(out var mapping, m => _spriteHistory.EndsWith(m.TriggerSequence)))
            {
                StartCoroutine(PlaySound(mapping.Sound));
            }
        }

        private IEnumerator PlaySound(RepeatingSound sound)
        {
            yield return new WaitForEndOfFrame();
            var (clip, pitch) = sound.GetNextClip();

            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }

        [Serializable]
        private class SoundMapping
        {
            [Tooltip("The exact sequence of sprites that must play in order to trigger this sound.")]
            [field: SerializeField] public List<Sprite> TriggerSequence { get; set; }
            
            [Tooltip("The sound configurations containing clips and pitch variances to pull from.")]
            [field: SerializeField] public RepeatingSound Sound { get; set; }
        }
    }
}