using System.Collections;
using UnityEngine;

namespace Levels.AI.Util
{
    public static class InterruptableWait
    {
        public static IEnumerator ForSeconds(float seconds)
        {
            var start = Time.time;
            for (var t = start; t < start + seconds; t = Time.time)
            {
                yield return null;
            }
        }
    }
}