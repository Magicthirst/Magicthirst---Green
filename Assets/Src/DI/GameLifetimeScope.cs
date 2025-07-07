using System;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Web;

namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private double tokenRefreshIntervalSeconds;
        [SerializeField] private AuthorizedClient.Config authorizedClientConfig;

        private TimeSpan _TokenRefreshInterval => TimeSpan.FromSeconds(tokenRefreshIntervalSeconds);

        // TODO OnlineGameSession or null object which will be injected to syncing components
        [CanBeNull] public AuthorizedClient AuthorizedClient { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // TODO launching here through delegates
        }
    }
}