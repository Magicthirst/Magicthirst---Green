using System;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Util.UI
{
    public class ScreenModeSelector : MonoBehaviour
    {
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TextMeshProUGUI text;

        [Header("Util")]
        [SerializeField] private MonospaceFix monospace;

        [Inject] private Settings.Visual _visual;

        private FullScreenMode[] _modes;

        private void Awake()
        {
            _modes = new[]
            {
                FullScreenMode.ExclusiveFullScreen,
                FullScreenMode.FullScreenWindow,
                FullScreenMode.MaximizedWindow,
                FullScreenMode.Windowed
            };
        }

        private void OnEnable()
        {
            Refresh();

            leftButton.onClick.AddListener(Previous);
            rightButton.onClick.AddListener(Next);

            _visual.ScreenModeChanged += OnChanged;
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(Previous);
            rightButton.onClick.RemoveListener(Next);

            _visual.ScreenModeChanged -= OnChanged;
        }

        private void Previous()
        {
            var index = Array.IndexOf(_modes, _visual.ScreenMode);
            index = (index - 1 + _modes.Length) % _modes.Length;
            _visual.ScreenMode = _modes[index];
        }

        private void Next()
        {
            var index = Array.IndexOf(_modes, _visual.ScreenMode);
            index = (index + 1) % _modes.Length;
            _visual.ScreenMode = _modes[index];
        }

        private void OnChanged(FullScreenMode _) => Refresh();

        private void Refresh()
        {
            text.text = monospace.Fix(_visual.ScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => "FULLSCREEN",
                FullScreenMode.FullScreenWindow => "BORDERLESS",
                FullScreenMode.MaximizedWindow => " MAXIMIZED",
                FullScreenMode.Windowed => "WINDOWED",
                _ => _visual.ScreenMode.ToString()
            });
        }
    }
}