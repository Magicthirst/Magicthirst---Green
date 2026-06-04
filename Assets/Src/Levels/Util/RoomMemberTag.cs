using UnityEngine;

namespace Levels.Util
{
    public class RoomMemberTag : MonoBehaviour
    {
        [SerializeField] private int roomId;

        public int RoomId => roomId;
    }
}