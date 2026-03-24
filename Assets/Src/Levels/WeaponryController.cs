using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Levels
{
    [RequireComponent(typeof(PlayerInput))]
    public class WeaponryController : MonoBehaviour
    {
        [SerializeField] private InputActionReference primaryKey;
        [SerializeField] private InputActionReference secondaryKey;

        private PlayerInput _playerInput;

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

        private void OnEnable()
        {
            _inputObservers = ObserveInputs();
            _weaponry.Invoked += OnAbilityInvoked;
        }

        private void OnDisable()
        {
            foreach (var observer in _inputObservers)
            {
                observer.Dispose();
            }
            _inputObservers = null;
            _weaponry.Invoked -= OnAbilityInvoked;
        }

        private ICollection<IDisposable> ObserveInputs()
        {
            var map = _playerInput.currentActionMap;

            return _weaponry.Abilities
                .Select(ability => map
                    .ConsumeAction(ability.InputActionName)
                    .OnPerformed(ability.Equip))
                .Append(map
                    .ConsumeAction(primaryKey.action.name)
                    .OnPerformed(_weaponry.InvokePrimary))
                .Append(map
                    .ConsumeAction(secondaryKey.action.name)
                    .OnPerformed(_weaponry.InvokeSecondary))
                .ToArray();
        }
    }
}