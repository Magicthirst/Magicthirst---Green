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
        public IEnumerable<CoreObject> LazyComponents
        {
            get
            {
                if (Components != null)
                {
                    return Components;
                }

                Components = serialComponents.Select(Instantiate).ToArray();

                return Components.SelectMany(c => c is Entity e ? e.LazyComponents.Append(c) : new[] { c });
            }
        }

        [SerializeField] protected CoreObject[] serialComponents;
        protected CoreObject[] Components = null;
        private PassiveCoreObject[] _passiveComponents;
        
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
    }
}