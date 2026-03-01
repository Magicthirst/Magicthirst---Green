using UnityEngine;

namespace Levels.Visual
{
    public class Pulsating : MonoBehaviour
    {
        [SerializeField] private AnimationCurve size;

        private Vector3 _baseScale;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            transform.localScale = _baseScale * size.Evaluate(Time.time);
        }
    }
}