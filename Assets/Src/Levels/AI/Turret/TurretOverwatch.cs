using System.Collections;
using UnityEngine;
using VContainer;

namespace Levels.AI.Turret
{
    public class TurretOverwatch : FsmState
    {
        protected override bool _IsReady => true;

        [SerializeField] private float rotationSpeed;
        [SerializeField] private float pauseSeconds;

        [Inject] private Transform _transform = null!;

        private WaitForSeconds _pause;
        private bool _active;
        private Coroutine _lookAroundCoroutine;

        protected override void Awake()
        {
            base.Awake();
            _pause = new WaitForSeconds(pauseSeconds);
        }

        public override void Enter()
        {
            _active = true;
            _lookAroundCoroutine = StartCoroutine(LookAroundOnOverwatch());
        }

        private IEnumerator LookAroundOnOverwatch()
        {
            var currentY = _transform.rotation.eulerAngles.y;
            var destinationY = currentY;
            var threshold = rotationSpeed * Time.deltaTime * 2;
            var direction = 1f;

            while (_active)
            {
                currentY = _transform.rotation.eulerAngles.y;
                if (Mathf.DeltaAngle(currentY, destinationY) < threshold)
                {
                    yield return _pause;
                    destinationY = Random.Range(-180f, 180f);
                    direction = Mathf.Sign(currentY - destinationY);
                }

                yield return null;

                _transform.Rotate(Vector3.up * (direction * Time.deltaTime * rotationSpeed));
            }
        }

        public override void Exit()
        {
            _active = false;
            if (_lookAroundCoroutine != null)
            {
                StopCoroutine(_lookAroundCoroutine);
                _lookAroundCoroutine = null;
            }
        }
    }
}