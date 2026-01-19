using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Levels
{
    [RequireComponent(typeof(PlayerInput))]
    public class WeaponryController : MonoBehaviour
    {
        [FormerlySerializedAs("_actionMappings")] // To hide this field being a workaround for dictionary
        [SerializeField]
        private List<ActionAbilityMappingSerial> actionMappingsSerial;

        private PlayerInput _playerInput;

        private ActionAbilityMapping[] _actionMappings;
        private IInHandAbility _primaryAbility;
        private IInHandAbility _secondaryAbility;

        private IEnumerable<IDisposable> _inputObservers;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            CheckMappings();
            _actionMappings = actionMappingsSerial
                .Select(mapping => new ActionAbilityMapping(mapping.actionName, mapping.position, (IInHandAbility) mapping.ability))
                .ToArray();
#if !UNITY_EDITOR
            _actionMappingsSerial = null;
#endif
        }

        private void Start()
        {
            _primaryAbility = _actionMappings
                .First(mapping => mapping.Position == AbilityPosition.Primary)
                .Ability;
            _secondaryAbility = _actionMappings
                .First(mapping => mapping.Position == AbilityPosition.Secondary)
                .Ability;
        }

        private void OnEnable() => _inputObservers = ObserveInputs();

        private void OnDisable()
        {
            foreach (var observer in _inputObservers)
            {
                observer.Dispose();
            }
            _inputObservers = null;
        }

        private void CheckMappings()
        {
            if (actionMappingsSerial == null || actionMappingsSerial.Count == 0)
            {
                Debug.LogWarning($"No action mappings found");
                return;
            }
            if (actionMappingsSerial.Any(mapping => mapping.ability is not IInHandAbility))
            {
                Debug.LogError($"Not all action mappings are [IInHandAbility]ies");
            }
        }

        private IEnumerable<IDisposable> ObserveInputs()
        {
            var map = _playerInput.currentActionMap;

            var abilities = _actionMappings
                .Select(mapping => map.ConsumeAction(mapping.ActionName).OnPerformed(() => OnPerformed(mapping)));

            return abilities
                .Append(map.ConsumeAction("Primary").OnPerformed(() => _primaryAbility.Invoke()))
                .Append(map.ConsumeAction("Secondary").OnPerformed(() => _secondaryAbility.Invoke()))
                .ToArray();

            void OnPerformed(ActionAbilityMapping mapping)
            {
                switch (mapping.Position)
                {
                    case AbilityPosition.Primary:
                        _primaryAbility = mapping.Ability; break;
                    case AbilityPosition.Secondary:
                        _secondaryAbility = mapping.Ability; break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mapping.Position), mapping.Position, null);
                }

                mapping.Ability.Invoke();
            }
        }
    }

    public enum AbilityPosition { Primary, Secondary }

    [Serializable]
    public class ActionAbilityMappingSerial
    {
        public string actionName;
        public AbilityPosition position;
        public MonoBehaviour ability;
    }

    internal record ActionAbilityMapping(string ActionName, AbilityPosition Position, IInHandAbility Ability);

    public interface IInHandAbility
    {
        public void Invoke();
    }
}