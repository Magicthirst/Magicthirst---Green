using Levels.Abilities.CommonImpacts;
using Levels.Core;
using Levels.Extensions;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;
using static Levels.Directorship.LevelDirector;

namespace Levels.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FleshUnitSpriteMaterialController : MonoBehaviour
    {
        private static readonly int Seed = Shader.PropertyToID("_Seed");
        private static readonly int Health = Shader.PropertyToID("_Health");
        private static readonly int LastHealTime = Shader.PropertyToID("_SpellLastHealTime");

        [SerializeField] private FleshUnitSpriteMaterialSeeds seeds;

        private SpriteRenderer _renderer;
        private MaterialPropertyBlock _properties;
        private IImpactConsumer _heals;

        [Inject] private Health _health;

        [Inject]
        private void InjectHeals(IImpactConsumer<HealImpact> heals) => _heals = heals;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _properties = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_properties);
        }

        private void OnEnable()
        {
            _health.HealthChangedRelative += OnHealthChanged;
            _heals.Impacted += OnHealed;

            OnHealthChanged(_health.Value);
        }

        private void Start()
        {
            _renderer.UpdatePropertyBlock(_properties, b => b.SetFloat(Seed, seeds.Get()));
        }

        private void OnHealthChanged(float health)
        {
            _renderer.UpdatePropertyBlock(_properties, b => b.SetFloat(Health, health));
        }

        private void OnHealed()
        {
            _renderer.UpdatePropertyBlock(_properties, b => b.SetFloat(LastHealTime, GameplayTime));
        }

        private void OnDisable()
        {
            _health.HealthChangedRelative -= OnHealthChanged;
            _heals.Impacted -= OnHealed;
        }
    }
}