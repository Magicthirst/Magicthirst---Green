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
        public event Action PlayOfflineRequested;
        public event Action HostSessionRequested;
        public event Action JoinSessionRequested;
        public event Action ExitApplicationRequested;

        public delegate bool IsAuthenticated();

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
        }

        [Inject]
        public void Configure(IsAuthenticated isAuthenticated)
        {
            var authenticated = isAuthenticated();

            this.BindButtons(
                ("PlayOffline", () => PlayOfflineRequested?.Invoke()),
                ("Exit", () => ExitApplicationRequested?.Invoke())
            );

            if (authenticated)
            {
                this.BindButtons(
                    ("HostSession", () => HostSessionRequested?.Invoke()),
                    ("JoinSession", () => JoinSessionRequested?.Invoke())
                );
                this.Q<Button>("HostSession").SetEnabled(true);
                this.Q<Button>("JoinSession").SetEnabled(true);
            }
        }

        public void DisplaySomethingWentWrong()
        {
            throw new NotImplementedException();
        }
    }
}
