using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Levels.Core;
using Levels.Directorship;
using UnityEngine;
using Util;
using VContainer;
using Random = UnityEngine.Random;

namespace Levels.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundForAbilityReload : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SerializeField] private AbilityReloadSoundMapping[] mappings;

        private AudioSource _audioSource;

        private IReadOnlyDictionary<Type, AbilitySound> _abilitySounds;

        [Inject] private Weaponry _weaponry;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void DidEnabled()
        {
            InitAbilitiesSounds();

            _weaponry.Invoked += OnAbilityInvoked;
        }

        protected override void DidUpdate()
        {
            foreach (var (ability, scheduledSound) in _abilitySounds.Values)
            {
                if (scheduledSound is null)
                {
                    continue;
                }

                if (ability.RemainingCooldown >= scheduledSound.Offset)
                {
                    PlaySound(scheduledSound.Sound);
                }
            }
        }

        protected override void DidDisabled()
        {
            _weaponry.Invoked -= OnAbilityInvoked;
        }

        private void OnAbilityInvoked(IAbility ability)
        {
            if (_abilitySounds.TryGetValue(ability.Type, out var sound))
            {
                var i = Random.Range(0, sound.PossibleSounds.Count);
                sound.ScheduledSound = sound.PossibleSounds[i];
            }
        }

        private void PlaySound(RepeatingSound sound)
        {
            var (clip, pitch) = sound.GetNextClip();

            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }

        private void InitAbilitiesSounds()
        {
            _abilitySounds = mappings
                .Select(mapping =>
                (
                    Ability: _weaponry.Abilities.FirstOrDefault(ability => ability.Type == mapping.Ability),
                    mapping.Sounds
                ))
                .Where(pair => pair.Ability is not null)
                .ToDictionary
                (
                    keySelector: pair => pair.Ability.Type,
                    elementSelector: pair => new AbilitySound
                    {
                        Ability = pair.Ability,
                        ScheduledSound = null,
                        PossibleSounds = pair.Sounds
                    }
                );
        }

        private record AbilitySound
        {
            public IAbility Ability;
            [CanBeNull] public ReloadSoundConfig ScheduledSound;
            public List<ReloadSoundConfig> PossibleSounds;

            public void Deconstruct(out IAbility ability, [CanBeNull] out ReloadSoundConfig scheduledSound)
            {
                ability = Ability;
                scheduledSound = ScheduledSound;
            }
        }

        [Serializable]
        private class AbilityReloadSoundMapping
        {
            public Type Ability => _abilityType ??= Type.GetType(abilityType);

            [field: SerializeField]
            public List<ReloadSoundConfig> Sounds { get; set; }

            [SubtypeProperty(typeof(IInHandAbility))]
            [SerializeField]
            private string abilityType;
            
            private Type _abilityType;
        }

        [Serializable]
        private class ReloadSoundConfig
        {
            [field: SerializeField]
            public RepeatingSound Sound { get; set; }

            [field: SerializeField]
            [field: Tooltip("Time in seconds before the ability reloads to start the sound")]
            public float Offset { get; set; }
        }
    }
}