using Levels.Abilities.CommonImpacts;
using Levels.Abilities.HitScanShoot;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.BadProjectilesParry
{
    public class BadlyParriedProjectilesRedirector : MonoBehaviour
    {
        [Header("Cone")]
        [SerializeField] private float horizontalAngle;
        [SerializeField] private float verticalAngle;

        private Transform _camera;

        [Inject] private PublishIntent<HitScanShootIntent> _publish;
        [Inject] private IImpactConsumer<BadlyParriedProjectileImpact> _consumer;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void OnEnable()
        {
            _consumer.Impacted += HandleRedirect;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleRedirect;
        }

        private void HandleRedirect(BadlyParriedProjectileImpact impact)
        {
            var original = impact.Original;

            var yaw = Random.Range(-horizontalAngle, horizontalAngle);
            var pitch = Random.Range(-verticalAngle, verticalAngle);
            var rotation = Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, _camera.right);

            var direction = (rotation * _camera.forward).normalized;

            var config = original.Config;
            var redirected = new HitScanShootIntent
            (
                Caster: gameObject,
                Origin: transform.position,
                Direction: direction,
                Config: config.WithContext(config.Context | ImpactContext.ResultOfBadParry)
            );

            _publish(redirected);
        }
    }
}