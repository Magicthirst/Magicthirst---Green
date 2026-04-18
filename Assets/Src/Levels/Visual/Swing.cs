using System;
using System.Collections.Generic;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    public class Swing : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private float durationSeconds;
        [SerializeField] private float coveredAngle;

        [SerializeField] private SwingDirection[] directionsLoop = { SwingDirection.RightToLeft };
        
        [Inject] private IImpactConsumer<CasterSwingedEffect> _consumer;

        private float _startYawAngle;
        private float _endYawAngle;
        private float _pitch;
        private float _progress;

        private IEnumerator<SwingDirection> _directionsQueue;

        private void Awake()
        {
            _pitch = pivot.localEulerAngles.x;
            
            _progress = durationSeconds + 1f; // disabled value
            trail.emitting = false;
            _directionsQueue = directionsLoop.InfinitelyLooping();
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
            InitAngles(_directionsQueue.Dequeue());

            _progress = 0f;
            UpdateRotation();
            trail.Clear();
            trail.emitting = true;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleSwing;
        }

        private void InitAngles(SwingDirection direction)
        {
            switch (direction)
            {
                case SwingDirection.LeftToRight:
                    _startYawAngle = 0f + coveredAngle / 2;
                    _endYawAngle = 360f - coveredAngle / 2;
                    break;
                case SwingDirection.RightToLeft:
                    _startYawAngle = 360f - coveredAngle / 2;
                    _endYawAngle = 0f + coveredAngle / 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }

    public enum SwingDirection
    {
        LeftToRight,
        RightToLeft
    }
}