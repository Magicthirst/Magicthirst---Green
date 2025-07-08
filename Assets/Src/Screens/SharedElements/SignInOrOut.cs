using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.SharedElements
{
    public class SignInOrOut : MonoBehaviour
    {
        [SerializeField] private SceneAsset enterScene;

        private SignInOrOutElement _component;

        [Inject] private Exit _exit; 

        public delegate void Exit();

        private void Awake()
        {
            var document = GetComponent<UIDocument>()!;
            _component = document.rootVisualElement.Q<SignInOrOutElement>();
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

        private void OnSignOutRequested()
        {
            _exit();
            _component.UpdateUIState();
        }
    }
}
