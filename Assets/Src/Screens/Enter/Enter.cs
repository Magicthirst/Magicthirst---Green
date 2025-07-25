using System.Threading.Tasks;
using Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.Enter
{
    [RequireComponent(typeof(UIElement))]    
    public class Enter : MonoBehaviour
    {
        [SerializeField] private AssetReference mainMenuScene;

        private EnterElement _component;

        [Inject] private CheckSignIn _checkSignedIn;
        private string _signUpUrl;

        public delegate Task<SignInResult> CheckSignIn(string id);
        public delegate string GetSignUpUrl();

        [Inject]
        public void Construct(GetSignUpUrl getSignUpUrl) => _signUpUrl = getSignUpUrl();

        public void Awake()
        {
            GetComponent<IUIReady>().UIReady += OnUIReady;
        }

        private void OnUIReady()
        {
            var document = GetComponent<UIDocument>()!;
            _component = document.rootVisualElement.Q<EnterElement>()!;

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
                    mainMenuScene.LoadSceneAsync();
                    break;
                case SignInResult.UserNotFound:
                default:
                    _component.DisplaySignInError(result.ToString()); // TODO
                    break;
            }
        }

        private void OnEnterUnanimouslyRequested() => mainMenuScene.LoadSceneAsync();

        private void OnSignUpRequested() => Application.OpenURL(_signUpUrl);
    }
}
