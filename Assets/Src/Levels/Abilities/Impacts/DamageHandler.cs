using System;
using System.Collections;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.Impacts
{
    // Stub
    public class DamageHandler : MonoBehaviour
    {
        private IImpactConsumer<DamageImpact> _consumer;

        [Inject]
        public void Construct(Func<GameObject, IImpactConsumer<DamageImpact>> subscribeOnImpulses)
        {
            _consumer = subscribeOnImpulses(gameObject);
        }

        private void OnEnable()
        {
            _consumer.Impacted += HandleDamage;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= HandleDamage;
            _consumer.Dispose();
        }

        private void HandleDamage(DamageImpact damage) => StartCoroutine(StubDie());

        private IEnumerator StubDie()
        {
            yield return new WaitForFixedUpdate();
            Destroy(gameObject);
        }
    }
}