using System;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Abilities.TeleportChip
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(Rigidbody))]
    public class TeleportChip : MonoBehaviour
    {
        [SerializeField] private Renderer highlight; 

        private Transform _transform;
        private Rigidbody _rigidbody;
        private Renderer[] _renderers;

        [Inject] private Core.TeleportChip _state;
        private IImpactConsumer<TeleportChipSpawnImpact> _spawnConsumer;

        private float _lostInFlyingTimer;
        private bool _justThrown;

        [Inject]
        private void Construct(IntentsImpacts.IntentsImpacts intentsImpacts)
        {
            _spawnConsumer = intentsImpacts.GetImpactConsumerFor<TeleportChipSpawnImpact>(gameObject);
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();
            _renderers = GetComponentsInChildren<Renderer>();
        }

        private void Start() => StopAndHide();

        private void OnEnable()
        {
            _spawnConsumer.Impacted += HandleSpawn;
            _state.StateChanged += HandleStateChanged;
            _state.Register(this);
        }

        private void FixedUpdate()
        {
            if (!_justThrown)
            {
                return;
            }

            if (DetectGround())
            {
                _justThrown = false;
                _state.Land();
                return;
            }

            if (_lostInFlyingTimer < 0)
            {
                _justThrown = false;
                _state.Restore();
            }
            else
            {
                _lostInFlyingTimer -= Time.fixedDeltaTime;
            }
        }

        private void HandleSpawn(TeleportChipSpawnImpact spawn)
        {
            RunAndShow();
            _transform.SetParent(null);
            _transform.position = spawn.Origin;
            _transform.LookAt(spawn.Origin + spawn.Velocity);
            _rigidbody.AddForce(spawn.Velocity, ForceMode.Impulse);
            _rigidbody.AddTorque(spawn.AngularVelocity, ForceMode.Impulse);
            _lostInFlyingTimer = spawn.Config.FlyingTimeLostThreshold;
            _justThrown = true;
        }

        private void HandleStateChanged(TeleportChipState state)
        {
            switch (state)
            {
                case TeleportChipState.Ready:
                    StopAndHide();
                    highlight.enabled = false;
                    break;
                case TeleportChipState.Thrown:
                    highlight.enabled = false;
                    break;
                case TeleportChipState.OnGround:
                    highlight.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDisable()
        {
            _spawnConsumer.Impacted -= HandleSpawn;
            _state.StateChanged -= HandleStateChanged;
        }

        private void StopAndHide()
        {
            _rigidbody.isKinematic = true;
            foreach (var renderer in _renderers)
            {
                renderer.enabled = false;
            }
        }

        private void RunAndShow()
        {
            _rigidbody.isKinematic = false;
            foreach (var renderer in _renderers)
            {
                renderer.enabled = true;
            }
        }

        private bool DetectGround()
        {
            var ray = new Ray(_transform.position, Vector3.down);
            return Physics.Raycast(ray, 0.1f);
        }
    }
}