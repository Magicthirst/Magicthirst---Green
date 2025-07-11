using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.MainMenu
{
    [UxmlElement(nameof(MainMenuElement))]
    public partial class MainMenuElement : VisualElement
    {
        private IAuthenticatedState _authenticated;
        public event Action PlayOfflineRequested;
        public event Action HostSessionRequested;
        public event Action JoinSessionRequested;
        public event Action ExitApplicationRequested;

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
            throw new NotImplementedException();
        }

        private void OnDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            _authenticated.IsAuthenticatedChanged -= OnAuthenticatedStateChange;
        }

        private void OnAuthenticatedStateChange(bool state)
        {
            this.Q<Button>("HostSession").SetEnabled(state);
            this.Q<Button>("JoinSession").SetEnabled(state);
        }

        public interface IAuthenticatedState
        {
            public event Action<bool> IsAuthenticatedChanged;

            public bool IsAuthenticated { get; }
        }
    }
}
