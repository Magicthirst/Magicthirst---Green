using System;
using System.Collections;
using Levels.Core.Room;
using Levels.UI.Tutorials;
using UnityEngine;
using Util;
using VContainer;
using static Levels.Directorship.LevelActivityMask;

namespace Levels.Directorship
{
    public class FirstLevelTutorial : MonoBehaviour, ILevelScenarioPlayer
    {
        [Serializable]
        private class TutorialStage
        {
            public TutorialKeysStep step;
            public GameObject window;
        }

        [Header("Tutorials")]
        [SerializeField] private TutorialStage saber;
        [SerializeField] private TutorialStage movement;
        [SerializeField] private TutorialStage shoot;
        [SerializeField] private TutorialStage chaos;
        [SerializeField] private TutorialStage teleport;

        [Header("Rooms")]
        [SerializeField] private int roomBeforeGunTutorialId;
        [SerializeField] private int roomBeforeChaosTutorialId;
        [SerializeField] private int roomBeforeTeleportTutorialId;

        private RoomUnits _roomBeforeGunTutorial;
        private RoomUnits _roomBeforeChaosTutorial;
        private RoomUnits _roomBeforeTeleportTutorial;

        private IEnumerator _tutorialRoutine;

        [Inject]
        private void Construct(Func<int, RoomUnits> resolveUnits)
        {
            _roomBeforeGunTutorial = resolveUnits(roomBeforeGunTutorialId);
            _roomBeforeChaosTutorial = resolveUnits(roomBeforeChaosTutorialId);
            _roomBeforeTeleportTutorial = resolveUnits(roomBeforeTeleportTutorialId);
        }

        private void Start()
        {
            HideAll();
        }

        public IEnumerator GetRoutine()
        {
            yield return PlayTutorial(saber, TutorialChooseSabre, TutorialUseSecondary);
            yield return PlayTutorial(movement, TutorialMovement);
            yield return WaitRoomCleared(_roomBeforeGunTutorial);
            yield return PlayTutorial(shoot, TutorialChoosePistol, TutorialUsePrimary);
            yield return WaitRoomCleared(_roomBeforeChaosTutorial);
            yield return PlayTutorial(chaos, TutorialChooseChaos, TutorialUseSecondary);
            yield return WaitRoomCleared(_roomBeforeTeleportTutorial);
            yield return PlayTutorial(teleport, TutorialChip);

            LevelDirector.ActivityMask = Gameplay;
        }

        private IEnumerator PlayTutorial(TutorialStage stage, params LevelActivityMask[] steps)
        {
            if (stage.window is null)
            {
                yield break;
            }

            stage.window.SetActive(true);

            foreach (var step in steps)
            {
                LevelDirector.ActivityMask = step;
                yield return new WaitUntil(() => stage.step.IsCompleted(step));
            }

            stage.window.SetActive(false);
        }

        private static IEnumerator WaitRoomCleared(RoomUnits room)
        {
            if (room is null)
            {
                yield break;
            }

            LevelDirector.ActivityMask = Gameplay;
            yield return new WaitUntil(() => room.IsCleared);
        }

        private void HideAll()
        {
            saber.window.OrNull()?.SetActive(false);
            movement.window.OrNull()?.SetActive(false);
            shoot.window.OrNull()?.SetActive(false);
            chaos.window.OrNull()?.SetActive(false);
            teleport.window.OrNull()?.SetActive(false);
        }
    }
}