using System.Collections.Generic;
using Levels.Abilities.CommonImpacts;
using Levels.Extensions;
using Levels.IntentsImpacts;
using UnityEngine;

namespace Levels.Abilities.TeleportChip
{
    public class TeleportChipMapper :
        IIntentToImpactsMapper<TeleportChipThrowIntent>,
        IIntentToImpactsMapper<TeleportChipActivateIntent>
    {
        public IEnumerable<IImpact> Map(TeleportChipThrowIntent intent)
        {
            var velocity = intent.Direction + intent.Movement.ToX0Y() * intent.Config.ThrowVelocity;
            var angularVelocity = Vector3.Cross(intent.Direction, Vector3.up) * intent.Config.FlippingAngularVelocity;

            var floorOffset = velocity.normalized * intent.Config.ThrowOriginHorizontalOffset;
            var offset = floorOffset.With(y: intent.Config.ThrowOriginVerticalOffset);
            var origin = intent.Thrower.transform.position + offset;

            yield return new TeleportChipSpawnImpact(intent.Chip, origin, velocity, angularVelocity, intent.Config);
        }

        public IEnumerable<IImpact> Map(TeleportChipActivateIntent intent) => new[]
        {
            new TeleportImpact(intent.Player, intent.Chip.transform.position)
        };
    }
}