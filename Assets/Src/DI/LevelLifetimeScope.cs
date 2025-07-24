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
        public event Action ConnectionEstablished;

        [CanBeNull] private ISyncConnection _connection = null;

        protected override void Configure(IContainerBuilder builder)
        {
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
                            ConnectionEstablished?.Invoke();
                        }
                        else
                        {
                            Debug.Log("Connection not Established");
                        }
                    });

                    return this;
                }, Lifetime.Singleton)
                .AsSelf();

            builder.RegisterDisposeCallback(resolver => resolver.ResolveOrDefault<ISyncConnection>()?.Dispose());

            builder
                .Register<SendInput.SendMovement>(_ => vector2 => _connection!.SendMovement(vector2), Lifetime.Singleton)
                .AsSelf();
        }
    }
}
