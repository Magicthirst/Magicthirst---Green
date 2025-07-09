using System.Threading.Tasks;
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

        public delegate Task<bool> AskToJoinSession(string hostId);

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
            if (await _askToJoinSession(hostId))
            {
                SceneManager.LoadScene(gameScene.name);
            }
            else
            {
                _element.DisplayError("Something went wrong"); // TODO
            }
        }
    }
}
