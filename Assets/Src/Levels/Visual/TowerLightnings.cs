using System.Collections;
using Levels.Extensions;
using UnityEngine;

namespace Levels.Visual
{
    public class TowerLightnings : MonoBehaviour
    {
        [Header("Spawn Areas")]
        [SerializeField] private BoxCollider fromArea;
        [SerializeField] private BoxCollider toArea;

        [Header("Lightning")]
        [SerializeField]
        [ColorUsage(true, true)]
        private Color[] colors;
        [SerializeField] private LineRenderer lightningPrefab;
        [SerializeField] private float lifetime = 0.08f;

        [Header("Timing")]
        [SerializeField] private Vector2 interval = new(0.5f, 3f);

        [Header("Jitter")]
        [SerializeField] private int segments = 10;
        [SerializeField] private float deviation = 1f;

        private void Start()
        {
            StartCoroutine(LightningRoutine());
        }

        private IEnumerator LightningRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(interval.x, interval.y));

                yield return SpawnLightning(RandomPoint(fromArea), RandomPoint(toArea));
            }
        }

        private IEnumerator SpawnLightning(Vector3 from, Vector3 to)
        {
            var line = Instantiate(lightningPrefab);

            if (Physics.Raycast(from, (to - from).normalized, out var hit))
            {
                line.transform.position = hit.point;
            }
            else
            {
                Debug.LogWarning("Lightning somehow missed, investigate");
            }

            SetColor(colors[Random.Range(0, colors.Length)]);
            line.positionCount = segments + 1;

            for (var i = 0; i <= segments; i++)
            {
                var t = i / (float)segments;

                var p = Vector3.Lerp(from, to, t);

                if (i != 0 && i != segments)
                {
                    p += Random.insideUnitSphere * deviation;
                }

                line.SetPosition(i, p);
            }

            yield return null;

            Destroy(line.gameObject, lifetime);
            yield break;

            void SetColor(Color color)
            {
                line.startColor = line.endColor = color;
                foreach (var sprite in line.GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.color = color.With(a: sprite.color.a);
                }
            }
        }

        private static Vector3 RandomPoint(BoxCollider box)
        {
            var c = box.center;
            var s = box.size * 0.5f;

            return box.transform.TransformPoint(new(Random.Range(-s.x, s.x) + c.x, Random.Range(-s.y, s.y) + c.y, Random.Range(-s.z, s.z) + c.z));
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            DrawBox(fromArea, Color.yellow);
            DrawBox(toArea, Color.cyan);
        }

        private static void DrawBox(BoxCollider box, Color color)
        {
            if (!box)
            {
                return;
            }

            Gizmos.color = color;
            Gizmos.matrix = box.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }
#endif
    }
}