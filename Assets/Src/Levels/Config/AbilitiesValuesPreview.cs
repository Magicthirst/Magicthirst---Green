using UnityEngine;
using Vertx.Debugging;

namespace Levels.Config
{
    public class AbilitiesValuesPreview : MonoBehaviour
    {
        [SerializeField] private AbilitiesConfig config;
        [SerializeField] private bool drawPush;
        [SerializeField] private bool drawShoot;

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
            var start = transform.position + transform.forward * config.shootOffset;
            var end = start + transform.forward * config.shootDistance;
            D.raw(new Shape.Capsule(start, end, config.shootCircleRadius), Color.red);
        }

        private void DrawPush()
        {
            var center = transform.position + transform.forward * config.pushCircleCenterOffset;

            D.raw(new Shape.Sphere(center, config.pushCircleRadius), Color.blue);
            Debug.DrawLine(
                center,
                center + transform.forward * config.pushVelocity,
                Color.red
            );
            Debug.DrawLine(
                center,
                center + transform.forward * (config.pushVelocity * config.pushDurationSeconds),
                Color.green
            );
        }
    }
}