using Levels.Extensions;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFacesMovementDirectionOfParent : MonoBehaviour
    {
        [SerializeField] private float flipXThreshold;

        private IMovementInputSource _movementInput;
        private SpriteRenderer _spriteRenderer;
        [Inject] private Camera _camera;

        private void Awake()
        {
            _movementInput = GetComponentInParent<IMovementInputSource>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var horizontal = _camera.transform.right.InFloorCoordinates().normalized;
            var dot = Vector2.Dot(_movementInput.Movement, horizontal);

            if (dot > flipXThreshold)
            {
                _spriteRenderer.flipX = false;
            }
            else if (dot < -flipXThreshold)
            {
                _spriteRenderer.flipX = true;
            }
        }
    }
}