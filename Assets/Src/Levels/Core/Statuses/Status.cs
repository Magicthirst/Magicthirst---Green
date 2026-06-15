using System;
using System.Collections;
using Levels.Directorship;
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
            var endTime = LevelDirector.GameplayTime + duration;
            while (LevelDirector.GameplayTime < endTime)
            {
                yield return null;
            }
        }
    }
}