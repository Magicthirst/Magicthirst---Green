using System;
using Levels.Extensions;
using UnityEngine;

namespace Levels.AI.Util
{
    public class Enemy
    {
        public float LookAheadTime { init; private get; }

        private Transform _transform;
        private Func<Vector3> _calculateLookAhead;

        public bool IsSet => _transform is not null;

        public void SetTo(Transform enemy)
        {
            _transform = enemy;

            if (enemy.TryGetComponent(out IObservableMovement movement) &&
                enemy.TryGetComponent(out ISpeedProvider speedProvider))
            {
                _calculateLookAhead = () => movement.AbsoluteMovement.ToX0Y() * (speedProvider.Speed * LookAheadTime);
            }
            else
            {
                _calculateLookAhead = () => Vector3.zero;
            }
        }

        public void Unset() => _transform = null;

        public bool Is(Collider other) => IsSet && _transform == other.transform;

        public Vector3 Position => _transform.position;

        public Vector3 EstimatedPosition => _transform.position + _calculateLookAhead();
    }
}