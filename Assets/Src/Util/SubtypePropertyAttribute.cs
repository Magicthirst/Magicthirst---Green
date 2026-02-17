using System;
using UnityEngine;

namespace Util
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SubtypePropertyAttribute : PropertyAttribute
    {
        public readonly Type BaseType;
        public readonly bool Required;

        public SubtypePropertyAttribute(Type baseType, bool required = true)
        {
            BaseType = baseType;
            Required = required;
        }
    }
}