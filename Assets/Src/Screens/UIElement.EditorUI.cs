#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens
{
    public partial class UIElement
    {
        [MenuItem("GameObject/UI Toolkit Extension/Element Object", false, 10)]
        public static void CreateMenuItem()
        {
            var newObject = new GameObject("UIElement");
            newObject.AddComponent<UIElement>();
            Selection.activeGameObject = newObject;
        }
    }

    [CustomPropertyDrawer(typeof(VisualElementTypeAttribute))]
    public class VisualElementTypeDrawer : PropertyDrawer
    {
        private static List<string> _displayNames;
        private static List<string> _classNames;

        static VisualElementTypeDrawer() => RefreshCache();

        [MenuItem("Tools/Refresh VisualElement Type Cache")]
        public static void RefreshCache()
        {
            var allValidTypes = TypeCache.GetTypesDerivedFrom<VisualElement>()
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsGenericTypeDefinition &&
                    type.GetConstructor(Type.EmptyTypes) != null
                )
                .ToList();

            var myTypes = allValidTypes.Where(type => type.Namespace?.StartsWith(typeof(UIElement).Namespace ?? "") == true);
            var libTypes = allValidTypes.Where(type => type.Namespace?.StartsWith(typeof(UIElement).Namespace ?? "") == false);

            var validTypes = new List<Type>();
            validTypes.AddRange(myTypes);
            validTypes.AddRange(libTypes);

            _displayNames = validTypes.Select(t => t.Name).Prepend("None").ToList();
            _classNames = validTypes.Select(t => t.AssemblyQualifiedName).Prepend("").ToList();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Type is serialized as string
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, $"VisualElementTypeAttribute can only be applied to Type fields", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var currentAssemblyQualifiedName = property.stringValue;
            var currentIndex = _classNames.IndexOf(currentAssemblyQualifiedName);

            var newIndex = EditorGUI.Popup(position, label.text, currentIndex, _displayNames.ToArray());

            if (newIndex != currentIndex)
            {
                property.stringValue = _classNames[newIndex];
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
