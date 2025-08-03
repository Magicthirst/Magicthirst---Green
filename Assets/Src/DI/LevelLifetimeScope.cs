using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Levels.Sync;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class LevelLifetimeScope : LifetimeScope, IConnectionEstablishedEventHolder
    {
        public event Action<ISyncConnection> ConnectionEstablished;

        [SerializeField] private new Camera camera;

        [CanBeNull] private ISyncConnection _connection = null;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(camera);

            builder
                .Register<IConnectionEstablishedEventHolder>(resolver =>
                {
                    if (!resolver.TryResolve(out IConnector connector))
                    {
                        return this;
                    }

                    Debug.Log("Launching connection try");
                    Task.Run(async () =>
                    {
                        Debug.Log("Trying to connect");
                        try
                        {
                            _connection = await connector.Connect();
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                        if (_connection != null)
                        {
                            Debug.Log("ConnectionEstablished");
                            ConnectionEstablished?.Invoke(_connection);
                        }
                        else
                        {
                            Debug.Log("Connection not Established");
                        }
                    });

                    return this;
                }, Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<InstantiateJoinedPlayer>(_ => (playerId, prefab) =>
                {
                    var consumer = _connection!.GetForIndividual(playerId);
                    var scope = CreateChild(scopeBuilder =>
                    {
                        scopeBuilder.RegisterInstance(consumer).As<IConsumer>();
                    });

                    Debug.Log($"Instantiating playerId={playerId} prefab={prefab.name}");
                    var instance = Instantiate(prefab);
                    instance.GetComponent<ConsumerLifetimeScope>().parentReference.Object = scope;
                    return instance;
                }, Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<IReinitSource>(_ => _connection!, Lifetime.Transient)
                .AsSelf();
        }

        private void OnDisable()
        {
            _connection?.Dispose();
        }
    }
}
