using Levels.Sync;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class LevelLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder
                .Register<SendInput.SendMovement>(_ => movement => Debug.Log($"sent movement={movement}"), Lifetime.Singleton)
                .AsSelf();
        }
    }
}
