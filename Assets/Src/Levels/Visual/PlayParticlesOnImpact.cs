using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(ParticleSystem))]
    public abstract class PlayParticlesOnImpact<T> : MonoBehaviour where T : IImpact
    {
        private ParticleSystem _particleSystem;

        [Inject] private IImpactConsumer<T> _consumer;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            _consumer.Impacted += Handle;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= Handle;
            _consumer.Dispose();
        }

        protected virtual void Handle(T impact) => _particleSystem.Play();
    }
}