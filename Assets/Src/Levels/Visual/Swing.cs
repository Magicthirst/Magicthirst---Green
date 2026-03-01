using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    public class Swing : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private float durationSeconds;
        [SerializeField] private float coveredAngle;
        
        [Inject] private IImpactConsumer<CasterSwingedEffect> _consumer;

        private float _startYawAngle;
        private float _endYawAngle;
        private float _pitch;
        private float _progress;

        private void Awake()
        {
            _pitch = pivot.localEulerAngles.x;
            _startYawAngle = 360f - coveredAngle / 2;
            _endYawAngle = 0f + coveredAngle / 2;
            _progress = durationSeconds + 1f; // disabled value
            trail.emitting = false;
        }

        private void OnEnable()
        {
            _consumer.Impacted += HandleSwing;
        }

        private void Update()
        {
            if (_progress > durationSeconds && trail.emitting)
            {
                if (trail.emitting)
                {
                    trail.emitting = false;
                }
                return;
            }

            UpdateRotation();
            _progress += Time.deltaTime;
        }

        private void UpdateRotation()
        {
            var angle = Mathf.LerpAngle(_startYawAngle, _endYawAngle, _progress / durationSeconds);
            pivot.localEulerAngles = new Vector3(_pitch, 0f, angle);
        }

        private void HandleSwing(CasterSwingedEffect effect)
        {
            _progress = 0f;
            UpdateRotation();
            trail.Clear();
            trail.emitting = true;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleSwing;
        }
    }
}