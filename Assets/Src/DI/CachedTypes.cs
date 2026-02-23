using System;
using System.Linq;
using System.Reflection;
using Levels.IntentsImpacts;

namespace DI
{
    internal static class CachedTypes
    {
        public static readonly Type[] Intents = Assembly
            .GetAssembly(typeof(IIntent))
            .GetTypes()
            .Where(t => typeof(IIntent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        public static readonly Type[] Impacts = Assembly
            .GetAssembly(typeof(IImpact))
            .GetTypes()
            .Where(t => typeof(IImpact).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();
    }
}