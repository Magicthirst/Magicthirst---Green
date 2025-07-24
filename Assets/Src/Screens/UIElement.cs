using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Screens
{
    [AddComponentMenu("UI Toolkit Extension/UIElement")]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(UIDocument))]
    public class UIElement : MonoBehaviour
    {
        [Tooltip("VisualElement which should be instantiated here")]
        [SerializeField]
        [VisualElementType]
        private string elementTypeName;
        private Type _elementType;
        private UIDocument _uiDocument;

        private VisualElement _instantiatedRootElement;

        [Inject] private IObjectResolver _resolver;

        [MenuItem("GameObject/UI Toolkit Extension/Element Object", false, 10)]
        public static void CreateMenuItem()
        {
            var newObject = new GameObject("UIElement");
            newObject.AddComponent<UIElement>();
            Selection.activeGameObject = newObject;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _elementType = Type.GetType(elementTypeName);
    
            if (_elementType == null)
            {
                Debug.LogWarning($"'{nameof(_elementType)}' is not set on {gameObject.name}. Cannot bind root VisualElement.", this);
                return;
            }

            // Keep these runtime checks for robustness, although the editor drawer prevents invalid selections.
            if (!typeof(VisualElement).IsAssignableFrom(_elementType))
            {
                Debug.LogError($"Selected type '{_elementType.Name}' is not a VisualElement. It must inherit from UnityEngine.UIElements.VisualElement.", this);
                return;
            }

            if (_elementType.GetConstructor(Type.EmptyTypes) == null)
            {
                Debug.LogError($"VisualElement type '{_elementType.Name}' must have a public parameterless constructor to be instantiated.", this);
                return;
            }

            _instantiatedRootElement = (VisualElement) _elementType.GetConstructors()[0].Invoke(new object[]{})
                                       ?? throw new NullReferenceException($"{_elementType}");
            if (Application.isPlaying)
            {
                _resolver.Inject(_instantiatedRootElement);
                _instantiatedRootElement.Query<VisualElement>().ForEach(_resolver.Inject);
            }

            var root = _uiDocument.rootVisualElement[0];
            root.Clear();
            root.Add(_instantiatedRootElement);
        }

        private void OnDestroy()
        {
            _instantiatedRootElement?.RemoveFromHierarchy();
            _instantiatedRootElement = null;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class VisualElementTypeAttribute : PropertyAttribute {}

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
