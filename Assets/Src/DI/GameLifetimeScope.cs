using Shared;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameNavigation gameNavigation;
        [SerializeField] private Settings.Audio audioSettings;
        [SerializeField] private Settings.Visual visualSettings;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameNavigation);
            builder.RegisterInstance(audioSettings);
            builder.RegisterInstance(visualSettings);

            builder.RegisterBuildCallback(_ =>
            {
                audioSettings.Init();
                visualSettings.Init();
            });
        }
    }
}
