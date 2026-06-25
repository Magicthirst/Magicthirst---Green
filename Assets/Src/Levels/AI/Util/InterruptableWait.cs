using System.Collections;
using Levels.Directorship;
using UnityEngine;

namespace Levels.AI.Util
{
    public static class InterruptableWait
    {
        public static IEnumerator ForSeconds(float seconds)
        {
            var start = LevelDirector.GameplayTime;
            while (LevelDirector.GameplayTime < start + seconds)
            {
                yield return null;
            }
        }
    }
}