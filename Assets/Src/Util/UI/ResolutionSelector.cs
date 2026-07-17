using System;
using System.Linq;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Util.UI
{
    public class ResolutionSelector : MonoBehaviour
    {
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TMP_Text text;

        [Header("Util")]
        [SerializeField] private MonospaceFix monospace;

        [Inject] private Settings.Visual _visual;

        private void OnEnable()
        {
            Refresh();

            leftButton.onClick.AddListener(Previous);
            rightButton.onClick.AddListener(Next);

            _visual.ResolutionChanged += OnChanged;
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(Previous);
            rightButton.onClick.RemoveListener(Next);

            _visual.ResolutionChanged -= OnChanged;
        }

        private void Previous()
        {
            var resolutions = _visual.AvailableResolutions
                .GroupBy(r => (r.width, r.height))
                .Select(g => g.First())
                .ToArray();

            var current = _visual.Resolution;

            var index = Array.FindIndex(resolutions, r => r.width == current.width && r.height == current.height);

            index = (index - 1 + resolutions.Length) % resolutions.Length;

            _visual.Resolution = resolutions[index];
        }

        private void Next()
        {
            var resolutions = _visual.AvailableResolutions;
            var current = _visual.Resolution;
            var index = Array.FindIndex(resolutions, r => r.width == current.width && r.height == current.height);

            index = (index + 1) % resolutions.Length;

            _visual.Resolution = resolutions[index];
        }

        private void OnChanged(Resolution _) => Refresh();

        private void Refresh()
        {
            var r = _visual.Resolution;
            text.text = monospace.Fix($"{r.width}x{r.height}");
        }
    }
}