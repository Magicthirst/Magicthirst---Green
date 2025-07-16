using System.Threading.Tasks;
using Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.JoinSession
{
    public class JoinSession : MonoBehaviour
    {
        [SerializeField] private SceneAsset gameScene;
        [SerializeField] private SceneAsset mainMenuScene;

        private JoinSessionElement _element;

        [Inject] private AskToJoinSession _askToJoinSession;
        [Inject] private IAssignConnectionRole _assignConnectionRole;

        public delegate Task<JoinSessionResult> AskToJoinSession(string hostId);

        private void Awake()
        {
            var document = GetComponent<UIDocument>();
            _element = document.rootVisualElement.Q<JoinSessionElement>();
        }

        private void OnEnable()
        {
            _element.CancelRequested += OnCancelRequested;
            _element.JoinRequested += OnJoinRequested;
        }

        private void OnDisable()
        {
            _element.CancelRequested -= OnCancelRequested;
            _element.JoinRequested -= OnJoinRequested;
        }

        private void OnCancelRequested() => SceneManager.LoadScene(mainMenuScene.name);

        private async void OnJoinRequested(string hostId)
        {
            var result = await _askToJoinSession(hostId);
            switch (result)
            {
                case JoinSessionResult.Success:
                    _assignConnectionRole.Guest();
                    SceneManager.LoadScene(gameScene.name);
                    break;
                case JoinSessionResult.HostNotFound:
                case JoinSessionResult.NotWelcome:
                case JoinSessionResult.SessionDoesNotExists:
                default:
                    _element.DisplayError("Something went wrong"); // TODO
                    break;
            }
        }
    }
}
