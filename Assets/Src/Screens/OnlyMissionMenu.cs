using Shared;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Screens
{
    public class OnlyMissionMenu : MonoBehaviour
    {
        [SerializeField] private Button startMission;
        [SerializeField] private Button exit;

        [Inject] private GameNavigation _navigation;

        private void OnEnable()
        {
            startMission.onClick.AddListener(StartMission);
            exit.onClick.AddListener(QuitGame);
        }

        private void OnDisable()
        {
            startMission.onClick.RemoveListener(StartMission);
            exit.onClick.RemoveListener(QuitGame);
        }

        private void StartMission() => _navigation.GoGame();

        private void QuitGame() => _navigation.QuitGame();
    }
}