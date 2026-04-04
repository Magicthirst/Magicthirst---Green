 using UnityEngine;
using VContainer;

namespace Levels.Sync
{
    [RequireComponent(typeof(IObservableMovement))]
    public class InputSender : SyncBehavior
    {
        [Inject] private IObjectResolver _resolver;

        private SendMovement _sendMovement = null!;
        private IObservableMovement _input = null!;

        public delegate void SendMovement(Vector2 position, Vector2 vector);

        protected override void OnAwake()
        {
            _input = GetComponent<IObservableMovement>();
        }

        protected override void OnEnableSync()
        {
            AssertProperConnectionRole(_resolver.Resolve<IsPublishingInput>());
            _sendMovement = _resolver.Resolve<SendMovement>();
            _input.Moved += SendMovementIfChanged;
        }

        protected override void OnDisableSync()
        {
            _input.Moved -= SendMovementIfChanged;
        }

        private void AssertProperConnectionRole(IsPublishingInput isPublishingInput)
        {
            if (!isPublishingInput())
            {
                Destroy(this);
            }
        }

        private void SendMovementIfChanged(Vector2 movement) => _sendMovement?.Invoke(transform.position, movement);
    }
}
