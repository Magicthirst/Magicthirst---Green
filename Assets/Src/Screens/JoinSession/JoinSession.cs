using System;
using System.Threading.Tasks;
using Common;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.JoinSession
{
    [RequireComponent(typeof(UIElement))]    
    public class JoinSession : MonoBehaviour
    {
        private JoinSessionElement _element;

        [Inject] private AskToJoinSession _askToJoinSession;
        [Inject] private IAssignConnectionRole _assignConnectionRole;
        [Inject] private IGameNavigation _navigation;

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

        private void OnCancelRequested() => _navigation.GoMainMenu();

        private async void OnJoinRequested(string hostId)
        {
            var result = await _askToJoinSession(hostId);

            if (result == JoinSessionResult.Success)
            {
                _assignConnectionRole.Guest();
                _navigation.GoGame();
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
