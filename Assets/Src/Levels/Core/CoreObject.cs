using System;
using UnityEngine;

namespace Levels.Core
{
    public abstract class CoreObject : ScriptableObject, IDisposable
    {
        public abstract void Init();

        public abstract void Dispose();
    }
}