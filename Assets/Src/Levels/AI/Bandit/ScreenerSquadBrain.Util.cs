using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Extensions;
using UnityEngine;

namespace Levels.AI.Bandit
{
    public static class ScreeningSquadBrainUtil
    {
        public static void CalculateFrontline
        (
            in Span<Vector3> frontline,
            Transform camera,
            Transform enemy,
            float memberRadius,
            float distance
        )
        {
            var length = frontline.Length;

            var distanceToEnemy = Vector3.Distance(camera.position, enemy.position);
            distance = Mathf.Min(distance, distanceToEnemy); // don't run away from enemy

            var enemyDirection = camera.forward.With(y: 0f);
            var enemyPosition = enemy.position;

            var frontCenter = enemyPosition + enemyDirection * distance;
            var frontAxis = Quaternion.Euler(0f, 90f, 0f) * enemyDirection;
            var frontWidth = length * memberRadius;

            for (var i = 0; i < length; i++)
            {
                var iRelativeToCenter = i - length / 2f;
                var centerOffset = frontAxis * (iRelativeToCenter * frontWidth);
                var place = frontCenter + centerOffset;
                frontline[i] = place;
            }
        }

        public static void AssignFrontPlaces(in Span<Vector3> places, Dictionary<int, Vector3> membersPlaces)
        {
            var length = places.Length;

            var map = new Span<int>(membersPlaces.Keys.ToArray());
            Span<bool> isPlaceAssigned = stackalloc bool[length];
            Span<bool> isMemberPlaced = stackalloc bool[length];

            for (var _ = 0; _ < length; _++)
            {
                (float Distance, int PlaceI, int MemberI) worstBest = (float.MinValue, -1, -1);

                for (var memberI = 0; memberI < length; memberI++)
                {
                    if (isMemberPlaced[memberI])
                    {
                        continue;
                    }

                    var member = membersPlaces[map[memberI]];

                    (float Distance, int PlaceI) best = (float.MaxValue, -1);

                    for (var placeI = 0; placeI < length; placeI++)
                    {
                        if (isPlaceAssigned[placeI])
                        {
                            continue;
                        }

                        var distance = (places[placeI] - member).sqrMagnitude;
                        if (distance < best.Distance)
                        {
                            best = (distance, placeI);
                        }
                    }

                    if (best.Distance > worstBest.Distance)
                    {
                        worstBest = (best.Distance, best.PlaceI, memberI);
                    }
                }

                isPlaceAssigned[worstBest.PlaceI] = true;
                isMemberPlaced[worstBest.MemberI] = true;

                membersPlaces[map[worstBest.MemberI]] = places[worstBest.PlaceI];
            }
        }
    }
}