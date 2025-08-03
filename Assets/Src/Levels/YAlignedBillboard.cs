using Levels.Extensions;
using UnityEngine;
using VContainer;

namespace Levels
{
    public class YAlignedBillboard : MonoBehaviour
    {
        private Transform _camera;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Update()
        {
            transform.LookAt(_camera.position.With(y: transform.position.y));
        }
    }
}
