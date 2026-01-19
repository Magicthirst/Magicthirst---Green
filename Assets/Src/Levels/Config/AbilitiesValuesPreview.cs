using UnityEngine;
using Vertx.Debugging;

namespace Levels.Config
{
    public class AbilitiesValuesPreview : MonoBehaviour
    {
        [SerializeField] private AbilitiesConfig config;

        private void OnDrawGizmos()
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