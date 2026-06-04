using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.Teleporter
{
    public class TeleportArea : MonoBehaviour
    {
        [SerializeField] private Transform exitAnchor;

        [Inject] private PublishIntent<ImpactIntent> _teleport;

        private void OnTriggerEnter(Collider other)
        {
            _teleport(ImpactIntent.SelfCast(new TeleportImpact(other.gameObject, exitAnchor.position)));
        }
    }
}