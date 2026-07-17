using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Levels.Visual
{
    public sealed class BloodSpatterPool : MonoBehaviour
    {
        private static readonly int Seed = Shader.PropertyToID("_Seed");

        [SerializeField] private DecalProjector prefab;
        [SerializeField] private int poolSize = 128;

        private static BloodSpatterPool _instance;

        private static Material[] _materials;
        private DecalProjector[] _pool;
        private int _next;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public IEnumerable<Action> WarmUp()
        {
            _materials = new Material[10];
            for (var i = 0; i < _materials.Length; i++)
            {
                var index = i;
                yield return () => {
                    _materials[index] = new Material(prefab.material);
                    _materials[index].SetFloat(Seed, index * index);
                 };
            }

            _pool = new DecalProjector[poolSize];
            for (var i = 0; i < poolSize; i++)
            {
                var index = i;
                yield return () =>
                {
                    var decal = Instantiate(prefab, transform);
                    decal.gameObject.SetActive(false);
                    _pool[index] = decal;
                };
            }
        }

        public static void Spawn(Vector3 position, Quaternion rotation)
        {
            var decal = _instance._pool[_instance._next];
            _instance._next = (_instance._next + 1) % _instance._pool.Length;

            decal.transform.SetPositionAndRotation(position, rotation);
            decal.material = _materials[Random.Range(0, _materials.Length)];

            decal.gameObject.SetActive(false);
            decal.gameObject.SetActive(true);
        }
    }
}