using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core;
using Levels.Directorship;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using static Levels.Directorship.WeaponryMasks;
using static Levels.Directorship.LevelActivityMask;

namespace Levels
{
    public class WeaponryController : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => Gameplay | TutorialWeapon;

        [SerializeField] private InputActionReference primaryKey;
        [SerializeField] private InputActionReference secondaryKey;

        [Inject] private PlayerInput _playerInput;
        [Inject] private Weaponry _weaponry;

        private Dictionary<IAbility, IInHandAbility> _abilities;
        private IEnumerable<IDisposable> _inputObservers;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _abilities = _weaponry.Abilities.ToDictionary(
                keySelector: ability => ability,
                elementSelector: ability => ability.FindIn(gameObject)
            );
        }

        private void OnAbilityInvoked(IAbility ability) => _abilities[ability].Invoke();

        protected override void DidEnabled()
        {
            _inputObservers = ObserveInputs();
            _weaponry.Invoked += OnAbilityInvoked;
        }

        protected override void DidDisabled()
        {
            if (_inputObservers != null)
            {
                foreach (var observer in _inputObservers)
                {
                    observer.Dispose();
                }
                _inputObservers = null;
            }
            _weaponry.Invoked -= OnAbilityInvoked;
        }

        private ICollection<IDisposable> ObserveInputs()
        {
            var map = _playerInput.currentActionMap;

            return _weaponry.Abilities
                .Select(ability => map
                    .ConsumeAction(ability.InputActionName)
                    .OnPerformed(() => { if (ability.Type.IsPlayableNow()) ability.Equip(); }))
                .Append(map
                    .ConsumeAction(primaryKey.action.name)
                    .OnPerformed(() => { if (IsPrimaryInvokable) _weaponry.InvokePrimary(); }))
                .Append(map
                    .ConsumeAction(secondaryKey.action.name)
                    .OnPerformed(() => { if (IsSecondaryInvokable) _weaponry.InvokeSecondary(); }))
                .ToArray();
        }
    }
}