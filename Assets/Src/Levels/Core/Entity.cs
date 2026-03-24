using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core.Passives;
using UnityEngine;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "Entity", menuName = "Core/Entities/Entity", order = 1)]
    [Serializable]
    public class Entity : PassiveCoreObject
    {
        public IEnumerable<CoreObject> LazyComponents => GetLazyObjects();

        [SerializeField] protected CoreObject[] serialComponents;
        protected CoreObject[] Components = null;
        private PassiveCoreObject[] _passiveComponents;

        private Dictionary<Type, object> _componentCache = new();
        
        public override void Init()
        {
            foreach (var component in Components)
            {
                component.Init();
            }

            _passiveComponents = Components
                .Where(component => component is PassiveCoreObject)
                .Cast<PassiveCoreObject>()
                .ToArray();
        }

        public override void FixedUpdate()
        {
            foreach (var component in _passiveComponents)
            {
                component.FixedUpdate();
            }
        }

        public override void Dispose()
        {
            foreach (var component in Components)
            {
                component.Dispose();
            }
        }

        public bool TryGetComponent<T>(out T component) where T : class
        {
            if (_componentCache.TryGetValue(typeof(T), out var temp))
            {
                component = (T)temp;
                return true;
            }

            component = LazyComponents.FirstOrDefault(component => component is T) as T;
            _componentCache[typeof(T)] = component;
            return component != null;
        }

        private IEnumerable<CoreObject> GetLazyObjects()
        {
            if (Components != null)
            {
                return Components;
            }

            Components = serialComponents.Select(Instantiate).ToArray();

            return Components.SelectMany(c => c is Entity e ? e.LazyComponents.Append(c) : new[] { c });
        }
    }
}