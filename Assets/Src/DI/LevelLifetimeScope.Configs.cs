using Levels.Util;
using UnityEngine;
using VContainer;

namespace DI
{
    public partial class LevelLifetimeScope
    {
        [SerializeReference]
        [SubclassSelector]
        private ISharedConfig[] sharedConfigs;

        private void ConfigureConfigs(IContainerBuilder builder)
        {
            foreach (var config in sharedConfigs)
            {
                builder.RegisterInstance(config, config.GetType());
            }
        }
    }
}