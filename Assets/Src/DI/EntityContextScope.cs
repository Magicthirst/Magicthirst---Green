using System;
using System.Collections.Generic;
using Levels;
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
            entity = Instantiate(entity);
            configs ??= new List<ScriptableObject>();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameObject);
            builder.Register<MonoBehaviour>(_ => this, Lifetime.Scoped);

            if (gameObject.TryGetComponent(out Fsm fsm))
            {
                builder.RegisterInstance(fsm);
            }

            if (gameObject.TryGetComponent(out IMovementInputSource movement))
            {
                builder.RegisterInstance(movement);
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
                foreach (var component in entity.LazyComponents)
                {
                    resolver.Inject(component);
                }

                resolver.Inject(entity);
                entity.Init();
            });
            builder.RegisterDisposeCallback(_ => entity.Dispose());

            return;

            void RegisterConsumerOverrides()
            {
                builder.Register
                (
                    interfaceType: typeof(IImpactConsumer<>),
                    implementationFactory: (resolver, tImpact) =>
                    {
                        var intentsImpacts = resolver.Resolve<IntentsImpacts>();
                        var affectable = resolver.ResolveOrDefault<IModifyingImpacts>();
                        return intentsImpacts.GetImpactConsumerFor(gameObject, tImpact, affectable);
                    },
                    lifetime: Lifetime.Scoped
                );
            }

            void RegisterEntityComponents()
            {
                foreach (var component in entity.LazyComponents)
                {
                    var registration = new InstanceRegistrationBuilder(component);
                    builder.Register(registration).AsSelf();
                }
            }
        }

        private void FixedUpdate() => entity.FixedUpdate();
    }
}