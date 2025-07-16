using Assets;
using Common;
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
                    resolver => new MenuUserSession(resolver.Resolve<IClientAuthenticator>()),
                    Lifetime.Singleton
                )
                .AsImplementedInterfaces();

            builder.RegisterInstance<IAssignConnectionRole>(_connectionRole).AsSelf();
            builder.RegisterInstance<IsReceiving>(_connectionRole.IsReceiving).AsSelf();
            builder.RegisterInstance<IsPublishingInput>(_connectionRole.IsPublishingInput).AsSelf();
            builder.RegisterInstance<IsPublishingUpdates>(_connectionRole.IsPublishingUpdates).AsSelf();
        }
    }
}
