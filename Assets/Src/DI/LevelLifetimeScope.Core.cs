using Levels.Util.MasksRegistry;
using UnityEngine;
using VContainer;

namespace DI
{
    public partial class LevelLifetimeScope
    {
        [SerializeField] private new Camera camera;

        private void ConfigureCore(IContainerBuilder builder)
        {
            builder.RegisterInstance(new MasksRegistry()).AsSelf();

            builder.RegisterInstance(camera);
        }
    }
}