using System;
using System.Collections;
using UnityEngine;

namespace Levels.Core.Statuses
{
    using DisposeAction = Action;

    public interface IStatus
    {
        public IEnumerator Run(Entity holder);
    }

    public static class Status
    {
        public static DisposeAction RunIn(this IStatus status, Entity holder)
        {
            Coroutine coroutine = null!;
            coroutine = holder.Runner.StartCoroutine(RoutineWithEnd());

            return Stop;

            IEnumerator RoutineWithEnd()
            {
                yield return status.Run(holder);
                Stop();
            }

            void Stop()
            {
                Coroutine c;
                // ReSharper disable once AccessToModifiedClosure
                if ((c = coroutine) != null)
                {
                    holder.Runner.StopCoroutine(c);
                    coroutine = null;
                }
            }
        }
    }
}