using System;
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
    public partial class UIElement : MonoBehaviour, IUIReady
    {
        public event Action UIReady;

        [Tooltip("VisualElement which should be instantiated here")]
        [SerializeField]
        [VisualElementType]
        private string elementTypeName;
        private Type _elementType;
        private UIDocument _uiDocument;

        private VisualElement _instantiatedRootElement;

        [Inject] private IObjectResolver _resolver;

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
        }

        private void Start()
        {
            var root = _uiDocument.rootVisualElement[0];
            root.Clear();
            root.Add(_instantiatedRootElement);

            UIReady?.Invoke();
        }

        private void OnDestroy()
        {
            _instantiatedRootElement?.RemoveFromHierarchy();
            _instantiatedRootElement = null;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class VisualElementTypeAttribute : PropertyAttribute {}
}
