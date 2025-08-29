using System.Timers;
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
        private Timer _timer = null;

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
            if (_velocity == Vector3.zero)
            {
                return;
            }

            // TODO: Tween using DOTween plugin
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleImpulse;
        }

        private void HandleImpulse(ImpulseImpact impulse)
        {
            _timer?.Dispose();

            _velocity = impulse.Velocity;
            _timer = new Timer(impulse.Duration.TotalMilliseconds);
            _timer.Start();
            _timer.Elapsed += (_, _) => _velocity = Vector3.zero;
        }
    }
}