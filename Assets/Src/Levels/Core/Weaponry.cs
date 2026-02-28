using System;
using System.Collections.Generic;
using System.Linq;
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

        public IReadOnlyList<IAbility> Abilities => abilities;
        public IAbility Primary { get; private set; }
        public IAbility Secondary { get; private set; }

        [FormerlySerializedAs("_actionMappings")] // To hide this field being a workaround for dictionary
        [SerializeField]
        private List<Ability> abilities;

        public override void Init()
        {
            Primary = abilities.First(ability => ability.Position == AbilityPosition.Primary);
            Secondary = abilities.First(ability => ability.Position == AbilityPosition.Secondary);
            foreach (var ability in abilities)
            {
                ability.Invoked += () => InvokeAbility(ability);
            }
        }

        private void InvokeAbility(Ability ability)
        {
            if (ability.LastUse > Time.time - ability.Cooldown)
            {
                return;
            }

            ability.LastUse = Time.time;
            Appoint(ability);
            Invoked?.Invoke(ability);
        }

        private void Appoint(Ability ability)
        {
            switch (ability.Position)
            {
                case AbilityPosition.Primary:
                    Primary = ability;
                    break;
                case AbilityPosition.Secondary:
                    Secondary = ability;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
        public event Action Invoked;

        public string InputActionName => action.action.name;
        public AbilityPosition Position => position;
        public float Cooldown => cooldown;

        public float LastUse { get; set; } = float.MinValue;

        [SerializeField] private InputActionReference action;
        [SerializeField] private AbilityPosition position;
        [SerializeField] private float cooldown;

        [SubtypeProperty(typeof(IInHandAbility))]
        [SerializeField] private string abilityType;

        public IInHandAbility FindIn(GameObject gameObject)
        {
            return (IInHandAbility)gameObject.GetComponent(Type.GetType(abilityType));
        }

        public void Invoke() => Invoked?.Invoke();

        public void Dispose() => Invoked = null;
    }

    public interface IAbility
    {
        public string InputActionName { get; }
        public float Cooldown { get; }

        IInHandAbility FindIn(GameObject gameObject);

        void Invoke();
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
}