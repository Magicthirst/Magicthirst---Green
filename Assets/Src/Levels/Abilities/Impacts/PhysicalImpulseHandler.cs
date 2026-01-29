using System;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.Impacts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalImpulseHandler : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        [Inject] private IImpactConsumer<ImpulseImpact> _consumer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _consumer.Impacted += HandleImpulse;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleImpulse;
            _consumer.Dispose();
        }

        private void HandleImpulse(ImpulseImpact impulse)
        {
            var t = (float) impulse.Duration.TotalSeconds;
            var scale = t == 0 ? 1f : Mathf.Clamp01(t);
            _rigidbody.linearVelocity += impulse.Velocity * scale;
        }
    }
}