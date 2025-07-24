using Assets;
using Common;
using JetBrains.Annotations;
using Levels.Sync;
using Model;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Web;

namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ClientConfigAsset clientConfigAsset;

        private readonly ConnectionRoleState _connectionRole = new();
        [CanBeNull] private IConnector _connector = null;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder
                .Register(_ => clientConfigAsset.ToRecord(), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<IClientAuthenticator>
                (
                    resolver => new ClientAuthenticator(resolver.Resolve<ClientConfig>()),
                    Lifetime.Singleton
                )
                .AsSelf();

            builder
                .Register
                (
                    resolver => new MenuUserSession
                    (
                        authenticator: resolver.Resolve<IClientAuthenticator>(),
                        setConnector: connector => _connector = connector
                    ),
                    Lifetime.Singleton
                )
                .AsImplementedInterfaces();

            builder.RegisterDisposeCallback(resolver => resolver.Resolve<IMenuUserSession>().Dispose());

            builder.Register(_ => _connector, Lifetime.Transient).AsSelf();

            builder.RegisterInstance<IAssignConnectionRole>(_connectionRole).AsSelf();
            builder
                .RegisterInstance<IsReceiving>(() => _connector != null && _connectionRole.IsReceiving())
                .AsSelf();
            builder
                .RegisterInstance<IsPublishingInput>(() => _connector != null && _connectionRole.IsPublishingInput())
                .AsSelf();
            builder
                .RegisterInstance<IsPublishingUpdates>(() => _connector != null && _connectionRole.IsPublishingUpdates())
                .AsSelf();
        }
    }
}
