using System;
using Levels.Core;
using UnityEngine;
using VContainer;
using static Levels.Core.TeleportChipState;

namespace Levels.UI.Weaponry
{
    public class TeleportationChipAnimation : MonoBehaviour
    {
        [SerializeField] private Transform chip;
        [SerializeField] private Transform vortex;
        private TrailRenderer[] _trails;

        [SerializeField] private Vector3 chipBaseRotation;
        [SerializeField] private Vector3 vortexBaseRotation;

        [SerializeField] private float trailNotActiveFadeTime;
        [SerializeField] private float trailActiveFadeTime;

        [SerializeField] private Vector3 activeChipRotationPerSecond;
        [SerializeField] private Vector3 activeVortexRotationPerSecond;

        [SerializeField] private Vector3 notActiveVortexRotationPerSecond;

        [Inject] private TeleportChip _teleportChip;

        private void Awake()
        {
            _trails = vortex.GetComponentsInChildren<TrailRenderer>();
        }

        private void OnEnable()
        {
            _teleportChip.StateChanged += HandleStateChanged;
            HandleStateChanged(_teleportChip.State);
        }

        private void Update()
        {
            switch (_teleportChip.State)
            {
                case Ready:
                    vortex.eulerAngles += notActiveVortexRotationPerSecond * Time.deltaTime;
                    break;
                case Thrown:
                case OnGround:
                    chip.eulerAngles += activeChipRotationPerSecond * Time.deltaTime;
                    vortex.eulerAngles += activeVortexRotationPerSecond * Time.deltaTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleStateChanged(TeleportChipState state)
        {
            switch (state)
            {
                case Ready:
                    chip.eulerAngles = chipBaseRotation;
                    vortex.eulerAngles = vortexBaseRotation;
                    foreach (var trail in _trails)
                    {
                        trail.time = trailNotActiveFadeTime;
                    }
                    break;
                case Thrown:
                    chip.eulerAngles = chipBaseRotation;
                    vortex.eulerAngles = vortexBaseRotation;
                    foreach (var trail in _trails)
                    {
                        trail.time = trailActiveFadeTime;
                    }
                    break;
                case OnGround:
                    chip.eulerAngles = chipBaseRotation;
                    vortex.eulerAngles = vortexBaseRotation;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDisable()
        {
            _teleportChip.StateChanged -= HandleStateChanged;
        }
    }
}