using System;
using Common;
using Levels.Directorship;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using static Levels.Directorship.LevelActivityMask;

namespace Levels.UI.PauseMenu
{
    public class PauseMenuController : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => Pause;

        [SerializeField] private GameObject block;
        [SerializeField] private Slider soundSlider;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private AudioMixer audioMixer;

        private LevelActivityMask _previousMask;

        private IDisposable _observer;

        [Inject] private PlayerInput _playerInput;
        
        [Inject] private IGameNavigation _navigation;

        private void Start()
        {
            _observer = _playerInput.currentActionMap.ConsumeAction("Pause").OnPerformed(() =>
            {
                LevelDirector.ActivityMask = LevelDirector.ActivityMask != Pause ? Pause : _previousMask;
            });
        }

        protected override void DidEnabled()
        {
            block.SetActive(true);

            soundSlider.onValueChanged.AddListener(OnSoundValueChanged);
            continueButton.onClick.AddListener(OnContinueClicked);
            exitButton.onClick.AddListener(OnExitClicked);
        }

        protected override void DidDisabled()
        {
            block.SetActive(false);

            soundSlider.onValueChanged.RemoveListener(OnSoundValueChanged);
            continueButton.onClick.RemoveListener(OnContinueClicked);
            exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnSoundValueChanged(float value)
        {
            value = Mathf.Max(value, 0.0001f);
            if (!audioMixer.SetFloat("Volume", Mathf.Log10(value) * 20f))
            {
                Debug.LogWarning("audioMixer's MasterVolume float not assigns");
            }
        }

        private void OnDestroy() => _observer?.Dispose();

        private void OnContinueClicked() => LevelDirector.ActivityMask = _previousMask;

        private void OnExitClicked() => _navigation.GoMainMenu();

        protected override void OnMaskChanged(LevelActivityMask previous, LevelActivityMask _) => _previousMask = previous;
    }
}