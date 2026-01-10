using System.Collections;
using Levels.Extensions;
using Levels.IntentsImpacts;
using Levels.IntentsImpacts.Impacts;
using UnityEngine;
using VContainer;

namespace Levels.ImpactHandlers
{
    [RequireComponent(typeof(CharacterController))]
    public class ImpulseHandler : MonoBehaviour
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
                _velocity = Vector3.zero;
                return;
            }

            _controller.Move(_velocity * Time.fixedDeltaTime);
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleImpulse;
            StopAllCoroutines();
            _velocity = Vector3.zero;
        }

        private void HandleImpulse(ImpulseImpact impulse) => StartCoroutine(ApplyImpulseRoutine(impulse));

        private IEnumerator ApplyImpulseRoutine(ImpulseImpact impulse)
        {
            var velocity3D = impulse.Velocity.ToX0Y();

            _velocity += velocity3D;

            yield return new WaitForSeconds((float) impulse.Duration.TotalSeconds);

            _velocity -= velocity3D;
        }
    }
}