using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Levels.Extensions
{
    public class Coroutines : IDisposable
    {
        private readonly MonoBehaviour _behaviour;
        private readonly List<Coroutine> _routines = new();

        public Coroutines(MonoBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void Run(IEnumerator routine)
        {
            _behaviour.StartCoroutine(routine);
        }

        public void Dispose()
        {
            foreach (var coroutine in _routines)
            {
                _behaviour.StopAllCoroutines();
            }
        }
    }
}