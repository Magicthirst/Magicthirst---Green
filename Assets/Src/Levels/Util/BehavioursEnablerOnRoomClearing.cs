using System;
using Levels.Core.Room;
using UnityEngine;
using VContainer;

namespace Levels.Util
{
    public class BehavioursEnablerOnRoomClearing : MonoBehaviour
    {
        private int _RoomId => GetComponent<RoomMemberTag>().RoomId;

        [SerializeField] private MonoBehaviour[] behaviours;

        private RoomUnits _units;

        [Inject]
        private void Construct(Func<int, RoomUnits> getUnits) => _units = getUnits(_RoomId);

        private void Awake()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = false;
            }
        }

        private void OnEnable()
        {
            _units.Cleared += OnRoomIsCleared;
        }

        private void OnRoomIsCleared()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = true;
            }
        }

        private void OnDisable()
        {
            _units.Cleared -= OnRoomIsCleared;
        }
    }
}