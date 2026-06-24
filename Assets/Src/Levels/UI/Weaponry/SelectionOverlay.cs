using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Levels.UI.Weaponry
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Renderer))]
    public class SelectionOverlay : MonoBehaviour
    {
        [SerializeField] private AnimationCurve moveCurve;

        private RectTransform _transform;

        [CanBeNull] private RectTransform _abilityToOverlay = null;
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
            if (_abilityToOverlay != null)
            {
                _startPosition = _transform.anchoredPosition;
                _desiredPosition = _abilityToOverlay.anchoredPosition;
                _startTime = Time.time;
                EnableRender();
                _abilityToOverlay = null;
            }
            else if (_abilityToOverlay is not null)
            {
                _abilityToOverlay = null;
            }

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
            _abilityToOverlay = ability;

            if (ability is null)
            {
                DisableRender();
            }
        }

        // TODO separate visual logic from moving logic
        private void EnableRender()
        {
            foreach (Transform child in _transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        // TODO separate visual logic from moving logic
        private void DisableRender()
        {
            foreach (Transform child in _transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}