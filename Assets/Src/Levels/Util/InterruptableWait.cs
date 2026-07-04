using System.Collections;
using Levels.Directorship;

namespace Levels.Util
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