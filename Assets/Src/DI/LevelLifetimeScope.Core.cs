using Levels.Util.MasksRegistry;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace DI
{
    [RequireComponent(typeof(PlayerInput))]
    public partial class LevelLifetimeScope
    {
        [SerializeField] private new Camera camera;

        private void ConfigureCore(IContainerBuilder builder)
        {
            builder.RegisterInstance(new MasksRegistry()).AsSelf();
            builder.RegisterInstance(GetComponent<PlayerInput>());
            builder.RegisterInstance(camera);
        }
    }
}