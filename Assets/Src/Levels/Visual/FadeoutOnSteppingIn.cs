using System.Collections;
using JetBrains.Annotations;
using Levels.Directorship;
using Levels.Extensions;
using Levels.Util;
using Levels.Util.MasksRegistry;
using UnityEngine;
using VContainer;
using static UnityEngine.ParticleSystem;

namespace Levels.Visual
{
    public class FadeoutOnSteppingIn : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [SerializeField] private AnimationCurve lifetimeFadeout;

        private ParticleSystem _particleSystem;
        [CanBeNull] private Coroutine _fadeoutRoutine = null;

        [Inject] private MasksRegistry _registry;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var isPlayer = _registry.Is(other.gameObject, Mask.PlayerCharacter);
            if (isPlayer && _fadeoutRoutine is null)
            {
                _fadeoutRoutine = StartCoroutine(Fadeout().WithInterruptions(_LevelLifecycle));
            }
        }

        private IEnumerator Fadeout()
        {
            var alpha = 1f;
            var particles = new Particle[_particleSystem.particleCount];

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            var particlesCount = _particleSystem.GetParticles(particles);
            if (particlesCount == 0)
            {
                yield break;
            }

            var baseAlpha = particles[0].startColor.a / 256f;

            for (float t = 0; alpha >= 0f; t += LevelDirector.GameplayDeltaTime)
            {
                
                alpha = lifetimeFadeout.Evaluate(t) - 0.1f;

                for (var i = 0; i < particles.Length; i++)
                {
                    particles[i].startColor = particles[i].startColor.WithA(baseAlpha * alpha);
                }
                _particleSystem.SetParticles(particles, particlesCount);

                yield return null;
                particlesCount = _particleSystem.GetParticles(particles);
            }

            gameObject.SetActive(false);
        }
    }
}