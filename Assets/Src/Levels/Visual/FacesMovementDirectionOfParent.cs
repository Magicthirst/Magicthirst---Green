using Levels.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Levels.Visual
{
    public class FacesMovementDirectionOfParent : MonoBehaviour
    {
        private IMovementInputSource _movementInput;

        private void Awake()
        {
            _movementInput = GetComponentInParent<IMovementInputSource>();
        }

        private void Update()
        {
            var direction = _movementInput.Movement;
            var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.eulerAngles = transform.eulerAngles.With(y: angle);
        }
    }
}