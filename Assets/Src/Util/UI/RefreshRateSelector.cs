using System;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Util.UI
{
    public class RefreshRateSelector : MonoBehaviour
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

            _visual.RefreshRateChanged += OnChanged;
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(Previous);
            rightButton.onClick.RemoveListener(Next);

            _visual.RefreshRateChanged -= OnChanged;
        }

        private void Previous()
        {
            var rates = _visual.AvailableRates;
            var current = _visual.RefreshRate;

            var index = Array.FindIndex(rates, r => r.numerator == current.numerator && r.denominator == current.denominator);

            index = (index - 1 + rates.Length) % rates.Length;

            _visual.RefreshRate = rates[index];
        }

        private void Next()
        {
            var rates = _visual.AvailableRates;
            var current = _visual.RefreshRate;

            var index = Array.FindIndex(rates, r => r.numerator == current.numerator && r.denominator == current.denominator);

            index = (index + 1) % rates.Length;

            _visual.RefreshRate = rates[index];
        }

        private void OnChanged(RefreshRate _) => Refresh();

        private void Refresh()
        {
            var rr = _visual.RefreshRate;
            text.text = monospace.Fix($"{Mathf.RoundToInt((float)rr.value)}Hz");
        }
    }
}