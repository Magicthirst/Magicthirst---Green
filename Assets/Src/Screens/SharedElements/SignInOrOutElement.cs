using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens.SharedElements
{
    [UxmlElement(nameof(SignInOrOutElement))]
    public partial class SignInOrOutElement : VisualElement
    {
        public event Action SignInRequested;
        public event Action SignOutRequested;

        private GetSignedInName _getSignedInNameDelegate = () => null;
        [CanBeNull] private string _Username => _getSignedInNameDelegate();

        private readonly VisualElement _signedInGroup;
        private readonly VisualElement _notSignedInGroup;
        private readonly Label _nameLabel;

        [CanBeNull]
        public delegate string GetSignedInName();

        public SignInOrOutElement()
        {
            var asset = Resources.Load<VisualTreeAsset>(nameof(SignInOrOutElement));
            if (asset == null)
            {
                Debug.LogError($"UXML asset '{nameof(SignInOrOutElement)}' not found in Resources for EnterElement.");
                throw new FileNotFoundException($"UXML asset '{nameof(SignInOrOutElement)}' not found.");
            }

            asset.CloneTree(this);
            this.BindButtons(
                ("Enter", () => SignInRequested?.Invoke()),
                ("Exit", () => SignOutRequested?.Invoke())
            );

            _signedInGroup = this.Q<VisualElement>("SignedIn");
            _notSignedInGroup = this.Q<VisualElement>("NotSignedIn");
            _nameLabel = this.Q<Label>("Username");

            RegisterCallback<AttachToPanelEvent>(_ => UpdateUIState());
        }

        [Inject]
        public void Construct(GetSignedInName getSignedInName)
        {
            _getSignedInNameDelegate = getSignedInName;
            UpdateUIState();
        }

        private void UpdateUIState()
        {
            var username = _Username;
            if (username != null)
            {
                _nameLabel.text = username;
                _signedInGroup.style.display = DisplayStyle.Flex;
                _notSignedInGroup.style.display = DisplayStyle.None;
            }
            else
            {
                _signedInGroup.style.display = DisplayStyle.None;
                _notSignedInGroup.style.display = DisplayStyle.Flex;
            }
        }
    }
}
