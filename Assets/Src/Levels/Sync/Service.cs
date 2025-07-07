using System;
using JetBrains.Annotations;
using UnityEngine;
using Web.Sync;

namespace Levels.Sync
{
    public class Service : SyncBehaviour
    {
        public static Service instance = null;

        [NonSerialized] [CanBeNull] public Output PlayerOutput = null;
        
        [SerializeField] private SyncConfig config;

        [CanBeNull] private SyncClient _client = null;

        protected override void Awake()
        {
            base.Awake();

            if (instance != null)
            {
                throw new Exception($"{nameof(Service)} is a singleton and sould have only one instance");
            }
            instance = this;
        }

        private void Start()
        {
            _client = new SyncClient(PlayerOutput!.TryCollectPlayerUpdate, config, Debug.Log);
        }

        private void OnEnable()
        {
            _client!.Start();
        }

        private void Update()
        {
            _client!.Update(Time.deltaTime);
        }

        private void OnDisable()
        {
            _client?.Dispose();
            _client = null;
        }
    }
}
