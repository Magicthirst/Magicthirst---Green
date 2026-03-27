using System.Linq;
using UnityEngine;

namespace Levels.UI.Weaponry
{
    [RequireComponent(typeof(RectTransform))]
    public class SelectionOverlay : MonoBehaviour
    {
        [SerializeField] private AnimationCurve moveCurve;

        private RectTransform _transform;

        private Vector2 _desiredPosition;
        private Vector2 _startPosition;
        private float _startTime;
        private float _duration;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _duration = moveCurve.keys.Last().time - moveCurve.keys.First().time;
        }

        private void Update()
        {
            if (Time.time > _startTime + _duration)
            {
                if (_transform.anchoredPosition != _desiredPosition)
                {
                    _transform.anchoredPosition = _desiredPosition;
                }

                return;
            }

            var t = moveCurve.Evaluate(Time.time - _startTime);
            _transform.anchoredPosition = Vector2.Lerp(_startPosition, _desiredPosition, t);
        }

        public void MoveAtop(RectTransform ability)
        {
            _startPosition = _transform.anchoredPosition;
            _desiredPosition = ability.anchoredPosition;
            _startTime = Time.time;
        }
    }
}