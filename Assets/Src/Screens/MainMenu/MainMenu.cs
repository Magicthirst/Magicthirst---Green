using System.Threading.Tasks;
using Screens.SharedElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.MainMenu
{
    [RequireComponent(typeof(SignInOrOut))]    
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SceneAsset gameScene;
        [SerializeField] private SceneAsset joinSessionScene;

        private MainMenuElement _component;

        [Inject] private HostSession _hostSession;
        [Inject] private IAssignConnectionRole _assignConnectionRole;

        public delegate Task<bool> HostSession();

        private void Awake()
        {
            var document = GetComponent<UIDocument>();
            _component = document.rootVisualElement.Q<MainMenuElement>();
        }

        private void OnEnable()
        {
            _component.PlayOfflineRequested += OnPlayOfflineRequested;
            _component.HostSessionRequested += OnHostSessionRequested;
            _component.JoinSessionRequested += OnJoinSessionRequested;
            _component.ExitApplicationRequested += OnExitApplicationRequested;
        }

        private void OnDisable()
        {
            _component.PlayOfflineRequested -= OnPlayOfflineRequested;
            _component.HostSessionRequested -= OnHostSessionRequested;
            _component.JoinSessionRequested -= OnJoinSessionRequested;
            _component.ExitApplicationRequested -= OnExitApplicationRequested;
        }

        private void OnPlayOfflineRequested()
        {
            _assignConnectionRole.Offline();
            SceneManager.LoadScene(gameScene.name);
        }

        private async void OnHostSessionRequested()
        {
            if (await _hostSession())
            {
                _assignConnectionRole.Host();
                SceneManager.LoadScene(gameScene.name);
            }
            else
            {
                _component.DisplaySomethingWentWrong();
            }
        }

        private void OnJoinSessionRequested() => SceneManager.LoadScene(joinSessionScene.name);

        private void OnExitApplicationRequested() => Application.Quit(0);
    }
}
