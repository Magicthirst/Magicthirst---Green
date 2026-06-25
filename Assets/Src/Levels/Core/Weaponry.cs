using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Directorship;
using Levels.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Util;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "Weaponry", menuName = "Core/Components/Weaponry", order = 1)]
    [Serializable]
    public class Weaponry : CoreObject
    {
        public event Action<IAbility> Invoked;
        public event Action<IAbility> Equipped;
        public event Action AvailableAbilitiesChanged;

        public IReadOnlyCollection<IAbility> Abilities => abilities.Where(a => _enabledAbilities.Contains(a.Type)).ToList();

        public IReadOnlyCollection<IAbility> AllAbilities => abilities;

        public IPropertyHandle<IAbility> Primary => _primary;
        public IPropertyHandle<IAbility> Secondary => _secondary;

        private PropertyHandle<Ability> _primary;
        private PropertyHandle<Ability> _secondary;

        private readonly HashSet<Type> _enabledAbilities = new();

        [FormerlySerializedAs("_actionMappings")]
        [SerializeField]
        private List<Ability> abilities;

        public override void Init()
        {
            foreach (var ability in abilities)
            {
                _enabledAbilities.Add(ability.Type);
                ability.Equipped += () => Equip(ability);
            }

            _primary ??= new PropertyHandle<Ability>
            {
                Value = abilities.First(a => a.Position == AbilityPosition.Primary)
            };

            _secondary ??= new PropertyHandle<Ability>
            {
                Value = abilities.First(a => a.Position == AbilityPosition.Secondary)
            };
        }

        public void SetAvailableAbilities(IEnumerable<Type> types)
        {
            _enabledAbilities.Clear();

            foreach (var type in types)
            {
                _enabledAbilities.Add(type);
            }

            ValidateSelectedAbilities();

            AvailableAbilitiesChanged?.Invoke();
        }

        private void ValidateSelectedAbilities()
        {
            if (!_enabledAbilities.Contains(_primary.Value.Type))
            {
                var replacement = abilities.FirstOrDefault(a =>
                    a.Position == AbilityPosition.Primary &&
                    _enabledAbilities.Contains(a.Type));

                if (replacement != null)
                {
                    _primary.Value = replacement;
                    Equipped?.Invoke(replacement);
                }
            }

            if (!_enabledAbilities.Contains(_secondary.Value.Type))
            {
                var replacement = abilities.FirstOrDefault(a =>
                    a.Position == AbilityPosition.Secondary &&
                    _enabledAbilities.Contains(a.Type));

                if (replacement != null)
                {
                    _secondary.Value = replacement;
                    Equipped?.Invoke(replacement);
                }
            }
        }

        public void InvokePrimary()
        {
            Use(_primary.Value);
        }

        public void InvokeSecondary()
        {
            Use(_secondary.Value);
        }

        private void Equip(Ability ability)
        {
            if (!_enabledAbilities.Contains(ability.Type))
            {
                return;
            }

            switch (ability.Position)
            {
                case AbilityPosition.Primary:
                    _primary.Value = ability;
                    break;

                case AbilityPosition.Secondary:
                    _secondary.Value = ability;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Equipped?.Invoke(ability);

            if (ability.InvokeOnEquip)
            {
                Use(ability);
            }
        }

        private void Use(Ability ability)
        {
            if (ability == null)
            {
                return;
            }

            if (!_enabledAbilities.Contains(ability.Type))
            {
                return;
            }

            if (ability.LastUse <= LevelDirector.GameplayTime - ability.Cooldown)
            {
                ability.LastUse = LevelDirector.GameplayTime;
                Invoked?.Invoke(ability);
            }
        }

        public override void Dispose()
        {
            foreach (var ability in abilities)
            {
                ability.Dispose();
            }

            _enabledAbilities.Clear();
            AvailableAbilitiesChanged = null;
        }
    }

    [Serializable]
    public class Ability : IAbility, IDisposable
    {
        public event Action Equipped;

        public string InputActionName => action.action.name;
        public string KeyName => action.action.GetBindingDisplayString();
        public AbilityPosition Position => position;
        public float CooldownProgress => Mathf.InverseLerp(LastUse, LastUse + cooldown, Time.time);
        public bool InvokeOnEquip => invokeOnEquip;
        public Type Type => _type ??= Type.GetType(abilityType);

        public float Cooldown => cooldown;

        public float LastUse { get; set; } = 0f;

        private Type _type = null;

        [SerializeField] private InputActionReference action;
        [SerializeField] private AbilityPosition position;
        [SerializeField] private float cooldown;
        [SerializeField] private bool invokeOnEquip;

        [SubtypeProperty(typeof(IInHandAbility))]
        [SerializeField] private string abilityType;

        public IInHandAbility FindIn(GameObject gameObject)
        {
            return (IInHandAbility)gameObject.GetComponent(Type);
        }

        public void Equip() => Equipped?.Invoke();

        public void Dispose() => Equipped = null;
    }

    public interface IAbility
    {
        public string InputActionName { get; }
        public string KeyName { get; }
        public AbilityPosition Position { get; }
        public float CooldownProgress { get; }
        public Type Type { get; }

        IInHandAbility FindIn(GameObject gameObject);

        void Equip();
    }

    public enum AbilityPosition
    {
        Primary,
        Secondary
    }

    public interface IInHandAbility
    {
        public void Invoke();
    }

    public interface ISpell {}
}