using System;
using Levels.Abilities.Impacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels
{
    public class Health : MonoBehaviour
    {
        public event Action<float> HealthChangedRelative;

        private event Action<int> HealthChanged;

        [SerializeField] private int maxHealth;
        [SerializeField] private int value;

        public int Value
        {
            get => value;
            private set
            {
                this.value = value;
                HealthChanged?.Invoke(value);
            }
        }

        private IImpactConsumer<DamageImpact> _consumer;

        public Health()
        {
            HealthChanged += health => HealthChangedRelative?.Invoke((float) health / maxHealth);
        }

        [Inject]
        public void Construct(Func<GameObject, IImpactConsumer<DamageImpact>> subscribeOnDamages)
        {
            _consumer = subscribeOnDamages(gameObject);
        }

        private void OnEnable()
        {
            _consumer.Impacted += HandleDamage;
        }

#if UNITY_EDITOR
        private void Update() => Value = Mathf.Clamp(Value, 0, maxHealth);
#endif

        private void OnDisable()
        {
            _consumer.Impacted -= HandleDamage;
            _consumer.Dispose();
        }

        private void HandleDamage(DamageImpact damage) => Value = Math.Max(0, Value - damage.Damage);
    }
}