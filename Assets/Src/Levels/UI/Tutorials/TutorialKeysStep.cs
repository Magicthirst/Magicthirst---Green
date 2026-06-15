using System;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<InputAction, int> _playedActions;
        private Dictionary<InputAction, int> _notPlayedActions;
        private LevelActivityMask _completedSteps = 0;

        [Inject] private PlayerInput _input;
        [Inject] private KeysActions _keysActions;

        public bool IsCompleted(LevelActivityMask step) => (_completedSteps & TutorialSpecificsPart & step) != 0;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _rawText = _text.text;
            _text.text = _keysActions.Apply(_rawText, appliedActions: out _notPlayedActions);
            _playedActions = new Dictionary<InputAction, int>();
        }

        private void OnEnable()
        {
            var map = _input.currentActionMap;

            _inputObservers = _notPlayedActions.Keys
                .Select(action => map
                    .ConsumeAction(action.name)
                    .OnPerformed(() => Remove(action)))
                .ToArray();
        }

        private void Remove(InputAction action)
        {
            var step = _keysActions.StepOf(action);
            if (!_notPlayedActions.ContainsKey(action) || !step.IsPlayableNow())
            {
                return;
            }

            _playedActions.TryAdd(action, 0);
            _playedActions[action]++;

            if (--_notPlayedActions[action] <= 0)
            {
                _notPlayedActions.Remove(action);
                _completedSteps |= (LevelActivityMask)step;
            }

            if (_notPlayedActions.Count != 0)
            {
                _text.text = _keysActions.Apply(_rawText, played: _playedActions);
            }
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