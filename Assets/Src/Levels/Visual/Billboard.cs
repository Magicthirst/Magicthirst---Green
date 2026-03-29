using Levels.Extensions;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    public class Billboard : MonoBehaviour
    {
        protected static Transform Camera;

        [SerializeField] private bool flipped = false;

        [Inject]
        public void Construct(Camera injectedCamera) => Camera = injectedCamera.transform;

        protected virtual void Update()
        {
            if (flipped)
            {
                transform.LookAway(from: Camera);
            }
            else
            {
                transform.LookAt(Camera);
            }
        }
    }
}