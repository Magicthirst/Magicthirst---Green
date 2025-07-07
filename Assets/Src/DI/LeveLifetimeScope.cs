using Levels.Sync;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public abstract class LeveLifetimeScope : LifetimeScope
    {
        public static void ContainOfflineSettings()
        {
            var container = new GameObject("LevelLifetimeScope Container");
            container.AddComponent<Offline>();
            container.transform.parent = null;
            DontDestroyOnLoad(container);
        }

        public static void ContainOnlineSettings()
        {
            var container = new GameObject("LevelLifetimeScope Container");
            container.AddComponent<Online>();
            container.transform.parent = null;
            DontDestroyOnLoad(container);
        }

        private class Offline : LeveLifetimeScope
        {
            protected override void Configure(IContainerBuilder builder)
            {
                base.Configure(builder);

                builder.Register<SyncBehaviour.IsOnline>(_ => () => false, Lifetime.Singleton);
            }
        }

        private class Online : LeveLifetimeScope
        {
            protected override void Configure(IContainerBuilder builder)
            {
                base.Configure(builder);

                builder.Register<SyncBehaviour.IsOnline>(_ => () => true, Lifetime.Singleton);
            }
        }
    }
}
