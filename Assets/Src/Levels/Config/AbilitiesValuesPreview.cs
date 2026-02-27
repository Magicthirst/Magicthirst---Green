using Levels.Abilities.HitScanShoot;
using Levels.Abilities.ParrySabre;
using Levels.Abilities.PushingShotgun;
using Levels.Abilities.TeleportChip;
using Levels.Core.Passives;
using UnityEngine;
using Vertx.Debugging;

namespace Levels.Config
{
    public class AbilitiesValuesPreview : MonoBehaviour
    {
        [SerializeField] private float throwingAngle;

        [SerializeField] private bool drawShotgun;
        [SerializeField] private bool drawShoot;
        [SerializeField] private bool drawParrySabre;
        [SerializeField] private bool drawTeleportChip;

        [SerializeField] private ShootConfig shootConfig;
        [SerializeField] private ShotgunConfig shotgunConfig;
        [SerializeField] private TeleportChipConfig teleportChipConfig;
        [SerializeField] private ParrySabreConfig parrySabreConfig;

        private void OnDrawGizmos()
        {
            if (drawShoot) DrawShoot();
            if (drawShotgun) DrawShotgun();
            if (drawParrySabre) DrawParrySabre();
            if (drawTeleportChip) DrawTeleportChip();
        }

        private void DrawShoot()
        {
            var shoot = (IShootConfig)shootConfig;

            var start = transform.position + transform.forward * shoot.Offset;
            var end = start + transform.forward * shoot.Distance;
            D.raw(new Ray(start, end), Color.red);
        }

        private void DrawShotgun()
        {
            var shotgun = (IShotgunConfig)shotgunConfig;

            var center = transform.position + transform.forward * shotgun.CircleCenterOffset;

            D.raw(new Shape.Sphere(center, shotgun.CircleRadius), Color.blue);
            Debug.DrawLine(
                center,
                center + transform.forward * shotgun.Velocity,
                Color.red
            );
            Debug.DrawLine(
                center,
                center + transform.forward * (shotgun.Velocity * (float) shotgun.Duration.TotalSeconds),
                Color.green
            );
        }

        private void DrawParrySabre()
        {
            var sabre = (ISabreConfig)parrySabreConfig;
            var parry = (IParryConfig)parrySabreConfig;

            var center = transform.position + transform.forward * sabre.CircleCenterOffset;

            D.raw(new Shape.Sphere(center, sabre.CircleRadius), Color.blue);

            D.raw(new Shape.Cone(
                pointBase: center,
                pointTip: transform.position,
                radiusBase: Mathf.Tan(parry.AngleDegrees * Mathf.Deg2Rad) * sabre.CircleCenterOffset
            ), Color.cyan);
        }

        private void DrawTeleportChip()
        {
            const float timeStep = 0.05f;

            var chip = (ITeleportChipConfig)teleportChipConfig;

            var startPos = transform.position 
                           + transform.up * chip.ThrowOriginVerticalOffset 
                           + transform.forward * chip.ThrowOriginHorizontalOffset;

            var throwDirection = Quaternion.Euler(0, 0, z: throwingAngle) * transform.forward;
            var initialVelocity = throwDirection * chip.ThrowVelocity;

            var previousPoint = startPos;

            D.raw(new Shape.Sphere(startPos, 0.1f), Color.cyan);

            for (var t = timeStep; t <= chip.FlyingTimeLostThreshold; t += timeStep)
            {
                var currentPoint = startPos 
                                   + (initialVelocity * t) 
                                   + (0.5f * Physics.gravity * t * t);

                Debug.DrawLine(previousPoint, currentPoint, Color.blue);
                previousPoint = currentPoint;
            }
        }
    }
}