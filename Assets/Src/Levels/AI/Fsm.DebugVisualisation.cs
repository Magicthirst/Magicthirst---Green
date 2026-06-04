
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
        private static readonly Dictionary<FsmState, Color> StatesColors = new();

        private const float BackgroundAlpha = 0.6f;

        public static bool DebugVisualisationVisibility = true;

        private void DebugAwake()
        {
            foreach (var state in _states)
            {
                if (StatesColors.ContainsKey(state))
                {
                    continue;
                }

                StatesColors[state] = GetRandomColor();
            }
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

            if (!StatesColors.TryGetValue(state, out var backgroundColor))
            {
                backgroundColor = Color.black;
            }

            var textColor = Color.white;
            var textPosition = transform.position + Vector3.up * 1f;

            var text = $"Current state: {state.GetType().Name}";

            D.raw(new Shape.Text(textPosition, text), backgroundColor, textColor);
        }
#endif
    }
}