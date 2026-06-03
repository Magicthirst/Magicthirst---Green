using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    public class FollowCameraOnAxis : MonoBehaviour
    {
        [SerializeField] private Vector3 axis;
        private Vector3 _constAxes;

        private Transform _transform;
        private Transform _camera;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            axis = axis.normalized;
            _constAxes = Vector3.one - axis;

            _transform = transform;
        }

        private void LateUpdate()
        {
            var constPosition = new Vector3(
                _constAxes.x * _transform.position.x,
                _constAxes.y * _transform.position.y,
                _constAxes.z * _transform.position.z
            );
            var derivativePosition = new Vector3(
                axis.x * _camera.position.x,
                axis.y * _camera.position.y,
                axis.z * _camera.position.z
            );

            _transform.position = constPosition + derivativePosition;
        }
    }
}