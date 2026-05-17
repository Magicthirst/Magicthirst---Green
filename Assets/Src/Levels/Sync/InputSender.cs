using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    [RequireComponent(typeof(IMovementInputSource))]
    public class InputSender : SyncBehavior
    {
        [Inject] private IsPublishingInput _isPublishingInput;
        [Inject] private SendMovement _sendMovement;
        [Inject] private IMovementInputSource _input;

        public delegate void SendMovement(Vector2 position, Vector2 vector);

        protected override void OnAwake()
        {
            _input = GetComponent<IMovementInputSource>();
        }

        protected override void OnEnableSync()
        {
            AssertProperConnectionRole();
            _input.MovementUpdated += SendMovementIfChanged;
        }

        protected override void OnDisableSync()
        {
            _input.MovementUpdated -= SendMovementIfChanged;
        }

        private void AssertProperConnectionRole()
        {
            if (!_isPublishingInput())
            {
                Destroy(this);
            }
        }

        private void SendMovementIfChanged(Vector2 movement) => _sendMovement?.Invoke(transform.position, movement);
    }
}
