using System;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
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

    [RequireComponent(typeof(ParticleSystem))]
    public abstract class PlayParticlesOnImpact : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        [SubtypeProperty(typeof(IImpact))]
        [SerializeField]
        private string impactType;
        private IImpactConsumer _consumer;

        [Inject]
        private void Consumer(IObjectResolver resolver)
        {
            var tImpact = Type.GetType(impactType);
            var consumerType = typeof(IImpactConsumer<>).MakeGenericType(tImpact);
            _consumer = (IImpactConsumer) resolver.Resolve(consumerType);
        }

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

        protected virtual void Handle() => _particleSystem.Play();
    }
}