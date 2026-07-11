using System;
using System.Linq;
using Levels.Directorship;
using TMPro;
using UnityEngine;

namespace Levels.UI.Prewarming
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DisplayPrewarmingSteps : MonoBehaviour
    {
        [SerializeField] private LevelPrewarmer prewarmer;
        [SerializeField] private StageDisplayName[] stageNames;

        private TextMeshProUGUI _text;
        private string _template;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _template = _text.text;
        }

        private void OnEnable()
        {
            prewarmer.StepCompleted += OnStepCompleted;
        }

        private void OnDisable()
        {
            prewarmer.StepCompleted -= OnStepCompleted;
        }

        private void OnStepCompleted(int totalSteps, int currentStep, LevelPrewarmer.Stage stage)
        {
            _text.text = _template
                .Replace(@"\TOTAL\", totalSteps.ToString())
                .Replace(@"\CURRENT\", currentStep.ToString())
                .Replace(@"\STAGE\", ToDisplayName(stage));
        }

        private string ToDisplayName(LevelPrewarmer.Stage stage)
        {
            return stageNames
                .FirstOrDefault(x => x.stage == stage)
                ?.displayName
                ?? stage.ToString();
        }

        [Serializable]
        private class StageDisplayName
        {
            public LevelPrewarmer.Stage stage;

            [TextArea]
            public string displayName;
        }
    }
}