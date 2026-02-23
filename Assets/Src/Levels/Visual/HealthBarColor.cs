using Levels.Core;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(Renderer))]
    public class HealthBarColor : MonoBehaviour
    {
        [SerializeField] private Color fullHealthColor;
        [SerializeField] private Color halfHealthColor;
        [SerializeField] private Color noHealthColor;

        [Inject] private Health _health;

        private Material _barMaterial;

        private void Awake()
        {
            _barMaterial = GetComponent<Renderer>().material;
        }

        private void OnEnable()
        {
            _health.HealthChangedRelative += UpdateColor;
        }

        private void OnDisable()
        {
            _health.HealthChangedRelative -= UpdateColor;
        }

        private void UpdateColor(float healthRelative)
        {
            if (healthRelative <= 0.5f)
            {
                var zeroToHalf = healthRelative * 2;
                _barMaterial.color = Color.Lerp(noHealthColor, halfHealthColor, zeroToHalf);
            }
            else // 0.5f < healthRelative <= 1f
            {
                var halfToFull = healthRelative * 2 - 1;
                _barMaterial.color = Color.Lerp(halfHealthColor, fullHealthColor, halfToFull);
            }
        }
    }
}