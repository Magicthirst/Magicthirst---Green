using System;
using System.Collections;
using Levels.Extensions;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.Impacts
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterImpulseHandler : MonoBehaviour
    {
        private CharacterController _controller;

        [Inject] private IImpactConsumer<ImpulseImpact> _consumer;

        private Vector3 _velocity = Vector3.zero;

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

        private void HandleImpulse(ImpulseImpact impulse) => StartCoroutine(ApplyImpulseRoutine(impulse));

        private IEnumerator ApplyImpulseRoutine(ImpulseImpact impulse)
        {
            _velocity += impulse.Velocity;

            yield return new WaitForSeconds((float) impulse.Duration.TotalSeconds);

            _velocity -= impulse.Velocity;
            if (_velocity.IsNearlyZero())
            {
                _velocity = Vector3.zero;
            }
        }
    }
}