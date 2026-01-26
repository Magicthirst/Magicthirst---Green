using UnityEngine;

namespace Levels.Visual
{
    public class FacesConstant : MonoBehaviour
    {
        [SerializeField] private Vector3 rotation;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.rotation = Quaternion.Euler(rotation);
        }
    }
}