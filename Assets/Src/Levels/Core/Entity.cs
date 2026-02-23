using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "Entity", menuName = "Core/Entities/Entity", order = 1)]
    [Serializable]
    public class Entity : CoreObject
    {
        public IEnumerable<CoreObject> FlattenedComponents => components?
            .SelectMany(c => c is Entity e ? e.FlattenedComponents.Append(c) : new[] { c })
            ?? Enumerable.Empty<CoreObject>();
        
        [SerializeField] protected CoreObject[] components;
        
        public override void Init()
        {
            foreach (var component in components)
            {
                component.Init();
            }
        }

        public override void Dispose()
        {
            foreach (var component in components)
            {
                component.Dispose();
            }
        }        
    }
}