using System;
using UnityEngine;
using VContainer;

namespace Levels.Core
{
    public abstract class CoreObject : ScriptableObject, IDisposable
    {
        [NonSerialized] [Inject] public GameObject Owner;
        [NonSerialized] [Inject] public MonoBehaviour Runner;

        public abstract void Init();

        public abstract void Dispose();
    }
}