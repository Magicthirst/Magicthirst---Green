using BlackMassSoftware.FloatingTextEngine.Lite;
using BlackMassSoftware.FloatingTextEngine.Lite.Behaviors;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    public class DisplayDamage : MonoBehaviour
    {
        [Header("Appearance")]
        [SerializeField] private Transform startAnchor;
        [SerializeField] private Transform endAnchor;
        [SerializeField] private float jumpInDuration;

        [Header("Fade-out")]
        [SerializeField] private float fadeOutDuration;

        private float _jumpDistance;

        [Inject] private IImpactConsumer<DamageImpact> _damages;

        private void Awake()
        {
            _jumpDistance = endAnchor.position.y - startAnchor.position.y;
        }

        private void OnEnable() => _damages.Impacted += PopupDamage;

        private void PopupDamage(DamageImpact impact)
        {
            FloatingTextEngine
                .CreateFloatingTextAt(startAnchor.position, impact.Damage, Color.red)
                .With(new MoveUpBehavior(_jumpDistance, jumpInDuration))
                .With(new PopBehavior(2f, jumpInDuration, fadeOutDuration))
                .With(new FadeOutBehavior(fadeOutDuration).DelayFor(jumpInDuration));
        }

        private void OnDisable() => _damages.Impacted -= PopupDamage;
    }
}