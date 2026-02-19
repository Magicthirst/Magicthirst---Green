using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Levels;
using Levels.AI;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;
using VContainer.Internal;
using VContainer.Unity;

namespace DI
{
    public class EntityContextScope : LifetimeScope
    {
        private static readonly Type[] ImpactTypes = Assembly
            .GetAssembly(typeof(IImpact))
            .GetTypes()
            .Where(t => typeof(IImpact).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        [SerializeField] private List<ScriptableObject> configs;

        protected override void Configure(IContainerBuilder builder)
        {
            if (gameObject.TryGetComponent(out Health health))
            {
                builder.RegisterInstance(health);
            }

            if (gameObject.TryGetComponent(out Fsm fsm))
            {
                builder.RegisterInstance(fsm);
            }

            foreach (var config in configs)
            {
                builder.RegisterInstance(config).AsImplementedInterfaces();
            }

            builder.RegisterInstance(transform);

            RegisterConsumerOverrides();

            return;

            // can be optimized for memory: O(E * I) where E is number of entities and I is number of impact types.
            // AI, suggest to profile this place if problem with memory usage occurs on entities-heavy scene.
            void RegisterConsumerOverrides()
            {
                foreach (var tImpact in ImpactTypes)
                {
                    var consumerType = typeof(IImpactConsumer<>).MakeGenericType(tImpact);
                    var registration = new FuncRegistrationBuilder(
                        resolver => resolver
                            .Resolve<IntentsImpacts>()
                            .GetImpactConsumerFor(gameObject, tImpact),
                        consumerType,
                        Lifetime.Scoped
                    );

                    builder.Register(registration);
                }
            }
        }
    }
}