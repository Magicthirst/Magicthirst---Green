using System;
using System.Threading.Tasks;
using Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.JoinSession
{
    [RequireComponent(typeof(UIElement))]    
    public class JoinSession : MonoBehaviour
    {
        [SerializeField] private AssetReference gameScene;
        [SerializeField] private AssetReference mainMenuScene;

        private JoinSessionElement _element;

        [Inject] private AskToJoinSession _askToJoinSession;
        [Inject] private IAssignConnectionRole _assignConnectionRole;

        public delegate Task<JoinSessionResult> AskToJoinSession(string hostId);

        public void Awake()
        {
            GetComponent<IUIReady>().UIReady += OnUIReady;
        }

        private void OnUIReady()
        {
            var document = GetComponent<UIDocument>();
            _element = document.rootVisualElement.Q<JoinSessionElement>();

            _element.CancelRequested += OnCancelRequested;
            _element.JoinRequested += OnJoinRequested;
        }

        private void OnDisable()
        {
            _element.CancelRequested -= OnCancelRequested;
            _element.JoinRequested -= OnJoinRequested;
        }

        private void OnCancelRequested() => mainMenuScene.LoadSceneAsync();

        private async void OnJoinRequested(string hostId)
        {
            var result = await _askToJoinSession(hostId);

            if (result == JoinSessionResult.Success)
            {
                _assignConnectionRole.Guest();
                gameScene.LoadSceneAsync();
                return;
            }

            _element.DisplayError(result switch
            {
                JoinSessionResult.HostNotFound => "Host were not found",
                JoinSessionResult.NotWelcome => "Host are not welcomes you",
                JoinSessionResult.SessionDoesNotExists => "This user is not hosting anything",
                JoinSessionResult.UnknownError => "Something went wrong",
                _ => throw new ArgumentOutOfRangeException()
            });
        }
    }
}
