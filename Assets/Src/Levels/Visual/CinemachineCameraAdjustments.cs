using Levels.Extensions;
using Unity.Cinemachine;
using UnityEngine;

namespace Levels.Visual
{
    public class CinemachineCameraAdjustments : MonoBehaviour
    {
        [Header("Orbital Follow\n" +
                "radius adjustments for collisions")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float maxDistance;
        [SerializeField] private float cameraRadius;
        [SerializeField] private AnimationCurve distanceRestorationSpeed;

        [Header("Rotation Composer\n" +
                "Target offset adjustments for radius")]
        [SerializeField] private AnimationCurve heightForRadius;

        private CinemachineCamera _camera;
        private CinemachineOrbitalFollow _orbit;
        private CinemachineRotationComposer _rotation;
        private Transform _target;

        private void OnValidate()
        {
            if (GetComponent<CinemachineCamera>() == null ||
                GetComponent<CinemachineOrbitalFollow>() == null ||
                GetComponent<CinemachineRotationComposer>() == null) // RequireComponent will create new component, I don't want that
            {
                throw new MissingComponentException($"There is no {typeof(CinemachineCamera)} in here");
            }
        }

        private void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();
            _orbit = GetComponent<CinemachineOrbitalFollow>();
            _rotation = GetComponent<CinemachineRotationComposer>();
            _target = _camera.Target.TrackingTarget;
        }

        private void LateUpdate()
        {
            var directionToCamera = (_camera.transform.position - _target.position).normalized;

            var isOccluded = Physics.Raycast(_target.position, directionToCamera, out var hit, maxDistance, layerMask);
            var goalDistance = isOccluded ? hit.distance - cameraRadius : maxDistance;

            var diff = _orbit.Radius - goalDistance;
            var delta = distanceRestorationSpeed.Evaluate(diff) * Time.deltaTime;
            _orbit.Radius += delta;

            if (_orbit.Radius >= goalDistance)
            {
                _orbit.Radius = goalDistance;
            }

            _rotation.TargetOffset = _rotation.TargetOffset.With(y: heightForRadius.Evaluate(_orbit.Radius));
        }
    }
}