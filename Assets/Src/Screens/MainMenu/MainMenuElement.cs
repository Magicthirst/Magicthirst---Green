using System;
using System.IO;
using Common;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.MainMenu
{
    [UxmlElement(nameof(MainMenuElement))]
    public partial class MainMenuElement : VisualElement
    {
        public event Action PlayOfflineRequested;
        public event Action HostSessionRequested;
        public event Action JoinSessionRequested;
        public event Action ExitApplicationRequested;

        private Label _somethingWentWrong;

        private IAuthenticatedState _authenticated;   

        public MainMenuElement()
        {
            var asset = Resources.Load<VisualTreeAsset>(nameof(MainMenuElement));
            if (asset == null)
            {
                Debug.LogError($"UXML asset '{nameof(MainMenuElement)}' not found in Resources for EnterElement.");
                throw new FileNotFoundException($"UXML asset '{nameof(MainMenuElement)}' not found.");
            }

            asset.CloneTree(this);

            this.Q<Button>("HostSession").SetEnabled(false);
            this.Q<Button>("JoinSession").SetEnabled(false);
            _somethingWentWrong = this.Q<Label>("SomethingWentWrong");
            _somethingWentWrong.visible = false;

            this.BindButtons(
                ("PlayOffline", () => PlayOfflineRequested?.Invoke()),
                ("Exit", () => ExitApplicationRequested?.Invoke()),
                ("HostSession", () => HostSessionRequested?.Invoke()),
                ("JoinSession", () => JoinSessionRequested?.Invoke())
            );

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
        }

        [Inject]
        public void Configure(IAuthenticatedState authenticated)
        {
            _authenticated = authenticated;
            OnAuthenticatedStateChange(_authenticated.IsAuthenticated);
            _authenticated.IsAuthenticatedChanged += OnAuthenticatedStateChange;
        }

        public void DisplaySomethingWentWrong()
        {
            _somethingWentWrong.visible = true;
        }

        private void OnDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            if (_authenticated != null)
            {
                _authenticated.IsAuthenticatedChanged -= OnAuthenticatedStateChange;
            }
        }

        private void OnAuthenticatedStateChange(bool state)
        {
            this.Q<Button>("HostSession").SetEnabled(state);
            this.Q<Button>("JoinSession").SetEnabled(state);
        }
    }
}
