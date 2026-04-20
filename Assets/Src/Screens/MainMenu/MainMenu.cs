using System.Threading.Tasks;
using Common;
using Screens.SharedElements;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.MainMenu
{
    [RequireComponent(typeof(UIElement))]    
    [RequireComponent(typeof(SignInOrOut))]    
    public class MainMenu : MonoBehaviour
    {
        private MainMenuElement _component;

        [Inject] private HostSession _hostSession;
        [Inject] private IAssignConnectionRole _assignConnectionRole;
        [Inject] private IGameNavigation _navigation;

        public delegate Task<HostSessionResult> HostSession();

        public void Awake()
        {
            GetComponent<IUIReady>().UIReady += OnUIReady;
        }

        private void OnUIReady()
        {
            var document = GetComponent<UIDocument>();
            _component = document.rootVisualElement.Q<MainMenuElement>();

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
            _navigation.GoGame();
        }

        private async void OnHostSessionRequested()
        {
            var result = await _hostSession();
            switch (result)
            {
                case HostSessionResult.Success:
                    _assignConnectionRole.Host();
                    _navigation.GoGame();
                    break;
                case HostSessionResult.UnknownError:
                default:
                    _component.DisplaySomethingWentWrong();
                    break;
            }
        }

        private void OnJoinSessionRequested() => _navigation.GoJoinSession();

        private void OnExitApplicationRequested() => _navigation.QuitGame();

    }
}
