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

        private MainMenuElement _component;

        [Inject] private HostSession _hostSession;
        [Inject] private JoinSession _joinSession;

        public delegate Task<bool> HostSession();

        public delegate Task<bool> JoinSession();

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

        private void OnPlayOfflineRequested() => SceneManager.LoadScene(gameScene.name);

        private async void OnHostSessionRequested()
        {
            if (await _hostSession())
            {
                SceneManager.LoadScene(gameScene.name);
            }
            else
            {
                _component.DisplaySomethingWentWrong();
            }
        }

        private async void OnJoinSessionRequested()
        {
            if (await _joinSession())
            {
                SceneManager.LoadScene(gameScene.name);
            }
            else
            {
                _component.DisplaySomethingWentWrong();
            }
        }

        private void OnExitApplicationRequested() => Application.Quit(0);
    }
}
