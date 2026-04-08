using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VContainer.Internal
{
    public sealed class OpenGenericFuncInstanceProvider : IInstanceProvider, IClosedRegistrationProvider
    {
        class TypeParametersEqualityComparer : IEqualityComparer<Type[]>
        {
            public bool Equals(Type[] x, Type[] y)
            {
                if (x == null || y == null) return x == y;
                if (x.Length != y.Length) return false;

                for (var i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i]) return false;
                }
                return true;
            }

            public int GetHashCode(Type[] typeParameters)
            {
                var hash = 5381;
                foreach (var typeParameter in typeParameters)
                {
                    hash = ((hash << 5) + hash) ^ typeParameter.GetHashCode();
                }
                return hash;
            }
        }

        readonly Type implementationType;
        readonly Lifetime lifetime;
        readonly Func<IObjectResolver, Type[], object> factory;

        readonly ConcurrentDictionary<Type[], Registration> constructedRegistrations = new ConcurrentDictionary<Type[], Registration>(new TypeParametersEqualityComparer());
        readonly Func<Type[], Registration> createRegistrationFunc;

        public OpenGenericFuncInstanceProvider(Type implementationType, Lifetime lifetime, Func<IObjectResolver, Type[], object> factory)
        {
            this.implementationType = implementationType;
            this.lifetime = lifetime;
            this.factory = factory;
            createRegistrationFunc = CreateRegistration;
        }

        public Registration GetClosedRegistration(Type closedInterfaceType, Type[] typeParameters)
        {
            return constructedRegistrations.GetOrAdd(typeParameters, createRegistrationFunc);
        }

        Registration CreateRegistration(Type[] typeParameters)
        {
            var newType = implementationType.MakeGenericType(typeParameters);
            var spawner = new FuncInstanceProvider(resolver => factory(resolver, typeParameters));

            return new Registration(newType, lifetime, new List<Type>(1) { newType }, spawner);
        }

        public object SpawnInstance(IObjectResolver resolver)
        {
            throw new InvalidOperationException();
        }
    }
}