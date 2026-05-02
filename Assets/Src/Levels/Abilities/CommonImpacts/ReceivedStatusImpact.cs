using Levels.Core.Statuses;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.CommonImpacts
{
    public record ReceivedStatusImpact(GameObject Target, IStatus Status, ImpactContext Context = ImpactContext.None) : IImpact;
}