using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Util
{
    public class EvacuateGameObjectOnDeath : MonoBehaviour
    {
        [SerializeField] private float lifespan;

        [Inject] private IImpactConsumer<TargetIsDeadImpact> _dead;

        private void OnEnable()
        {
            _dead.Impacted += OnDead;
        }

        private void OnDead(TargetIsDeadImpact _)
        {
            if (lifespan == 0)
            {
                return;
            }

            if (lifespan > 0)
            {
                Destroy(gameObject, lifespan);
            }

            transform.SetParent(transform.root);
            enabled = false;
        }

        private void OnDisable()
        {
            _dead.Impacted -= OnDead;
        }
    }
}