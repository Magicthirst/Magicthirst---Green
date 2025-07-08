using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.Enter
{
    public class Enter : MonoBehaviour
    {
        [SerializeField] private SceneAsset mainMenuScene;

        private EnterElement _component;

        [Inject] private CheckSignIn _checkSignedIn;
        [Inject] private string _signUpUrl;

        public delegate Task<SignInResult> CheckSignIn(string id);
        public delegate string GetSignUpUrl();

        private void Awake()
        {
            var document = GetComponent<UIDocument>()!;
            _component = document.rootVisualElement.Q<EnterElement>()!;
        }

        private void OnEnable()
        {
            _component.SignUpRequested += OnSignUpRequested;
            _component.SignInRequested += OnSignInRequested;
            _component.EnterUnanimouslyRequested += OnEnterUnanimouslyRequested;
        }

        private void OnDisable()
        {
            _component.SignUpRequested -= OnSignUpRequested;
            _component.SignInRequested -= OnSignInRequested;
            _component.EnterUnanimouslyRequested -= OnEnterUnanimouslyRequested;
        }

        private async void OnSignInRequested(string id)
        {
            var result = await _checkSignedIn(id);
            if (result is SignInResult.Error error)
            {
                _component.DisplaySignInError(error.Message);
            }
            else
            {
                SceneManager.LoadScene(mainMenuScene.name);
            }
        }

        private void OnEnterUnanimouslyRequested() => SceneManager.LoadScene(mainMenuScene.name);

        private void OnSignUpRequested() => Application.OpenURL(_signUpUrl);

        public abstract record SignInResult
        {
            private SignInResult() {}

            public sealed record Success : SignInResult;

            public sealed record Error : SignInResult
            {
                public string Message;
            }
        }
    }
}
