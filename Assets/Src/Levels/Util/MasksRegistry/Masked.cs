using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Levels.Util.MasksRegistry
{
    public class Masked : MonoBehaviour
    {
        [SerializeField] private Mask mask;

        private void Start()
        {
            LifetimeScope.Find<LifetimeScope>().Container
                .Resolve<MasksRegistry>()
                .Register(gameObject, mask);
        }
    }
}