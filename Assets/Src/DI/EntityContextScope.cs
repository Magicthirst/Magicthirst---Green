using System;
using Levels;
using Levels.Abilities.Impacts;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class EntityContextScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            if (gameObject.TryGetComponent(out Health health))
            {
                builder.RegisterInstance(health);
            }

            RegisterConsumerOverride<ImpulseImpact>();
            RegisterConsumerOverride<DamageImpact>();
            RegisterConsumerOverride<ShotImpact>();

            return;

            void RegisterConsumerOverride<T>() where T : IImpact => builder.Register
            (
                resolver => resolver.Resolve<Func<GameObject, IImpactConsumer<T>>>().Invoke(gameObject),
                Lifetime.Transient
            );
        }
    }
}