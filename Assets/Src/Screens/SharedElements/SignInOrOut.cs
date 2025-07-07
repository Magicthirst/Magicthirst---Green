using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.SharedElements
{
    public class SignInOrOut : MonoBehaviour
    {
        private event Exit OnExit;

        [SerializeField] private SceneAsset enterScene;

        private UIDocument _document;
        private SignInOrOutElement _component;

        public delegate void Exit();

        [Inject]
        public void Construct(Exit exit) => OnExit += exit;

        private void Awake()
        {
            _document = GetComponent<UIDocument>()!;
            _component = _document.rootVisualElement.Q<SignInOrOutElement>();
        }

        private void OnEnable()
        {
            _component.SignInRequested += OnSignInRequested;
            _component.SignOutRequested += OnSignOutRequested;
        }

        private void OnDisable()
        {
            _component.SignInRequested -= OnSignInRequested;
            _component.SignOutRequested -= OnSignOutRequested;
        }

        private void OnSignInRequested() => SceneManager.LoadScene(enterScene.name);

        private void OnSignOutRequested() => OnExit?.Invoke();
    }
}
