using Shared;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Util.UI
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        [Inject] private Settings.Audio _audioSettings;

        private void OnEnable()
        {
            slider.normalizedValue = _audioSettings.MasterVolume01;
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(float value) => _audioSettings.MasterVolume01 = value;
    }
}