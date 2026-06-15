using System.Collections;
using System.Collections.Generic;

namespace Levels.Directorship
{
    public interface ILevelScenarioPlayer
    {
        IEnumerator GetRoutine();
    }

    public static class LevelScenarioPlayer
    {
        public static IEnumerator Link(this IEnumerable<ILevelScenarioPlayer> players)
        {
            foreach (var player in players)
            {
                yield return player.GetRoutine();
            }
        }
    }
}