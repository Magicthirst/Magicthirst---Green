using System.Collections.Generic;
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

            string text;
            Color textColor;
            Color backgroundColor;

            if (state is null)
            {
                text = "null";
                textColor = Color.darkRed;
                backgroundColor = Color.white;
            }
            else
            {
                if (!StatesColors.TryGetValue(state, out backgroundColor))
                {
                    backgroundColor = Color.black;
                }
                textColor = Color.white;
                text = $"Current state: {state.GetType().Name}";
            }

            var textPosition = transform.position + Vector3.up * 1f;
            D.raw(new Shape.Text(textPosition, text), backgroundColor, textColor);
        }
#endif
    }
}