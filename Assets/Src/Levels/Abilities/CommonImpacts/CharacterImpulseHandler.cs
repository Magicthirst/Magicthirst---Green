using System.Collections;
using Levels.Extensions;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.CommonImpacts
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterImpulseHandler : MonoBehaviour
    {
        private CharacterController _controller;

        [Inject] private IImpactConsumer<ImpulseImpact> _consumer;

        private Vector3 _velocity = Vector3.zero;

        private readonly WaitForFixedUpdate _waitFixedUpdate = new();

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _consumer.Impacted += HandleImpulse;
        }

        private void FixedUpdate()
        {
            if (_velocity.IsNearlyZero())
            {
                return;
            }

            _controller.Move(_velocity * Time.fixedDeltaTime);
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleImpulse;
            _consumer.Dispose();
            StopAllCoroutines();
            _velocity = Vector3.zero;
        }

        private void HandleImpulse(ImpulseImpact impulse)
        {
            if (!gameObject.TryInterrupt<IMovementReason>(ApplyImpulseRoutine(impulse)))
            {
                StartCoroutine(ApplyImpulseRoutine(impulse));
            }
        }

        private IEnumerator ApplyImpulseRoutine(ImpulseImpact impulse)
        {
            var left = (float) impulse.Duration.TotalSeconds;
            while (left >= 0)
            {
                yield return _waitFixedUpdate;
                var dt = Time.fixedDeltaTime;
                _controller.Move(impulse.Velocity * dt);
                left -= dt;
            }
        }
    }
}