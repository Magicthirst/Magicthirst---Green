using System.Collections;
using UnityEngine;

namespace Levels.AI.Util
{
    public static class InterruptableWait
    {
        public static IEnumerator ForSeconds(float seconds)
        {
            var start = Time.time;
            while (Time.time < start + seconds)
            {
                yield return null;
            }
        }
    }
}