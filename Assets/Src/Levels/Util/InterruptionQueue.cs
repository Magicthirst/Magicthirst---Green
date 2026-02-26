using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Levels.Util
{
    // ReSharper disable once UnusedTypeParameter
    public interface IInterruptable<TReason>
    {
        public void Interrupt(IEnumerator block);
    }

    public interface IMovementReason {}

    public class InterruptionQueue : IDisposable
    {
        public bool Running => _interventions.Count != 0 || _runningIntervention is not null;

        private readonly Queue<IEnumerator> _interventions;
        private Coroutine _runningIntervention;

        private readonly MonoBehaviour _owner;

        [CanBeNull] private readonly YieldInstruction _divider;

        public InterruptionQueue(MonoBehaviour owner, [CanBeNull] YieldInstruction divider)
        {
            _interventions = new Queue<IEnumerator>();
            _runningIntervention = null;
            _owner = owner;
            _divider = divider;
        }

        public void Interrupt(IEnumerator block)
        {
            var interruption = ToInterruption(block);

            if (_runningIntervention is not null)
            {
                _interventions.Enqueue(interruption);
            }
            else
            {
                _runningIntervention = _owner.StartCoroutine(interruption);
            }
        }

        public void Dispose()
        {
            if (_runningIntervention is not null)
            {
                _owner.StopCoroutine(_runningIntervention);
            }
            _interventions.Clear();
        }

        private IEnumerator ToInterruption(IEnumerator block)
        {
            yield return block;
            yield return _divider;

            if (_interventions.TryDequeue(out var next))
            {

                yield return next;
            }
            else
            {
                _runningIntervention = null;
            }
        }

        public IEnumerator MakeInterruptable(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                while (Running)
                {
                    yield return _divider;
                }

                yield return enumerator.Current;
            }
        }
    }

    public static class InterruptionsExtensions
    {
        public static bool TryInterrupt<TReason>(this GameObject gameObject, IEnumerator routine)
        {
            if (gameObject.TryGetComponent<IInterruptable<TReason>>(out var behaviour))
            {
                behaviour.Interrupt(routine);
                return true;
            }

            return false;
        }
    }
}