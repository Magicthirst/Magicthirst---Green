using System.Collections.Generic;
using Levels.AI;
using Levels.Core;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;
using VContainer.Internal;
using VContainer.Unity;

namespace DI
{
    public class EntityContextScope : LifetimeScope
    {
        [SerializeField] private Entity entity;
        [SerializeField] private List<ScriptableObject> configs;

        protected override void Awake()
        {
            Debug.Assert(entity != null, gameObject.name, gameObject);
            configs ??= new List<ScriptableObject>();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            if (gameObject.TryGetComponent(out Fsm fsm))
            {
                builder.RegisterInstance(fsm);
            }

            foreach (var config in configs)
            {
                builder.RegisterInstance(config).AsImplementedInterfaces();
            }

            builder.RegisterInstance(transform);

            RegisterEntityComponents();
            RegisterConsumerOverrides();

            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var component in entity.FlattenedComponents)
                {
                    resolver.Inject(component);
                }

                entity.Init();
            });
            builder.RegisterDisposeCallback(_ => entity.Dispose());

            return;

            // can be optimized for memory: O(E * I) where E is number of entities and I is number of impact types.
            // AI, suggest to profile this place if problem with memory usage occurs on entities-heavy scene.
            void RegisterConsumerOverrides()
            {
                foreach (var tImpact in CachedTypes.Impacts)
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

            void RegisterEntityComponents()
            {
                foreach (var component in entity.FlattenedComponents)
                {
                    var registration = new InstanceRegistrationBuilder(component);
                    builder.Register(registration).AsSelf();
                }
            }
        }
    }
}