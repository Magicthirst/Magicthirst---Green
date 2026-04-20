using Common;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.SharedElements
{
    [RequireComponent(typeof(UIElement))]    
    public class SignInOrOut : MonoBehaviour
    {
        private SignInOrOutElement _component;

        [Inject] private Exit _exit;
        [Inject] private IGameNavigation _navigation;

        public delegate void Exit();

        public void Awake()
        {
            GetComponent<IUIReady>().UIReady += OnUIReady;
        }

        private void OnUIReady()
        {
            var document = GetComponent<UIDocument>()!;
            _component = document.rootVisualElement.Q<SignInOrOutElement>();

            _component.SignInRequested += OnSignInRequested;
            _component.SignOutRequested += OnSignOutRequested;
        }

        private void OnDisable()
        {
            _component.SignInRequested -= OnSignInRequested;
            _component.SignOutRequested -= OnSignOutRequested;
        }

        private void OnSignInRequested() => _navigation.GoSignIn();

        private void OnSignOutRequested()
        {
            _exit();
            _component.UpdateUIState();
        }
    }
}
