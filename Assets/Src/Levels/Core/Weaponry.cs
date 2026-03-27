using System;
using System.Collections.Generic;
using System.Linq;
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

        public IReadOnlyList<IAbility> Abilities => abilities;

        public IPropertyHandle<IAbility> Primary => _primary;
        public IPropertyHandle<IAbility> Secondary => _secondary;

        private PropertyHandle<Ability> _primary;
        private PropertyHandle<Ability> _secondary;

        [FormerlySerializedAs("_actionMappings")] // To hide this field being a workaround for dictionary
        [SerializeField]
        private List<Ability> abilities;

        public override void Init()
        {
            _primary ??= new PropertyHandle<Ability>
            {
                Value = abilities.First(ability => ability.Position == AbilityPosition.Primary)
            };
            _secondary ??= new PropertyHandle<Ability>
            {
                Value = abilities.First(ability => ability.Position == AbilityPosition.Secondary)
            };

            foreach (var ability in abilities)
            {
                ability.Equipped += () => Equip(ability);
            }
        }

        public void InvokePrimary() => Use(_primary.Value);

        public void InvokeSecondary() => Use(_secondary.Value);

        private void Equip(Ability ability)
        {
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
            if (ability.LastUse <= Time.time - ability.Cooldown)
            {
                ability.LastUse = Time.time;
                Invoked?.Invoke(ability);
            }
        }

        public override void Dispose()
        {
            foreach (var ability in abilities)
            {
                ability.Dispose();
            }
        }
    }

    [Serializable]
    public class Ability : IAbility, IDisposable
    {
        public event Action Equipped;

        public string InputActionName => action.action.name;
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