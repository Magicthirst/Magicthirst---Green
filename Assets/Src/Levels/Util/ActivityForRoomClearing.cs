using System;
using Levels.Core.Room;
using UnityEngine;
using VContainer;

namespace Levels.Util
{
    public class ActivityForRoomClearing : MonoBehaviour
    {
        private int _RoomId => GetComponent<RoomMemberTag>().RoomId;

        [SerializeField] private GameObject[] activeOnRoomCleared;
        [SerializeField] private GameObject[] nonActiveOnRoomCleared;

        private RoomUnits _units;

        [Inject]
        private void Construct(Func<int, RoomUnits> getUnits) => _units = getUnits(_RoomId);

        private void Awake()
        {
            foreach (var o in activeOnRoomCleared)
            {
                o.SetActive(false);
            }
            foreach (var o in nonActiveOnRoomCleared)
            {
                o.SetActive(true);
            }
        }

        private void OnEnable()
        {
            _units.Cleared += OnRoomIsCleared;
        }

        private void OnRoomIsCleared()
        {
            foreach (var o in activeOnRoomCleared)
            {
                o.SetActive(true);
            }
            foreach (var o in nonActiveOnRoomCleared)
            {
                o.SetActive(false);
            }
        }

        private void OnDisable()
        {
            _units.Cleared -= OnRoomIsCleared;
        }
    }
}