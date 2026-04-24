using UnityEngine;
using VContainer;

namespace Levels.Util.MasksRegistry
{
    public class Masked : MonoBehaviour
    {
        [SerializeField] private Mask mask;

        [Inject] private MasksRegistry _registry;

        private void Start() => _registry.Register(gameObject, mask);
    }
}