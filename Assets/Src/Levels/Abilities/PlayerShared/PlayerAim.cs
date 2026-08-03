using Levels.Util.MasksRegistry;
using UnityEngine;
using Util;

namespace Levels.Abilities.PlayerShared
{
    public static class PlayerAim
    {
        private static readonly RaycastHit[] Buffer = new RaycastHit[2];

        public static Vector3 GetDirection(Transform camera, Transform character, MasksRegistry registry)
        {
            if (
                Physics.RaycastNonAlloc(camera.position, camera.forward, Buffer) > 0 &&
                Buffer.TryGetFirst(out var hit, hit => !registry.Is(hit.transform.gameObject, Mask.PlayerCharacter))
            )
            {
                return (hit.point - character.position).normalized;
            }

            return camera.forward;
        }
    }
}