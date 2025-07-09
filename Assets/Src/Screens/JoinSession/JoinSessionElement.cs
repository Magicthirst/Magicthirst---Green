using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens.JoinSession
{
    [UxmlElement(nameof(JoinSessionElement))]
    public partial class JoinSessionElement : VisualElement
    {
        public event Action CancelRequested;
        public event Action<string> JoinRequested;

        private Label _errorLabel;

        public JoinSessionElement()
        {
            var asset = Resources.Load<VisualTreeAsset>(nameof(JoinSessionElement));
            if (asset == null)
            {
                Debug.LogError($"UXML asset '{nameof(JoinSessionElement)}' not found in Resources for EnterElement.");
                throw new FileNotFoundException($"UXML asset '{nameof(JoinSessionElement)}' not found.");
            }

            asset.CloneTree(this);

            var hostIdField = this.Q<TextField>("HostId");
            _errorLabel = this.Q<Label>("ErrorLabel");

            this.BindButtons(
                ("Join", () => JoinRequested?.Invoke(hostIdField.value)),
                ("Cancel", () => CancelRequested?.Invoke())
            );
        }

        public void DisplayError(string message)
        {
            _errorLabel.text = message;
            _errorLabel.style.display = DisplayStyle.Flex;
        }
    }
}
