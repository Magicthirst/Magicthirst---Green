#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Util
{
    [CustomPropertyDrawer(typeof(SubtypePropertyAttribute))]
    public class SubtypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Type is serialized as string
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, $"VisualElementTypeAttribute can only be applied to Type fields", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var (displayNames, classNames) = GetNames();

            var currentAssemblyQualifiedName = property.stringValue;
            var currentIndex = classNames.IndexOf(currentAssemblyQualifiedName);

            var previousColor = GUI.color;
            if (string.IsNullOrEmpty(property.stringValue))
            {
                GUI.color = Color.red;
            }

            var newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayNames.ToArray());
            GUI.color = previousColor;

            if (newIndex != currentIndex)
            {
                property.stringValue = classNames[newIndex];
            }

            EditorGUI.EndProperty();
        }

        private (List<string> DisplayNames, List<string> ClassNames) GetNames()
        {
            var attr = (SubtypePropertyAttribute) attribute;
            var baseType = attr.BaseType;

            var types = TypeCache.GetTypesDerivedFrom(baseType).ToList();

            return
            (
                DisplayNames: types.Select(t => t.Name).Prepend("None").ToList(),
                ClassNames: types.Select(t => t.FullName).Prepend("").ToList()
            );
        }
    }
}
#endif