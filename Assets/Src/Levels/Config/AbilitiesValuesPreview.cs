using Levels.Abilities.HitScanShoot;
using Levels.Abilities.PushingShotgun;
using UnityEngine;
using Vertx.Debugging;

namespace Levels.Config
{
    public class AbilitiesValuesPreview : MonoBehaviour
    {
        [SerializeField] private bool drawPush;
        [SerializeField] private bool drawShoot;

        [SerializeField] private ShootConfig shootConfig;
        [SerializeField] private ShotgunConfig pushConfig;

        private void OnDrawGizmos()
        {

            if (drawPush)
            {
                DrawPush();
            }

            if (drawShoot)
            {
                DrawShoot();
            }
        }

        private void DrawShoot()
        {
            var shoot = (IShootConfig) shootConfig;
            
            var start = transform.position + transform.forward * shoot.Offset;
            var end = start + transform.forward * shoot.Distance;
            D.raw(new Ray(start, end), Color.red);
        }

        private void DrawPush()
        {
            var config = (IShotgunConfig) pushConfig;

            var center = transform.position + transform.forward * config.CircleCenterOffset;

            D.raw(new Shape.Sphere(center, config.CircleRadius), Color.blue);
            Debug.DrawLine(
                center,
                center + transform.forward * config.Velocity,
                Color.red
            );
            Debug.DrawLine(
                center,
                center + transform.forward * (config.Velocity * (float) config.Duration.TotalSeconds),
                Color.green
            );
        }
    }
}