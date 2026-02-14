using System;
using UnityEngine;

namespace Util
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SubtypePropertyAttribute : PropertyAttribute
    {
        public readonly Type BaseType;

        public SubtypePropertyAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}