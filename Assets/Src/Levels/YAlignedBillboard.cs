using Levels.Extensions;
using UnityEngine;

namespace Levels
{
    public class YAlignedBillboard : MonoBehaviour
    {
        [SerializeField] private new Transform camera;

        private void Update()
        {
            transform.LookAt(camera.transform.position.With(y: transform.position.y));
        }
    }
}
