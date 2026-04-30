using UnityEngine;

namespace DI
{
    public class RoomMemberTag : MonoBehaviour
    {
        [SerializeField] private int roomId;

        public int RoomId => roomId;
    }
}