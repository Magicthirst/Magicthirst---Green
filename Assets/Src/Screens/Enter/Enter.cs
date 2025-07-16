using System.Threading.Tasks;
using Common;
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
        private string _signUpUrl;

        public delegate Task<SignInResult> CheckSignIn(string id);
        public delegate string GetSignUpUrl();

        [Inject]
        public void Construct(GetSignUpUrl getSignUpUrl) => _signUpUrl = getSignUpUrl();

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
            switch (result)
            {
                case SignInResult.Success:
                    SceneManager.LoadScene(mainMenuScene.name);
                    break;
                case SignInResult.UserNotFound:
                default:
                    _component.DisplaySignInError(result.ToString()); // TODO
                    break;
            }
        }

        private void OnEnterUnanimouslyRequested() => SceneManager.LoadScene(mainMenuScene.name);

        private void OnSignUpRequested() => Application.OpenURL(_signUpUrl);
    }
}
