using System;
using System.Collections.Generic;
using Levels.Directorship;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using static System.Linq.Enumerable;
using static Levels.Directorship.LevelActivityMask;
using static Levels.PlayerInputExtension;

namespace Levels.UI.Tutorials
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TutorialKeysStep : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialWindow;

        private TextMeshProUGUI _text;

        private string _rawText;
        private DisposableAction[] _inputObservers = Array.Empty<DisposableAction>();
        private IEnumerable<InputAction> _actionsToPlay;
        private TutorialStep _completedSteps = 0;

        [Inject] private PlayerInput _input;
        [Inject] private TutorialAbilities _tutorialAbilities;

        public bool IsCompleted(LevelActivityMask step) => ((LevelActivityMask)_completedSteps & TutorialSpecificsPart & step) != 0;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _rawText = _text.text;
            _text.text = _tutorialAbilities.Apply(_rawText, out _actionsToPlay);
        }

        private void OnEnable()
        {
            var map = _input.currentActionMap;

            _inputObservers = _actionsToPlay
                .Select(action => map
                    .ConsumeAction(action.name)
                    .OnPerformed(() => Remove(action)))
                .ToArray();
        }

        private void Remove(InputAction action)
        {
            if (!_tutorialAbilities.TryGetNextStep(action, _completedSteps, out var step))
            {
                return;
            }

            if (((int)LevelDirector.ActivityMask & (int)TutorialSpecificsPart & (int)step) == 0)
            {
                return;
            }

            _completedSteps |= step;
            _text.text = _tutorialAbilities.Apply(_rawText, _completedSteps);
        }

        private void OnDisable()
        {
            foreach (var observer in _inputObservers ?? Empty<IDisposable>())
            {
                observer.Dispose();
            }

            _inputObservers = Array.Empty<DisposableAction>();
        }
    }
}