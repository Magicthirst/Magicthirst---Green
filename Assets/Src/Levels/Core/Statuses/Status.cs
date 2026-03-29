using System;
using System.Collections;
using UnityEngine;

namespace Levels.Core.Statuses
{
    public interface IStatus
    {
        public IEnumerator Run(Entity holder);
    }

    [Serializable]
    public class DecorativeStatus : IStatus
    {
        [SerializeField] private float duration;

        public DecorativeStatus() {}

        public DecorativeStatus(float duration)
        {
            this.duration = duration;
        }

        public IEnumerator Run(Entity _)
        {
            yield return new WaitForSeconds(duration);
        }
    }
}