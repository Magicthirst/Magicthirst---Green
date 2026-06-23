using System;
using Levels.Abilities.CommonImpacts;
using Levels.Abilities.KillAndDown;
using Levels.Abilities.Revival;
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

        [Inject] private IImpactConsumer<HealImpact> _healConsumer;
        [Inject] private IImpactConsumer<DamageImpact> _damageConsumer;
        [Inject] private PublishIntent<DownedIntent> _publishKill;
        [Inject] private PublishIntent<ImpactIntent> _publishRecovery;

        public bool IsDown => Value == 0;
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
            _healConsumer.Impacted += HandleHeal;
            _damageConsumer.Impacted += HandleDamage;
        }

        public override void Dispose()
        {
            _damageConsumer.Impacted -= HandleDamage;
            _damageConsumer.Dispose();
            _healConsumer.Impacted -= HandleHeal;
            _healConsumer.Dispose();
        }

        private void HandleDamage(DamageImpact damage)
        {
            Value = Math.Max(0, Value - damage.Damage);
            if (Value == 0)
            {
                _publishKill(new DownedIntent(Caster: damage.Attacker, Victim: Owner));
            }
        }

        private void HandleHeal(HealImpact heal)
        {
            var wasDown = Value == 0;
            Value = Math.Min(MaxHealth, Value + heal.Amount);
            var isNotDown = Value > 0;

            if (wasDown && isNotDown)
            {
                _publishRecovery(ImpactIntent.SelfCast(new RecoveredImpact(Owner)));
            }
        }
    }
}