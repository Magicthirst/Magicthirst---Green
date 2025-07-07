using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens.Enter
{
    [UxmlElement(nameof(EnterElement))]
    public partial class EnterElement : VisualElement
    {
        public event Action SignUpRequested;
        public event Action<string> SignInRequested;
        public event Action EnterUnanimouslyRequested;

        private readonly Label _errorLabel;
        private readonly TextField _userIdField;

        public EnterElement()
        {
            var asset = Resources.Load<VisualTreeAsset>(nameof(EnterElement));
            if (asset == null)
            {
                Debug.LogError($"UXML asset '{nameof(EnterElement)}' not found in Resources for EnterElement.");
                throw new FileNotFoundException($"UXML asset '{nameof(EnterElement)}' not found.");
            }

            asset.CloneTree(this);
            this.BindButtons(
                ("SignUp", () => SignUpRequested?.Invoke()),
                ("SignIn", SignIn),
                ("SignAnonymously", () => EnterUnanimouslyRequested?.Invoke())
            );

            _errorLabel = this.Q<Label>("UserIdError");
            _userIdField = this.Q<TextField>("UserId");
        }

        public void DisplaySignInError(string message)
        {
            _errorLabel.text = message;
            _errorLabel.style.display = DisplayStyle.Flex;
        }

        private void SignIn()
        {
            _errorLabel.style.display = DisplayStyle.None;
            SignInRequested?.Invoke(_userIdField.value);
        }
    }
}
