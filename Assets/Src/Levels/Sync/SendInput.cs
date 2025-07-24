using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    [RequireComponent(typeof(ApplyInput))]
    public class SendInput : SyncBehavior
    {
        [Inject] private SendMovement _sendMovement = null!;

        private ApplyInput _input = null!;

        public delegate void SendMovement(Vector2 movement);

        [Inject]
        public void AssertProperConnectionRole(IsPublishingInput isPublishingInput)
        {
            if (!isPublishingInput())
            {
                Destroy(this);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _input = GetComponent<ApplyInput>();
        }

        private void OnEnable() => _input.Moved += SendMovementIfChanged;

        private void OnDisable()
        {
            if (_input != null)
            {
                _input.Moved -= SendMovementIfChanged;
            }
        }

        private void SendMovementIfChanged(Vector2 movement) => _sendMovement(movement);
    }
}
