using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Levels.UI.Tutorials
{
    [Serializable]
    public class KeysActions : ISharedConfig
    {
        private static readonly string[] Wasd = { "w", "a", "s", "d" };
        private static readonly string[] Arrows = { "↑", "←", "→", "↓" };

        private static readonly Dictionary<string, string> KeyAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Up Arrow"] = "↑",
            ["Down Arrow"] = "↓",
            ["Left Arrow"] = "←",
            ["Right Arrow"] = "→",

            ["upArrow"] = "↑",
            ["downArrow"] = "↓",
            ["leftArrow"] = "←",
            ["rightArrow"] = "→",

            ["up"] = "↑",
            ["down"] = "↓",
            ["left"] = "←",
            ["right"] = "→",
        };

        [SerializeField] private string groupTemplate = "[GROUP]";
        [SerializeField] private string appliedActionDecoration = "<color=#00ff00>ACTION</color>";

        [SerializeField]
        [TextArea]
        private string groupsSeparator;
        [SerializeField] private KeySymbolInputAction[] keysActions;

        public string Apply(string rawText, Dictionary<InputAction, int> played)
        {
            var playedActions = new Dictionary<InputAction, int>(played);

            var parts = rawText.Split('\\').Select(part =>
            {
                if (!keysActions.TryGetFirst(out var item, item => part == item.key))
                {
                    return part;
                }

                var formatted = FormatGroups(GetDisplayKeys(item.Action));

                if (playedActions.ContainsKey(item.Action))
                {
                    formatted = appliedActionDecoration.Replace("ACTION", formatted);

                    if (--playedActions[item.Action] < 0)
                    {
                        playedActions.Remove(item.Action);
                    }
                }

                return formatted;
            });

            return string.Join("", parts);
        }

        public string Apply(string rawText, out Dictionary<InputAction, int> appliedActions)
        {
            var usedActions = new Dictionary<InputAction, int>();
            
            var parts = rawText.Split('\\').Select(part =>
            {
                if (!keysActions.TryGetFirst(out var item, item => part == item.key))
                {
                    return part;
                }

                usedActions.TryAdd(item.Action, 0);
                usedActions[item.Action]++;

                var formatted = FormatGroups(GetDisplayKeys(item.Action));

                return formatted;
            });

            appliedActions = usedActions;

            return string.Join("", parts);
        }

        public TutorialStep StepOf(InputAction inputAction) => keysActions.First(i => i.Action == inputAction).step;

        private string FormatGroups(IEnumerable<string> keys)
        {
            var groups = keys
                .GroupBy(GetKeyGroup)
                .Select(group => (key: group.Key, set: ApplySpecificReorders(group.ToHashSet())))
                .Select(group => group.key == KeysGroup.Text ? string.Join(", ", group.set) : JoinChars(group));

            return string.Join(groupsSeparator, groups);

            string JoinChars((KeysGroup key, IEnumerable<string> set) group)
            {
                return groupTemplate.Replace("GROUP", string.Join("", group.set));
            }
        }

        private IEnumerable<string> ApplySpecificReorders(HashSet<string> keys)
        {
            return
                keys.SetEquals(Wasd) ? Wasd :
                keys.SetEquals(Arrows) ? Arrows :
                keys;
        }

        private IEnumerable<string> GetDisplayKeys(InputAction action)
        {
            return action.controls
                .Select(ToDisplayName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct();
        }

        private string ToDisplayName(InputControl control)
        {
            var name = control.displayName;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = control.name;
            }

            return KeyAliases.GetValueOrDefault(name, name).ToLower();
        }

        private KeysGroup GetKeyGroup(string key) =>
            key.Length > 1 ? KeysGroup.Text :
            char.IsLetter(key[0]) ? KeysGroup.Alpha :
            char.IsDigit(key[0]) ? KeysGroup.Numeric :
            KeysGroup.Symbolic;

        private enum KeysGroup { Alpha, Numeric, Symbolic, Text }
    }

    [Serializable]
    public class KeySymbolInputAction
    {
        [Tooltip("Example: KEYS_MOVEMENT")]
        public string key;

        public TutorialStep step;

        [SerializeField] private InputActionReference rAction;
        public InputAction Action => _action ??= rAction.action;
        private InputAction _action;
    }
}