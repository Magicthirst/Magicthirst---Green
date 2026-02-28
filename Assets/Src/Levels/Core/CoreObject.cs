using System;
using UnityEngine;
using VContainer;

namespace Levels.Core
{
    public abstract class CoreObject : ScriptableObject, IDisposable
    {
        [Inject] public GameObject Owner;

        public abstract void Init();

        public abstract void Dispose();
    }
}