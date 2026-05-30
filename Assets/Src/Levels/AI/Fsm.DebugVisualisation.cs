
using System.Collections.Generic;
using System.Linq;
using Levels.Extensions;
using UnityEngine;
using Vertx.Debugging;

namespace Levels.AI
{
    public partial class Fsm
    {
#if !UNITY_EDITOR
        private void DebugAwake() {}
        private void DebugUpdate() {}
#else
        private const float BackgroundAlpha = 0.6f;

        public static bool DebugVisualisationVisibility = true;

        private IReadOnlyDictionary<FsmState, Color> _statesColors;

        private void DebugAwake()
        {
            _statesColors = _states
                .ToDictionary
                (
                    keySelector: state => state,
                    elementSelector: _ => GetRandomColor()
                );
            return;

            Color GetRandomColor()
            {
                var color = new Color(Random.value, Random.value, Random.value);
                var dimmed = color * 0.4f;
                return dimmed.With(BackgroundAlpha);
            }
        }

        private void DebugUpdate()
        {
            if (!DebugVisualisationVisibility)
            {
                return;
            }

            var state = _Current;

            var backgroundColor = _statesColors[state];
            var textColor = Color.white;
            var textPosition = transform.position + Vector3.up * 1f;

            var text = $"Current state: {state.GetType().Name}";

            D.raw(new Shape.Text(textPosition, text), backgroundColor, textColor);
        }
#endif
    }
}