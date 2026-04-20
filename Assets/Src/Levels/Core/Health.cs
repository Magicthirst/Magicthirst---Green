using System;
using Levels.Abilities.CommonImpacts;
using Levels.Abilities.Kill;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "Health", menuName = "Core/Components/Health", order = 1)]
    [Serializable]
    public class Health : CoreObject
    {
        public event Action<float> HealthChangedRelative;

        public event Action<int> HealthChanged;

        [SerializeField] private int maxHealth;
        [SerializeField] private int value;

        [Inject] private IImpactConsumer<DamageImpact> _consumer;
        [Inject] private PublishIntent<KilledIntent> _publishKill;

        public int MaxHealth => maxHealth;
        public int Value
        {
            get => value;
            private set
            {
                this.value = value;
                HealthChanged?.Invoke(value);
            }
        }

        public Health()
        {
            HealthChanged += health => HealthChangedRelative?.Invoke((float) health / maxHealth);
        }

        public override void Init()
        {
            _consumer.Impacted += HandleDamage;
        }

        public override void Dispose()
        {
            _consumer.Impacted -= HandleDamage;
            _consumer.Dispose();
        }

        private void HandleDamage(DamageImpact damage)
        {
            Value = Math.Max(0, Value - damage.Damage);
            if (Value == 0)
            {
                _publishKill(new KilledIntent(Caster: damage.Attacker, Victim: Owner));
            }
        }
    }
}