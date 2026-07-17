using System;
using Levels.Directorship;
using Shared;
using UnityEngine;
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
        [SerializeField] private Button continueButton;
        [SerializeField] private Button exitButton;

        private LevelActivityMask _previousMask;

        private IDisposable _observer;

        [Inject] private PlayerInput _playerInput;
        [Inject] private GameNavigation _navigation;

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

            continueButton.onClick.AddListener(OnContinueClicked);
            exitButton.onClick.AddListener(OnExitClicked);
        }

        protected override void DidDisabled()
        {
            block.SetActive(false);

            continueButton.onClick.RemoveListener(OnContinueClicked);
            exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnDestroy() => _observer?.Dispose();

        private void OnContinueClicked() => LevelDirector.ActivityMask = _previousMask;

        private void OnExitClicked() => _navigation.GoMainMenu();

        protected override void OnMaskChanged(LevelActivityMask previous, LevelActivityMask _) => _previousMask = previous;
    }
}