using System;
using Levels;
using Levels.AI;
using Levels.Core;
using Levels.Core.Room;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using VContainer;
using VContainer.Internal;
using VContainer.Unity;

namespace DI
{
    public class EntityContextScope : LifetimeScope
    {
        [SerializeField] private Entity entity;
        [SerializeReference]
        [SubclassSelector]
        private IConfig[] configs = {};

        protected override void Awake()
        {
            Debug.Assert(entity != null, gameObject.name, gameObject);
            entity = Instantiate(entity);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameObject);
            builder.Register<MonoBehaviour>(_ => this, Lifetime.Scoped);

            if (TryGetComponent(out RoomMemberTag roomMemberTag))
            {
                builder.Register
                (
                    resolver => resolver.Resolve<Func<int, Room>>().Invoke(roomMemberTag.RoomId),
                    Lifetime.Singleton
                );
                builder.Register(resolver => resolver.Resolve<Room>().Units, Lifetime.Singleton);
                builder.Register(resolver => resolver.Resolve<Room>().Healing, Lifetime.Singleton);
            }

            if (gameObject.TryGetComponent(out Fsm fsm))
            {
                builder.RegisterInstance(fsm);
            }

            if (gameObject.TryGetComponent(out IMovementInputSource movement1))
            {
                builder.RegisterInstance(movement1)
                    .As<IObservableMovement>();
            }
            else if (gameObject.TryGetComponent(out IObservableMovement movement2))
            {
                builder.RegisterInstance(movement2);
            }

            foreach (var config in configs)
            {
                try
                {
                    builder.RegisterInstance(config, config.GetType());
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
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

                resolver.ResolveOrDefault<RoomUnits>()?.Register(entity);
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
                    builder.Register(registration).AsSelf().AsImplementedInterfaces();
                }
            }
        }

        private void FixedUpdate() => entity.FixedUpdate();
    }
}